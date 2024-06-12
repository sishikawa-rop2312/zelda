using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarknessBossController : MonoBehaviour
{
    public float moveSpeed = 1f; // 移動速度
    public float attackRange = 15f; // 攻撃範囲（マス数）
    public GameObject darknessAttackPrefab; // 闇の玉の攻撃のプレハブ
    public float attackSpeed = 1f; // 攻撃の速度
    public float attackCooldown = 5f; // 攻撃のクールダウン（5秒）
    public float attackDamage = 0.5f; // 攻撃のダメージ量
    public LayerMask obstacleLayerMask; // 障害物のレイヤーマスク

    private Transform player; // プレイヤーのTransform
    private float nextAttackTime = 0f; // 次の攻撃可能時間
    private Animator animator;
    private DealDamage dealDamage;
    private SpriteRenderer spriteRenderer;
    private bool isTeleporting = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator.Play("DarknessBossWalk");
        dealDamage = GetComponent<DealDamage>();
    }

    void Update()
    {
        if (dealDamage.isDead || isTeleporting) return; // 死亡しているかテレポート中は行動停止
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
        return hit.collider != null;
    }

    void AttackPlayer()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer <= attackRange * 32f && Time.time >= nextAttackTime)
            {
                Vector3 direction = (player.position - transform.position).normalized;

                GameObject darknessAttack = Instantiate(darknessAttackPrefab, transform.position, Quaternion.identity);
                darknessAttack.GetComponent<Rigidbody2D>().velocity = direction * attackSpeed;

                DarknessAttack darknessAttackScript = darknessAttack.GetComponent<DarknessAttack>();
                if (darknessAttackScript != null)
                {
                    darknessAttackScript.damage = attackDamage;
                }

                nextAttackTime = Time.time + attackCooldown;
            }
        }
    }

    public void TakeDamage(float damage)
    {
        dealDamage.Damage(damage);
        if (!dealDamage.isDead)
        {
            StartCoroutine(Teleport());
        }
    }

    IEnumerator Teleport()
    {
        isTeleporting = true;

        // テレポート前にフェードアウト効果を無効にする
        Color color = spriteRenderer.color;
        color.a = 0;
        spriteRenderer.color = color;

        yield return new WaitForSeconds(0.5f); // 少し待ってからテレポート

        Vector3 newPosition = GetRandomPosition();
        while (IsPositionBlocked(newPosition))
        {
            newPosition = GetRandomPosition();
        }

        Debug.Log("テレポート先の新しい位置: " + newPosition);
        transform.position = newPosition;

        // テレポート後にフェードイン効果を無効にする
        color.a = 1;
        spriteRenderer.color = color;

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
        Collider2D hit = Physics2D.OverlapCircle(position, 1f, obstacleLayerMask);
        return hit != null;
    }
}
