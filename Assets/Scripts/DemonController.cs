using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonController : MonoBehaviour
{
    public float moveSpeed = 1f; // 移動速度
    public float attackRange = 15f; // 攻撃範囲（マス数）
    public GameObject fireballPrefab; // ファイアボールのプレハブ
    public float fireballSpeed = 1f; // ファイアボールの速度
    public float attackCooldown = 5f; // 攻撃のクールダウンタイム（5秒）
    public int fireballDamage = 1; // ファイアボールのダメージ量
    public int meleeDamage = 2; // 近接攻撃のダメージ量
    public float meleeAttackCooldown = 2f; // 近接攻撃のクールダウンタイム（2秒）

    private Transform player; // プレイヤーのTransform
    private float nextAttackTime = 0f; // 次の攻撃可能時間
    private float nextMeleeAttackTime = 0f; // 次の近接攻撃可能時間
    private Animator animator;
    private DealDamage dealDamage;
    private SpriteRenderer spriteRenderer; // スプライトレンダラー

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform; // プレイヤーを探す
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>(); // スプライトレンダラーを取得
        animator.Play("DemonWalk");
        dealDamage = GetComponent<DealDamage>();
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

            // 隣接している場合、近接攻撃
            if (distanceToPlayer <= 32f && Time.time >= nextMeleeAttackTime) // 1タイル=32pxとして計算
            {
                MeleeAttack();
                nextMeleeAttackTime = Time.time + meleeAttackCooldown; // 次の近接攻撃可能時間を設定
            }
            // それ以外の場合、ファイアボールで攻撃
            else if (distanceToPlayer <= attackRange * 32f && Time.time >= nextAttackTime)
            {
                FireballAttack();
                // 次の攻撃可能時間を設定
                nextAttackTime = Time.time + attackCooldown;
            }
        }
    }

    void MeleeAttack()
    {
        // 近接攻撃の処理
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, 1f); // 半径1で隣接チェック
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                Debug.Log("デーモンの近接攻撃がプレイヤーに当たった！");
                DealDamage playerDamage = hitCollider.GetComponent<DealDamage>();
                if (playerDamage != null)
                {
                    playerDamage.Damage(meleeDamage);
                }
            }
        }
    }

    void FireballAttack()
    {
        // ファイアボールを扇形に3方向発射
        Vector3[] directions = new Vector3[]
        {
            (player.position - transform.position).normalized,
            Quaternion.Euler(0, 0, 15) * (player.position - transform.position).normalized,
            Quaternion.Euler(0, 0, -15) * (player.position - transform.position).normalized
        };

        foreach (var direction in directions)
        {
            GameObject fireball = Instantiate(fireballPrefab, transform.position, Quaternion.identity);
            fireball.GetComponent<Rigidbody2D>().velocity = direction * fireballSpeed;

            // ファイアボールのダメージ量を設定
            Fireball fireballScript = fireball.GetComponent<Fireball>();
            if (fireballScript != null)
            {
                fireballScript.damage = fireballDamage;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("敵の攻撃が当たった！");
        }
    }

    public void TakeDamage(int damage)
    {
        dealDamage.Damage(damage);
        if (dealDamage.hp <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("デーモンが倒された！");
        StartCoroutine(FadeOutAndDestroy()); // フェードアウトしてから消滅
    }

    IEnumerator FadeOutAndDestroy()
    {
        float fadeDuration = 3f; // フェードアウトにかかる時間
        float fadeSpeed = 1f / fadeDuration;
        Color color = spriteRenderer.color;

        Debug.Log("フェードアウト開始");

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            color.a = Mathf.Lerp(1, 0, t / fadeDuration);
            spriteRenderer.color = color;
            Debug.Log($"フェードアウト中: t={t}, alpha={color.a}");
            yield return null;
        }

        color.a = 0;
        spriteRenderer.color = color;
        Debug.Log("フェードアウト完了");
        Destroy(gameObject); // 完全にフェードアウトしたらオブジェクトを消滅させる
    }
}
