using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonController : MonoBehaviour
{
    public float moveSpeed = 1f; // 移動速度
    public float attackRange = 15f; // 攻撃範囲（マス数）
    public GameObject demonFlamePrefab; // DemonFlameのプレハブ
    public GameObject meleeAttackPrefab; // 近接攻撃のプレハブ
    public float demonFlameSpeed = 1f; // DemonFlameの速度
    public float attackCooldown = 5f; // 攻撃のクールダウン（5秒）
    public int demonFlameDamage = 1; // DemonFlameのダメージ量
    public int meleeDamage = 2; // 近接攻撃のダメージ量
    public float meleeAttackCooldown = 2f; // 近接攻撃のクールダウンタイム（2秒）
    public LayerMask obstacleLayerMask; // 障害物のレイヤーマスク

    private Transform player; // プレイヤーのTransform
    private float nextAttackTime = 0f; // 次の攻撃可能時間
    private float nextMeleeAttackTime = 0f; // 次の近接攻撃可能時間
    private Animator animator;
    private DealDamage dealDamage;
    private SpriteRenderer spriteRenderer;
    private bool isPlayerInContact = false; // プレイヤーとの接触状態を管理するフラグ
    private Camera mainCamera; // メインカメラ

    void Start()
    {
        // プレイヤーを探してTransformを取得
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator.Play("DemonWalk");
        // ダメージ処理コンポーネントを取得
        dealDamage = GetComponent<DealDamage>();
        mainCamera = Camera.main; // メインカメラの取得
    }

    void Update()
    {
        // 死亡している場合、またはカメラに映っていなければ行動を停止
        if (dealDamage.isDead || !IsVisible()) return;
        MoveTowardsPlayer();
        AttackPlayer();
    }

    bool IsVisible()
    {
        Vector3 screenPoint = mainCamera.WorldToViewportPoint(transform.position);
        return screenPoint.z > 0 && screenPoint.x >= 0 && screenPoint.x <= 1 && screenPoint.y >= 0 && screenPoint.y <= 1;
    }

    void MoveTowardsPlayer()
    {
        if (player != null && !isPlayerInContact)
        {
            // プレイヤーの位置に向かって方向を計算
            Vector3 direction = player.position - transform.position;
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer > 1.5f) // 1.5タイル（48ピクセル）以上離れている場合のみ移動
            {
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

                // 障害物のチェック（プレイヤーも障害物として扱う）
                if (!IsObstacleInDirection(direction))
                {
                    // 障害物がない場合に移動
                    transform.position += direction * moveSpeed * Time.deltaTime;
                }
            }
        }
    }

    bool IsObstacleInDirection(Vector3 direction)
    {
        // 障害物がないかチェックする（プレイヤーも含む）
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 1f, obstacleLayerMask | LayerMask.GetMask("Player"));
        return hit.collider != null;
    }

    void AttackPlayer()
    {
        if (player != null)
        {
            // プレイヤーとの距離を計算
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            // DemonFlame攻撃が可能な距離
            // クールダウンが終了している場合に攻撃
            if (distanceToPlayer <= attackRange * 32f && Time.time >= nextAttackTime)
            {
                DemonFlameAttack();
                // 次の攻撃可能時間を設定
                nextAttackTime = Time.time + attackCooldown;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // プレイヤーと接触した場合、進行を停止し、近接攻撃のクールダウンが終了している場合、攻撃
        if (other.CompareTag("Player"))
        {
            isPlayerInContact = true;
            if (Time.time >= nextMeleeAttackTime)
            {
                MeleeAttack(other.gameObject);
                // 次の近接攻撃可能時間を設定
                nextMeleeAttackTime = Time.time + meleeAttackCooldown;
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // プレイヤーとの接触が離れた場合、進行を再開
        if (other.CompareTag("Player"))
        {
            isPlayerInContact = false;
        }
    }

    void MeleeAttack(GameObject player)
    {
        // 近接攻撃の処理
        Vector3 spawnPosition = (player.transform.position + transform.position) / 2; // プレイヤーとボスの間に生成
        GameObject meleeAttack = Instantiate(meleeAttackPrefab, spawnPosition, Quaternion.identity); // 近接攻撃エフェクトを生成
        meleeAttack.GetComponent<MeleeAttack>().damage = meleeDamage; // 近接攻撃のダメージを設定

        DealDamage playerDamage = player.GetComponent<DealDamage>();
        if (playerDamage != null)
        {
            // プレイヤーにダメージを与える
            playerDamage.Damage(meleeDamage);
        }
    }

    void DemonFlameAttack()
    {
        // DemonFlameを扇形に3方向発射
        Vector3[] directions = new Vector3[]
        {
            (player.position - transform.position).normalized,
            Quaternion.Euler(0, 0, 15) * (player.position - transform.position).normalized,
            Quaternion.Euler(0, 0, -15) * (player.position - transform.position).normalized
        };

        foreach (var direction in directions)
        {
            GameObject demonFlame = Instantiate(demonFlamePrefab, transform.position, Quaternion.identity);
            demonFlame.GetComponent<Rigidbody2D>().velocity = direction * demonFlameSpeed;

            // DemonFlameのダメージ量を設定
            DemonFlame demonFlameScript = demonFlame.GetComponent<DemonFlame>();
            if (demonFlameScript != null)
            {
                demonFlameScript.damage = demonFlameDamage;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        // ダメージを受けた際の処理
        dealDamage.Damage(damage);
    }
}
