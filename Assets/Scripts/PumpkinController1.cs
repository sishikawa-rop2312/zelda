using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // 基本攻撃力
    public int attackPower = 1;
    // 1マス当たりの移動速度
    public float moveSpeed = 0.2f;
    // 索敵範囲
    public float searchRange = 10f;
    // 攻撃時クールタイム
    public int attackCoolTime = 3;
    // 障害物レイヤー
    public LayerMask detectionMask;
    // 敵レイヤー
    public LayerMask PlayerMask;
    // 攻撃エフェクト
    public ParticleSystem attackEffect;

    //Animator animator;
    SpriteRenderer spriteRenderer;
    Vector3 currentDirection = Vector3.zero;
    bool isMoving = false;
    public Transform playerTransform;
    bool isPlayerNearby = false;

    void Start()
    {
        //animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        SetSprite();

        // プレイヤーとの距離を計測
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        // プレイヤーが索敵範囲内にいるかチェック
        isPlayerNearby = distanceToPlayer <= searchRange;

        //　距離が1マス以内(隣マス)なら攻撃
        if (distanceToPlayer <= 1f && !isMoving)
        {
            //animator.SetTrigger("WalkStop");
            StartCoroutine(Attack());
        }
        // 索敵範囲内だが2マス以上離れている場合は追跡
        else if (isPlayerNearby && !isMoving)
        {
            // プレイヤーに向かって移動
            Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
            MoveDirection(directionToPlayer, "Walk");
        }
        // 索敵範囲内に見つからなければランダム
        else if (!isMoving)
        {
            // ランダムな方向に移動
            Vector3 randomDirection = Random.insideUnitCircle.normalized;
            MoveDirection(randomDirection, "Walk");

            isMoving = false;
        }
        // 万が一、何にも当てはまらなければ何もしない
        else
        {
            Debug.Log(gameObject.name + "は何もすることがありません(" + isMoving + ", " + isPlayerNearby + ")");
        }


    }

    void SetSprite()
    {
        if (currentDirection == Vector3.up)
        {
            // 上向きのスプライトを設定
        }
        else if (currentDirection == Vector3.down)
        {
            // 下向きのスプライトを設定
        }
        else if (currentDirection == Vector3.left)
        {
            // 左向きのスプライトを設定
        }
        else if (currentDirection == Vector3.right)
        {
            // 右向きのスプライトを設定
        }
    }

    void MoveDirection(Vector3 direction, string animation)
    {
        // 斜め移動を制限し、進行方向を変える
        if (direction.x != 0 && direction.y != 0)
        {
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                direction.y = 0;

                if (direction.x > 0)
                {
                    direction = Vector3.right;
                }
                else
                {
                    direction = Vector3.left;
                }
            }
            else
            {
                direction.x = 0;

                if (direction.y > 0)
                {
                    direction = Vector3.up;
                }
                else
                {
                    direction = Vector3.down;
                }
            }
        }

        // 現在の向きを設定
        currentDirection = direction;

        // アニメーション再生
        //animator.SetBool(animation, true);

        // 進行方向に障害物がないかチェック
        if (Physics2D.OverlapPoint(transform.position + currentDirection, detectionMask) != null || Physics2D.OverlapPoint(transform.position + currentDirection, PlayerMask) != null)
        {
            isMoving = false;
        }
        else // なければMove呼び出し
        {
            StartCoroutine(Move(animation));
        }
    }

    IEnumerator Move(string animation)
    {
        // 行動中フラグを立てる
        isMoving = true;
        // 現在地を取得
        Vector3 nowPosition = transform.position;
        // 目的地を取得
        Vector3 targetPosition = nowPosition + currentDirection;
        float elapsedTime = 0f;

        // 移動速度に合わせて移動
        while (elapsedTime < moveSpeed)
        {
            transform.position = Vector3.Lerp(nowPosition, targetPosition, (elapsedTime / moveSpeed));
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // 万が一、移動しすぎor間に合わないときは強制移動
        transform.position = targetPosition;

        // 移動中フラグオフ
        isMoving = false;

        //animator.SetBool(animation, false);
    }

    IEnumerator Attack()
    {
        // 敵の位置を取得
        Vector3 direction = (playerTransform.position - transform.position).normalized;

        // 敵の方を向く
        if (direction.x != 0 || direction.y != 0)
        {
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                direction.y = 0;

                if (direction.x > 0)
                {
                    direction = Vector3.right;
                }
                else
                {
                    direction = Vector3.left;
                }
            }
            else
            {
                direction.x = 0;

                if (direction.y > 0)
                {
                    direction = Vector3.up;
                }
                else
                {
                    direction = Vector3.down;
                }
            }
        }

        // 現在の向きを設定
        currentDirection = direction;

        // 目的地に敵が居るか判定
        Collider2D hit = Physics2D.OverlapPoint(transform.position + currentDirection, PlayerMask);

        if (hit != null) // 何か居たら
        {
            if (hit.CompareTag("Player")) // プレイヤーならば
            {
                // 行動中かどうか
                isMoving = true;

                // 相手の被ダメージメソッドを取得
                DealDamage dealDamage = hit.GetComponent<DealDamage>();

                // 相手がダメージメソッドを持っていれば呼び出し
                if (dealDamage != null)
                {
                    dealDamage.Damage(attackPower);
                }

                // エフェクトを再生
                ParticleSystem attackParticle = Instantiate(attackEffect, transform.position + currentDirection, Quaternion.identity);
                attackParticle.Play();

                // クールタイム終了まで待機する
                yield return new WaitForSeconds(attackCoolTime);

                // 行動中フラグを戻す
                isMoving = false;

                // エフェクトオブジェクトを削除
                Destroy(attackParticle.gameObject);
            }
        }
        else
        {
            Debug.Log("攻撃対象が見つかりませんでした");
        }
    }

}
