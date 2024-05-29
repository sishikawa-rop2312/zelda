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

    private Vector2 initialPosition;  // 初期位置
    private Vector2 targetPosition;  // 目標位置
    private bool isHostile = false;  // 敵対状態かどうか
    private float moveTimer;  // 移動タイマー

    private Animator animator;  // Animatorコンポーネント

    void Start()
    {
        // 初期位置を保存
        initialPosition = transform.position;
        // 初期位置は現在位置
        targetPosition = transform.position;
        // タイマー初期化(２秒)
        moveTimer = moveInterval;
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
                // 攻撃アクション実行
                AttackPlayer();
            }
        }
        else
        {
            // 非敵対時のランダム移動
            RandomMovement();
        }

        // Animatorパラメータを更新
        animator.SetBool("isWalking", isHostile || (Vector2.Distance(transform.position, targetPosition) > 0.1f));
        animator.SetBool("isAttacking", isHostile && distanceToPlayer <= attackRange);
    }

    void ChasePlayer()
    {
        // プレイヤーの方向を計算
        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        // プレイヤーの方向に32ピクセル移動する目標位置を設定
        targetPosition = (Vector2)transform.position + directionToPlayer * moveDistance;
        // 目標位置に移動
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    void AttackPlayer()
    {
        // 攻撃デバッグ用
        //Debug.Log("敵の攻撃！");
        // 攻撃ロジックをここに実装する
    }

    void RandomMovement()
    {
        // タイマーが0になったら再度標的位置を設定
        moveTimer -= Time.deltaTime;
        if (moveTimer <= 0f)
        {
            // ランダムな方向を選択
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
            // 新しい標的位置を計算
            targetPosition = (Vector2)initialPosition + direction * moveDistance;
            // タイマーリセット
            moveTimer = moveInterval;
        }

        // 目標位置に移動
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    //デバッグ用（索敵範囲表示）
    void OnDrawGizmosSelected()
    {
        // 敵対範囲を描画
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        // 攻撃範囲を描画
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
