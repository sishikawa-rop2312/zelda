using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonController : MonoBehaviour
{
    public float moveSpeed = 1f; // 移動速度
    public float searchRange = 20f; // 索敵範囲（マス数）
    public float attackRange = 15f; // 攻撃範囲（マス数）
    public GameObject fireballPrefab; // ファイアボールのプレハブ
    public float fireballSpeed = 1f; // ファイアボールの速度
    public float attackCooldown = 5f; // 攻撃のクールダウンタイム（5秒）
    public int fireballDamage = 1; // ファイアボールのダメージ量

    private Transform player; // プレイヤーのTransform
    private float nextAttackTime = 0f; // 次の攻撃可能時間
    private Animator animator;
    private DealDamage dealDamage;
    private bool isSearchingPlayer = false; // プレイヤーを索敵中かどうか
    private float randomMoveTime = 0f; // ランダム移動の時間
    private Vector3 randomDirection; // ランダム移動の方向

    private float tileSize = 32f; // 1タイルのサイズ
    private Vector2 bossSize = new Vector2(3, 3); // ボスキャラクターのタイルサイズ

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // プレイヤーを探す
        animator = GetComponent<Animator>();
        animator.Play("DemonWalk");
        dealDamage = GetComponent<DealDamage>();
        SetRandomDirection(); // 初期のランダム移動方向を設定
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= searchRange * tileSize) // 索敵範囲内にプレイヤーがいるか
        {
            isSearchingPlayer = true;
            MoveTowardsPlayer();
            AttackPlayer();
        }
        else
        {
            isSearchingPlayer = false;
            RandomMove();
        }
    }

    void MoveTowardsPlayer()
    {
        if (player != null)
        {
            Vector3 direction = player.position - transform.position;
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                // 横方向の距離が縦方向より大きい場合、横方向に移動
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
                // 縦方向の距離が横方向より大きい場合、縦方向に移動
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

            transform.position += direction * moveSpeed * Time.deltaTime;
        }
    }

    void RandomMove()
    {
        if (Time.time >= randomMoveTime)
        {
            SetRandomDirection();
        }

        transform.position += randomDirection * moveSpeed * Time.deltaTime;

        if (randomDirection.x > 0)
        {
            animator.SetTrigger("WalkRight");
        }
        else if (randomDirection.x < 0)
        {
            animator.SetTrigger("WalkLeft");
        }
        else if (randomDirection.y > 0)
        {
            animator.SetTrigger("WalkUp");
        }
        else if (randomDirection.y < 0)
        {
            animator.SetTrigger("WalkDown");
        }
    }

    void SetRandomDirection()
    {
        int direction = Random.Range(0, 4);
        switch (direction)
        {
            case 0:
                randomDirection = Vector3.up;
                break;
            case 1:
                randomDirection = Vector3.down;
                break;
            case 2:
                randomDirection = Vector3.left;
                break;
            case 3:
                randomDirection = Vector3.right;
                break;
        }

        randomMoveTime = Time.time + Random.Range(2f, 5f); // 2秒から5秒間ランダムに動く
    }

    void AttackPlayer()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer <= attackRange * tileSize && Time.time >= nextAttackTime) // 1タイル=32pxとして計算
            {
                // プレイヤーの方向を向く
                Vector3 direction = (player.position - transform.position).normalized;

                // ファイアボールを生成し、プレイヤーに向かって発射
                GameObject fireball = Instantiate(fireballPrefab, transform.position, Quaternion.identity);
                fireball.GetComponent<Rigidbody2D>().velocity = direction * fireballSpeed;

                // ファイアボールのダメージ量を設定
                Fireball fireballScript = fireball.GetComponent<Fireball>();
                if (fireballScript != null)
                {
                    fireballScript.damage = fireballDamage;
                }
                //次の攻撃可能時間を設定
                nextAttackTime = Time.time + attackCooldown;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // プレイヤーに当たった時の処理
            Debug.Log("敵の攻撃が当たった！");
        }
    }

    public void TakeDamage(int damage)
    {
        dealDamage.Damage(damage);
    }

    void Die()
    {
        Debug.Log("デーモンが倒された！");
        StartCoroutine(FadeOutAndDestroy()); // フェードアウトしてから消滅
    }

    IEnumerator FadeOutAndDestroy()
    {
        float fadeDuration = 1f; // フェードアウトにかかる時間
        float fadeSpeed = 1f / fadeDuration;
        Color color = GetComponent<SpriteRenderer>().color;

        for (float t = 0; t < 1; t += Time.deltaTime * fadeSpeed)
        {
            color.a = Mathf.Lerp(1, 0, t);
            GetComponent<SpriteRenderer>().color = color;
            yield return null;
        }

        color.a = 0;
        GetComponent<SpriteRenderer>().color = color;
        Destroy(gameObject); // 完全にフェードアウトしたらオブジェクトを消滅させる
    }
}
