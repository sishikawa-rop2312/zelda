using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindBossController : MonoBehaviour
{
    public float moveSpeed = 1f; // 移動速度
    public float attackRange = 15f; // 攻撃範囲（マス数）
    public GameObject windAttackPrefab; // 風の攻撃のプレハブ
    public float attackSpeed = 1f; // 攻撃の速度
    public float attackCooldown = 5f; // 攻撃のクールダウン（5秒）
    public float attackDamage = 0.5f; // 攻撃のダメージ量
    public float teleportCooldown = 15f; // テレポートのクールダウン
    public LayerMask obstacleLayerMask; // 障害物のレイヤーマスク

    private Transform player; // プレイヤーのTransform
    private float nextAttackTime = 0f; // 次の攻撃可能時間
    private float nextTeleportTime = 0f; // 次のテレポート可能時間
    private Animator animator;
    private DealDamage dealDamage;
    private SpriteRenderer spriteRenderer;
    private bool isTeleporting = false;
    private bool isPaused = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator.Play("WindBossWalk");
        dealDamage = GetComponent<DealDamage>();
    }

    void Update()
    {
        if (dealDamage.isDead || isTeleporting || isPaused) return; // 死亡しているかテレポート中か攻撃後の停止中は行動停止
        MoveAwayFromPlayer();
        AttackPlayer();
    }

    void MoveAwayFromPlayer()
    {
        if (player != null)
        {
            Vector3 direction = transform.position - player.position;
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer < attackRange * 32f)
            {
                if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
                {
                    direction = new Vector3(Mathf.Sign(direction.x), 0, 0);
                    if (direction.x > 0)
                    {
                        animator.SetTrigger("WalkRight");
                    }
                    else
                    {
                        animator.SetTrigger("WalkLeft");
                    }
                }
                else
                {
                    direction = new Vector3(0, Mathf.Sign(direction.y), 0);
                    if (direction.y > 0)
                    {
                        animator.SetTrigger("WalkUp");
                    }
                    else
                    {
                        animator.SetTrigger("WalkDown");
                    }
                }

                if (!IsObstacleInDirection(direction))
                {
                    transform.position += direction * moveSpeed * Time.deltaTime;
                }
            }
        }
    }

    bool IsObstacleInDirection(Vector3 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 1f, obstacleLayerMask);
        if (hit.collider != null && Time.time >= nextTeleportTime)
        {
            StartCoroutine(Teleport());
            nextTeleportTime = Time.time + teleportCooldown;
            return true;
        }
        return false;
    }

    void AttackPlayer()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer <= attackRange * 32f && Time.time >= nextAttackTime)
            {
                Vector3 direction = (player.position - transform.position).normalized;

                Vector3[] directions = new Vector3[]
                {
                    direction,
                    Quaternion.Euler(0, 0, 15) * direction,
                    Quaternion.Euler(0, 0, -15) * direction,
                    Quaternion.Euler(0, 0, 30) * direction,
                    Quaternion.Euler(0, 0, -30) * direction
                };

                foreach (var dir in directions)
                {
                    GameObject windAttack = Instantiate(windAttackPrefab, transform.position, Quaternion.identity);
                    windAttack.GetComponent<Rigidbody2D>().velocity = dir * attackSpeed;

                    WindAttack windAttackScript = windAttack.GetComponent<WindAttack>();
                    if (windAttackScript != null)
                    {
                        windAttackScript.damage = attackDamage;
                    }
                }

                nextAttackTime = Time.time + attackCooldown;
                StartCoroutine(PauseAfterAttack());
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & obstacleLayerMask) != 0 && Time.time >= nextTeleportTime)
        {
            StartCoroutine(Teleport());
            nextTeleportTime = Time.time + teleportCooldown;
        }
    }

    IEnumerator Teleport()
    {
        isTeleporting = true;

        // テレポート前にフェードアウト効果
        yield return StartCoroutine(FadeOut());

        Vector3 newPosition = GetRandomPosition();
        while (IsPositionBlocked(newPosition))
        {
            newPosition = GetRandomPosition();
        }

        transform.position = newPosition;

        // テレポート後にフェードイン効果
        yield return StartCoroutine(FadeIn());

        isTeleporting = false;
    }

    Vector3 GetRandomPosition()
    {
        float x = Random.Range(-10f, 10f);
        float y = Random.Range(-10f, 10f);
        return new Vector3(x, y, 0);
    }

    bool IsPositionBlocked(Vector3 position)
    {
        Collider2D hit = Physics2D.OverlapCircle(position, 1f, obstacleLayerMask | LayerMask.GetMask("Player"));
        return hit != null;
    }

    IEnumerator PauseAfterAttack()
    {
        isPaused = true;
        yield return new WaitForSeconds(4.0f); // 攻撃後4秒間移動を停止
        isPaused = false;
    }

    IEnumerator FadeOut()
    {
        float fadeDuration = 0.5f;
        Color color = spriteRenderer.color;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            color.a = Mathf.Lerp(1, 0, t / fadeDuration);
            spriteRenderer.color = color;
            yield return null;
        }

        color.a = 0;
        spriteRenderer.color = color;
    }

    IEnumerator FadeIn()
    {
        float fadeDuration = 0.5f;
        Color color = spriteRenderer.color;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            color.a = Mathf.Lerp(0, 1, t / fadeDuration);
            spriteRenderer.color = color;
            yield return null;
        }

        color.a = 1;
        spriteRenderer.color = color;
    }

    public void TakeDamage(float damage)
    {
        dealDamage.Damage(damage);
    }
}
