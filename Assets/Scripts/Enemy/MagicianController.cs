using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicianController : MonoBehaviour
{
    public float moveSpeed = 1f; // 移動速度
    public float attackRange = 15f; // 攻撃範囲（マス数）
    public GameObject fireballPrefab; // ファイアボールのプレハブ
    public float fireballSpeed = 1f; // ファイアボールの速度
    public float attackCooldown = 5f; // 攻撃のクールダウンタイム（5秒）
    public int fireballDamage = 1; // ファイアボールのダメージ量

    private Transform player; // プレイヤーのTransform
    private float nextAttackTime = 0f; // 次の攻撃可能時間
    private Animator animator;
    private DealDamage dealDamage;
    private Camera mainCamera;//メインカメラ

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // プレイヤーを探す
        animator = GetComponent<Animator>();
        animator.Play("MagicianWalk");
        dealDamage = GetComponent<DealDamage>();
        mainCamera = Camera.main;//メインカメラの取得
    }

    void Update()
    {
        // 死亡している場合、またはカメラに映っていなければ行動を停止
        if (dealDamage.isDead || IsVisible()) return;
        MoveTowardsPlayer();
        AttackPlayer();
    }

    bool IsVisible()
    {
        Vector3 screenPoint = mainCamera.WorldToViewportPoint(transform.position);
        return screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
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

    void AttackPlayer()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer <= attackRange * 32f && Time.time >= nextAttackTime) // 1タイル=32pxとして計算
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
        Debug.Log("魔術師が倒された！");
        Destroy(gameObject); // 敵キャラクターを消滅させる
    }
}
