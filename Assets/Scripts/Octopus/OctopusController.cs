using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OctopusController : MonoBehaviour
{
    // 基本攻撃力
    public int attackPower = 1;
    // 1マス当たりの移動速度
    public float moveSpeed = 0.0f;
    // 索敵範囲
    public float searchRange = 10f;
    // 攻撃時クールタイム
    public float attackCoolTime = 5;
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
    Vector3 currentDirection = Vector3.down;
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

        if (dealDamage.isDead)
        {
            // 何も行動しない
        }
        else
        {
            // 索敵する
            StartCoroutine(SearchPlayer(0.2f));

            //Debug.Log("isPlayerNearby:" + isPlayerNearby + ",isMoving:" + isMoving);

            //　距離が5マス以内かつ自分よりもyの値が小さければ攻撃
            if (distanceToPlayer <= 10f && !isMoving && transform.position.y > playerTransform.position.y)
            {
                Debug.Log(gameObject.name + "はプレイヤーを発見しました");
                ResetAnimation();
                Debug.Log(gameObject.name + "は攻撃メソッドを呼び出します(" + distanceToPlayer + ")");
                StartCoroutine(Attack());
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

    public GameObject attackObject;

    IEnumerator Attack()
    {
        // 目的地に敵が居るか判定
        RaycastHit2D hit = Physics2D.Raycast(transform.position, currentDirection, 5, PlayerMask);

        if (hit.collider != null) // 何か居たら
        {
            Debug.Log(gameObject.name + "の攻撃範囲に何か居ます");

            if (hit.collider.gameObject.CompareTag("Player")) // プレイヤーならば
            {
                // 行動中フラグオン
                isMoving = true;

                Debug.Log("墨を放つ");

                // オブジェクトを飛ばす向きを取得
                float objectRotate = 0;

                // 矢インスタンスを生成
                GameObject Arrow = Instantiate(attackObject, transform.position + currentDirection, Quaternion.Euler(0, 0, objectRotate));

                // クールタイム
                yield return new WaitForSeconds(attackCoolTime);

                // 行動中フラグを戻す
                isMoving = false;

                yield return null;
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