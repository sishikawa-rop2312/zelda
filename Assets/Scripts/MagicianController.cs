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
    public int enemyHp = 1; // 敵の体力

    private Transform player; // プレイヤーのTransform
    private float nextAttackTime = 0f; // 次の攻撃可能時間
    private Animator animator;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // プレイヤーを探す
        animator = GetComponent<Animator>();
        animator.Play("MagicianWalk");
    }

    void Update()
    {
        MoveTowardsPlayer();
        AttackPlayer();
    }

    void MoveTowardsPlayer()
    {
        if (player != null)
        {
            // プレイヤーの方向を向く（正面のみで実装）
            Vector3 direction = (player.position - transform.position).normalized;
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

                Debug.Log("敵のファイアボール攻撃！");

                // 次の攻撃可能時間を設定
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
        enemyHp -= damage;
        Debug.Log("敵の体力: " + enemyHp);

        if (enemyHp <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("魔術師が倒された！");
        Destroy(gameObject); // 敵キャラクターを消滅させる
    }

}
