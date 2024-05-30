using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobEnemyController : MonoBehaviour
{
    public float moveDistance = 32f;  // 移動距離
    public float moveInterval = 2f;  // 移動間隔（非敵対時）
    public float detectRange = 5f;  // 敵対範囲
    public float attackRange = 1f;  // 攻撃範囲
    public float moveSpeed = 2f;  // 移動速度
    public Transform player;  // プレイヤーのTransform
    public GameObject fireballPrefab;  // 火の玉のプレハブ

    private Vector2 targetPosition;  // ターゲット位置
    private bool isHostile = false;  // 敵対状態かどうか
    private float moveTimer;  // 移動タイマー
    private float attackTimer;  // 攻撃タイマー
    private Animator animator;  // Animatorコンポーネント

    void Start()
    {
        // 初期のタゲポジは現在位置
        targetPosition = transform.position;
        // タイマーを初期化
        moveTimer = moveInterval;
        attackTimer = 0f;
        // Animatorコンポーネントを取得
        animator = GetComponent<Animator>();
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
            ChasePlayer();

            if (distanceToPlayer <= attackRange)
            {
                // 攻撃時間をカウントダウン
                attackTimer -= Time.deltaTime;
                if (attackTimer <= 0f)
                {
                    // 攻撃を実行
                    AttackPlayer();
                    // 攻撃時間をリセット
                    attackTimer = 5f;  // 次の攻撃までの待機時間（仮設定で５秒）
                }
            }
        }
        else
        {
            // 非敵対時のランダム移動
            RandomMovement();
        }

        // Animatorパラメータ更新
        animator.SetBool("isWalking", isHostile || (Vector2.Distance(transform.position, targetPosition) > 0.1f));
        animator.SetBool("isAttacking", isHostile && distanceToPlayer <= attackRange);
    }

    void ChasePlayer()
    {
        // プレイヤーの方向を計算
        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        // プレイヤーの方向に32ピクセルで移動設定
        targetPosition = (Vector2)transform.position + directionToPlayer * moveDistance;
        // ターゲット位置に移動
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    void AttackPlayer()
    {
        // 火の玉を生成し、プレイヤーの方向に発射
        GameObject fireball = Instantiate(fireballPrefab, transform.position, Quaternion.identity);
        Rigidbody2D rb = fireball.GetComponent<Rigidbody2D>();
        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        rb.velocity = directionToPlayer * moveSpeed;  // (仮）火の玉の速度を設定

        // 攻撃のデバッグログ
        Debug.Log("敵の攻撃！");
    }

    void RandomMovement()
    {
        // タイマーが0になったら再度索敵設定
        moveTimer -= Time.deltaTime;
        if (moveTimer <= 0f)
        {
            // ランダムな方向を向く
            Vector2 direction = Vector2.zero;
            int randomDir = Random.Range(0, 4);
            switch (randomDir)
            {
                case 0:
                    direction = Vector2.up;
                    break;
                case 1:
                    direction = Vector2.down;
                    break;
                case 2:
                    direction = Vector2.left;
                    break;
                case 3:
                    direction = Vector2.right;
                    break;
            }
            // 新しくターゲット位置計算
            targetPosition = (Vector2)transform.position + direction * moveDistance;
            // タイマーリセット
            moveTimer = moveInterval;
        }

        // ターゲット位置に移動
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
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

