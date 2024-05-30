using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobEnemyController : MonoBehaviour
{
    public float moveDistance = 32f;  // 移動距離
    public float detectRange = 5f;  // 敵対範囲
    public float attackRange = 1f;  // 攻撃範囲
    public float moveSpeed = 2f;  // 移動速度
    public Transform player;  // プレイヤーのTransform
    public GameObject fireballPrefab;  // 火の玉のプレハブ

    private bool isHostile = false;  // 敵対状態かどうか
    private float attackTimer;  // 攻撃タイマー
    private Animator animator;  // Animatorコンポーネント
    private bool isMoving;  // 移動中かどうか

    void Start()
    {
        // 攻撃タイマーを初期化
        attackTimer = 0f;
        // Animatorコンポーネントを取得
        animator = GetComponent<Animator>();
        // 移動中フラグを初期化
        isMoving = false;
    }

    void Update()
    {
        // プレイヤーとの距離を計算
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectRange)
        {
            // 敵対状態に遷移
            isHostile = true;
        }
        else
        {
            // 非敵対状態に遷移
            isHostile = false;
        }

        if (isHostile)
        {
            // プレイヤーを追跡
            if (!isMoving)
            {
                StartCoroutine(ChasePlayer());
            }

            if (distanceToPlayer <= attackRange)
            {
                // 攻撃時間をカウントダウン
                attackTimer -= Time.deltaTime;
                if (attackTimer <= 0f)
                {
                    // 攻撃を実行
                    AttackPlayer();

                    attackTimer = 5f;  // 次の攻撃までの待機時間（仮設定で５秒）
                }
            }
        }

        // アニメーションパラメータの更新
        UpdateAnimationParameters();
    }

    IEnumerator ChasePlayer()
    {
        isMoving = true;

        // プレイヤーの方向を計算
        Vector2 directionToPlayer = (player.position - transform.position).normalized;

        // 移動先の位置を計算
        Vector2 targetPosition = (Vector2)transform.position + directionToPlayer * moveDistance;

        // 0.2秒かけて移動
        float elapsedTime = 0f;
        Vector2 startPosition = transform.position;

        while (elapsedTime < 0.2f)
        {
            transform.position = Vector2.Lerp(startPosition, targetPosition, elapsedTime / 0.2f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        isMoving = false;
    }

    void AttackPlayer()
    {
        // 火の玉を生成し、プレイヤーの方向に発射
        GameObject fireball = Instantiate(fireballPrefab, transform.position, Quaternion.identity);
        Rigidbody2D rb = fireball.GetComponent<Rigidbody2D>();
        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        rb.velocity = directionToPlayer * moveSpeed;  // 火の玉の速度を設定

        // 攻撃のデバッグログ
        Debug.Log("敵の攻撃！");
    }

    void UpdateAnimationParameters()
    {
        Vector2 moveDirection = (player.position - transform.position).normalized;
        if (Mathf.Abs(moveDirection.x) > Mathf.Abs(moveDirection.y))
        {
            if (moveDirection.x > 0)
            {
                animator.Play("EnemyWalk_Right");
            }
            else
            {
                animator.Play("EnemyWalk_Left");
            }
        }
        else
        {
            if (moveDirection.y > 0)
            {
                animator.Play("EnemyWalk_Up");
            }
            else
            {
                animator.Play("EnemyWalk_Down");
            }
        }
    }

    //デバッグ用索敵可視化
    void OnDrawGizmosSelected()
    {
        // 敵対範囲のギズモを描画
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        // 攻撃範囲のギズモを描画
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
