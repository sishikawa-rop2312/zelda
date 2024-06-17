using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarknessBossController : MonoBehaviour
{
    public float moveSpeed = 1f; // 移動速度
    public float attackRange = 15f; // 攻撃範囲（マス数）
    public GameObject darknessAttackPrefab; // 闇の玉の攻撃のプレハブ
    public float attackSpeed = 1f; // 攻撃の速度
    public float attackCooldown = 5f; // 攻撃のクールダウン（5秒）
    public float attackDamage = 0.5f; // 攻撃のダメージ量
    public LayerMask obstacleLayerMask; // 障害物のレイヤーマスク
    public GameObject demonPrefab; // デーモンのプレハブ
    public AudioClip summonSound; // デーモン召喚時の音
    private AudioSource audioSource; // AudioSource

    private Transform player; // プレイヤーのTransform
    private float nextAttackTime = 0f; // 次の攻撃可能時間
    private Animator animator;
    private DealDamage dealDamage;
    private SpriteRenderer spriteRenderer;
    private bool isTeleporting = false;
    private Camera mainCamera; // メインカメラ

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator.Play("DarknessBossWalk");
        dealDamage = GetComponent<DealDamage>();
        mainCamera = Camera.main; // メインカメラの取得
        audioSource = gameObject.AddComponent<AudioSource>(); // AudioSourceを追加

        // ダメージイベントをフック
        dealDamage.OnDamageTaken += HandleDamageTaken;
        dealDamage.OnDeath += Die; // 死亡イベントをフック
    }

    void OnDestroy()
    {
        // ダメージイベントのフックを解除
        dealDamage.OnDamageTaken -= HandleDamageTaken;
        dealDamage.OnDeath -= Die; // 死亡イベントのフックを解除
    }

    void Update()
    {
        // 死亡している場合、またはカメラに映っていなければ行動を停止
        if (dealDamage.isDead || !IsVisible() || isTeleporting) return;
        MoveAwayFromPlayer();
        AttackPlayer();
    }

    bool IsVisible()
    {
        Vector3 screenPoint = mainCamera.WorldToViewportPoint(transform.position);
        return screenPoint.z > 0 && screenPoint.x >= 0 && screenPoint.x <= 1 && screenPoint.y >= 0 && screenPoint.y <= 1;
    }

    void MoveAwayFromPlayer()
    {
        if (player != null)
        {
            Vector3 direction = transform.position - player.position;
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer < attackRange * 32f)
            {
                if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
                {
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

                if (!IsObstacleInDirection(direction))
                {
                    transform.position += direction * moveSpeed * Time.deltaTime;
                }
            }
        }
    }

    bool IsObstacleInDirection(Vector3 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 1f, obstacleLayerMask);
        return hit.collider != null;
    }

    void AttackPlayer()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer <= attackRange * 32f && Time.time >= nextAttackTime)
            {
                Vector3 direction = (player.position - transform.position).normalized;

                GameObject darknessAttack = Instantiate(darknessAttackPrefab, transform.position, Quaternion.identity);
                darknessAttack.GetComponent<Rigidbody2D>().velocity = direction * attackSpeed;

                DarknessAttack darknessAttackScript = darknessAttack.GetComponent<DarknessAttack>();
                if (darknessAttackScript != null)
                {
                    darknessAttackScript.damage = attackDamage;
                }

                nextAttackTime = Time.time + attackCooldown;
            }
        }
    }

    void HandleDamageTaken(float damage)
    {
        if (!dealDamage.isDead)
        {
            StartCoroutine(Teleport());
        }
    }

    IEnumerator Teleport()
    {
        isTeleporting = true;

        Vector3 newPosition = GetRandomPosition();
        while (IsPositionBlocked(newPosition))
        {
            newPosition = GetRandomPosition();
        }

        transform.position = newPosition;

        isTeleporting = false;
        yield return null;
    }

    Vector3 GetRandomPosition()
    {
        Vector3 newPosition = Vector3.zero;
        bool validPosition = false;

        while (!validPosition)
        {
            newPosition = new Vector3(
                Mathf.Round(Random.Range(-6.5f, 6.5f)) + 0.5f,
                Mathf.Round(Random.Range(8.5f, 11.5f)) + 0.5f,
                0
            );

            // カメラの端から3マス上、1マス左右を除外する
            if (newPosition.x < -6.5f || newPosition.x > 6.5f || newPosition.y < 8.5f || newPosition.y > 11.5f)
            {
                validPosition = false;
                continue;
            }

            Collider2D[] colliders = Physics2D.OverlapCircleAll(newPosition, 1f);
            validPosition = true;
            foreach (var collider in colliders)
            {
                if (collider.CompareTag("Obstacle") || collider.CompareTag("Player") || collider.CompareTag("Enemy"))
                {
                    validPosition = false;
                    break;
                }
            }
        }

        return newPosition;
    }

    bool IsPositionBlocked(Vector3 position)
    {
        Collider2D hit = Physics2D.OverlapCircle(position, 1f, obstacleLayerMask | LayerMask.GetMask("Player") | LayerMask.GetMask("Enemy"));
        return hit != null;
    }

    void Die()
    {
        StartCoroutine(SpawnDemonAfterDelay(transform.position));
    }

    IEnumerator SpawnDemonAfterDelay(Vector3 position)
    {
        yield return StartCoroutine(FadeOut());

        if (demonPrefab != null)
        {
            GameObject demon = Instantiate(demonPrefab, position, Quaternion.identity);
            demon.SetActive(true); // デーモンをアクティブにする
            if (summonSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(summonSound); // デーモン召喚音を再生
            }
            demon.GetComponent<DemonController>().enabled = true;
        }

        Destroy(gameObject);
    }

    IEnumerator FadeOut()
    {
        float fadeDuration = 1f;
        Color color = spriteRenderer.color;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            color.a = Mathf.Lerp(1, 0, t / fadeDuration);
            spriteRenderer.color = color;
            yield return null;
        }

        color.a = 0;
        spriteRenderer.color = color;
    }
}
