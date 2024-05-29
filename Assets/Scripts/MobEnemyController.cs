using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobEnemyController : MonoBehaviour
{
    //仮設定フレーム数 & 仮設定Public
    public float moveDistance = 32f;    //移動距離
    public float moveInterval = 2f;     //移動間隔（非敵対時）
    public float detectRange = 5f;      //敵対範囲
    public float attackRange = 1f;      //攻撃範囲
    public float moveSpeed = 2f;        //移動速度
    public Transform player;            //プレイヤーのtransform(変数名仮称)


    Vector2 initialPosition;    //初期位置
    Vector2 targetPosition;     //ターゲット(プレイヤー)位置
    bool isHostile = false;     //敵対状態かどうか
    float moveTimer;            //非敵対時ランダム移動(仮)
    private Animator animator;



    void Start()
    {
        //初期位置を保存
        initialPosition = transform.position;

        //プレイヤーの位置確認
        targetPosition = transform.position;

        animator = GetComponent<Animator>();

        moveTimer = moveInterval;


    }

    void Update()
    {
        //プレイヤーとの距離を計算(仮)
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectRange)
        { isHostile = true; }//接敵状態に遷移
        else
        { isHostile = false; }//非接敵状態

        if (isHostile)
        { ChasePlayer(); }//プレイヤーを追従

        if (distanceToPlayer <= attackRange)
        { AttackPlayer(); }//攻撃実行
        else
        { RandomMovement(); }//非敵対時のランダム移動



    }

    void ChasePlayer()
    {
        //プレイヤーの方向計算
        Vector2 directionToPlayer = (player.position - transform.position);

        // プレイヤーの方向に32ピクセル移動する目標位置を設定
        Vector2 newTargetPosition = (Vector2)transform.position + directionToPlayer * moveDistance;

        //プレイヤー位置に移動
        transform.position = Vector2.MoveTowards
        (transform.position, player.position, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, newTargetPosition) < 0.1f)
        { moveTimer = moveInterval; }
    }

    void AttackPlayer()
    {
        //攻撃ロジック未実装

        //デバッグ用ログの出力
        Debug.Log("敵の攻撃");

    }

    void RandomMovement()
    {
        moveTimer -= Time.deltaTime;
        if (moveTimer <= 0f)
        {
            //ランダム方向に移動
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
            //ターゲット位置計算
            targetPosition = initialPosition + direction * moveDistance;

            //リセット
            moveTimer = moveInterval;
        }

        //ターゲット位置に移動
        transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }
}
