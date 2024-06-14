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

        // ダメージイベントをフック
        dealDamage.OnDamageTaken += HandleDamageTaken;
    }

    void OnDestroy()
    {
        // ダメージイベントのフックを解除
        dealDamage.OnDamageTaken -= HandleDamageTaken;
    }

    void Update()
    {
        // 死亡している場合、またはテレポート中は行動を停止
        if (dealDamage.isDead || isTeleporting) return;
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

    void HandleDamageTaken(float damage)
    {
        if (!dealDamage.isDead)
        {
            StartCoroutine(Teleport());
        }
    }

    IEnumerator Teleport()
    {
        isTeleporting = true;

        Vector3 newPosition = GetRandomPosition();
        while (IsPositionBlocked(newPosition))
        {
            newPosition = GetRandomPosition();
        }

        transform.position = newPosition;

        isTeleporting = false;
        yield return null;
    }

    Vector3 GetRandomPosition()
    {
        Vector3 newPosition = Vector3.zero;
        bool validPosition = false;

        while (!validPosition)
        {
            // テレポート可能範囲内のランダムな位置を選択
            float x = Random.Range(-6.5f, 6.5f);
            float y = Random.Range(-12.5f, -8.5f);
            newPosition = new Vector3(x, y, 0);

            // 左上、右上、右下の制限
            if ((x == -6.5f && (y >= -12.5f && y <= -8.5f)) ||
                (x == 3.5f && y == -8.5f) ||
                (x == 6.5f && (y == -9.5f || y == -12.5f)))
            {
                continue;
            }

            // 障害物やプレイヤー、敵と重ならない位置かをチェック
            Collider2D[] colliders = Physics2D.OverlapCircleAll(newPosition, 1f);
            validPosition = true;
            foreach (var collider in colliders)
            {
                if (collider.CompareTag("Obstacle") || collider.CompareTag("Player") || collider.CompareTag("Enemy"))
                {
                    validPosition = false;
                    break;
                }
            }
        }

        return newPosition;
    }

    bool IsPositionBlocked(Vector3 position)
    {
        Collider2D hit = Physics2D.OverlapCircle(position, 1f, obstacleLayerMask | LayerMask.GetMask("Player") | LayerMask.GetMask("Enemy"));
        return hit != null;
    }
}
