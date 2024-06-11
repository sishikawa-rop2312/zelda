using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarknutController : MonoBehaviour
{
    // 基本攻撃力
    public float attackPower = 1.5f;
    // 1マス当たりの移動速度
    public float moveSpeed = 0.2f;
    // 索敵範囲
    public float searchRange = 10f;
    // 攻撃時クールタイム
    public float attackCoolTime = 3;
    // 障害物レイヤー
    public LayerMask detectionMask;
    // 敵レイヤー
    public LayerMask PlayerMask;
    // 攻撃エフェクト
    public ParticleSystem attackEffect;

    // プレイヤーとの距離
    float distanceToPlayer;

    // プレイヤーが索敵範囲内にいるか
    bool isPlayerNearby = false;

    Animator animator;
    SpriteRenderer spriteRenderer;
    DealDamage dealDamage;
    Vector3 currentDirection = Vector3.zero;
    bool isMoving = false;
    public Transform playerTransform;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        dealDamage = GetComponent<DealDamage>();
        playerTransform = GameObject.Find("Player").transform;
    }

    void Update()
    {
        SetSprite();

        if (dealDamage.isDead)
        {
            // 何も行動しない
        }
        else
        {
            // moveSpeedに合わせて索敵する
            StartCoroutine(SearchPlayer(0.2f));

            Debug.Log("isPlayerNearby:" + isPlayerNearby + ",isMoving:" + isMoving);

            // プレイヤーが自分の前方にいるならば、ダメージを無効にする
            if (playerTransform.position == transform.position + currentDirection)
            {
                dealDamage.defense = 0;
            }
            else
            {
                dealDamage.defense = 1;
            }

            //　距離が1マス以内(隣マス)なら攻撃
            if (distanceToPlayer <= 1.5f && !isMoving)
            {
                ResetAnimation();
                Debug.Log(gameObject.name + "は攻撃メソッドを呼び出します(" + distanceToPlayer + ")");
                StartCoroutine(Attack());
            }
            // 索敵範囲内だが2マス以上離れている場合は追跡
            else if (isPlayerNearby && !isMoving)
            {
                // プレイヤーに向かって移動
                Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
                Debug.Log(gameObject.name + "はプレイヤーに向かって移動します");
                MoveDirection(directionToPlayer);
            }
            // 索敵範囲内に見つからなければランダム
            else if (!isMoving)
            {
                // ランダムな方向に移動
                Vector3 randomDirection = Random.insideUnitCircle.normalized;
                Debug.Log(gameObject.name + "はランダムに移動します");
                MoveDirection(randomDirection);
            }
            // 万が一、何にも当てはまらなければ何もしない
            else
            {
                Debug.Log(gameObject.name + "は何もすることがありません(" + isMoving + ", " + isPlayerNearby + ")");
            }
        }

    }

    IEnumerator SearchPlayer(float interval)
    {
        // プレイヤーとの距離を計測
        distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        // プレイヤーが索敵範囲内にいるかチェック
        isPlayerNearby = distanceToPlayer <= searchRange;
        yield return new WaitForSeconds(interval);
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

    void WalkAnimation(string animation)
    {
        if (animation == "WalkDown")
        {
            if (!animator.GetBool("WalkDown"))
            {
                animator.SetBool("WalkDown", true);
            }
            animator.SetBool("WalkUp", false);
            animator.SetBool("WalkLeft", false);
            animator.SetBool("WalkRight", false);
        }
        if (animation == "WalkUp")
        {
            if (!animator.GetBool("WalkUp"))
            {
                animator.SetBool("WalkUp", true);
            }
            animator.SetBool("WalkDown", false);
            animator.SetBool("WalkLeft", false);
            animator.SetBool("WalkRight", false);
        }
        if (animation == "WalkLeft")
        {
            if (!animator.GetBool("WalkLeft"))
            {
                animator.SetBool("WalkLeft", true);
            }
            animator.SetBool("WalkDown", false);
            animator.SetBool("WalkUp", false);
            animator.SetBool("WalkRight", false);
        }
        if (animation == "WalkRight")
        {
            if (!animator.GetBool("WalkRight"))
            {
                animator.SetBool("WalkRight", true);
            }
            animator.SetBool("WalkDown", false);
            animator.SetBool("WalkUp", false);
            animator.SetBool("WalkLeft", false);
        }
    }

    void MoveDirection(Vector3 direction)
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
        string animation;
        if (currentDirection == Vector3.up) { animation = "WalkUp"; }
        else if (currentDirection == Vector3.left) { animation = "WalkLeft"; }
        else if (currentDirection == Vector3.right) { animation = "WalkRight"; }
        else { animation = "WalkDown"; }
        WalkAnimation(animation);

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
                    Debug.Log("dealDamage呼び出し");
                }

                // エフェクトを再生
                ParticleSystem attackParticle = Instantiate(attackEffect, transform.position + (currentDirection * 2 / 3), Quaternion.identity);
                Debug.Log("エフェクト再生");
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

    void ResetAnimation()
    {
        animator.SetBool("WalkDown", false);
        animator.SetBool("WalkUp", false);
        animator.SetBool("WalkLeft", false);
        animator.SetBool("WalkRight", false);
    }

}
