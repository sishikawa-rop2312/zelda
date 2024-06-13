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
    public float pauseAfterAttackDuration = 4.0f; // 攻撃後の移動停止時間
    public LayerMask obstacleLayerMask; // 障害物のレイヤーマスク

    private Transform player; // プレイヤーのTransform
    private float nextAttackTime = 0f; // 次の攻撃可能時間
    private Animator animator;
    private DealDamage dealDamage;
    private SpriteRenderer spriteRenderer;
    private bool isPaused = false;
    private Camera mainCamera;//メインカメラ

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator.Play("WindBossWalk");
        dealDamage = GetComponent<DealDamage>();
        mainCamera = Camera.main;//メインカメラの取得
    }

    void Update()
    {
        // 死亡している場合、またはカメラに映っていなければ行動を停止
        if (dealDamage.isDead || !IsVisible()) return;
        MoveAwayFromPlayer();
        AttackPlayer();
    }

    bool IsVisible()
    {
        Vector3 screenPoint = mainCamera.WorldToViewportPoint(transform.position);
        return screenPoint.z > 0 && screenPoint.x >= 0 && screenPoint.x <= 1 && screenPoint.y >= 0 && screenPoint.y <= 1;
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

    IEnumerator PauseAfterAttack()
    {
        isPaused = true;
        yield return new WaitForSeconds(pauseAfterAttackDuration); // インスペクターで設定可能な移動停止時間
        isPaused = false;
    }

    public void TakeDamage(float damage)
    {
        dealDamage.Damage(damage);
    }
}
