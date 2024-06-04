using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public int attackPower = 1;
    public float moveSpeed = 0.2f;
    public LayerMask detectionMask;
    public ParticleSystem attackEffect;
    public Collider2D targetCollider;

    //Animator animator;
    SpriteRenderer spriteRenderer;
    Vector3 currentDirection = Vector3.zero;
    bool isMoving = false;
    Transform playerTransform;
    bool isPlayerNearby = false;

    void Awake()
    {
        //animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        SetSprite();

        // プレイヤーが近くにいるかどうかをチェック
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        isPlayerNearby = distanceToPlayer <= 10f;

        if (distanceToPlayer <= 1 && !isMoving)
        {
            //animator.SetTrigger("WalkStop");
            Debug.Log("Attack呼び出し");
            StartCoroutine(Attack());
        }
        else if (isPlayerNearby && !isMoving)
        {
            // プレイヤーに向かって移動
            Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
            MoveDirection(directionToPlayer, "Walk");
        }
        else if (!isMoving)
        {
            // ランダムな方向に移動
            Vector3 randomDirection = Random.insideUnitCircle.normalized;
            MoveDirection(randomDirection, "Walk");
        }
        else
        {
            Debug.Log(gameObject.name + "は何もすることがありません");
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
        // 斜め移動を制限する
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

        currentDirection = direction;

        // アニメーション再生
        //animator.SetBool(animation, true);

        // 進行方向に障害物がないかチェック
        if (Physics2D.OverlapPoint(transform.position + currentDirection, detectionMask) != null || Physics2D.OverlapPoint(transform.position + currentDirection, 7) != null)
        {
            Debug.Log(currentDirection + "に障害物検知:" + currentDirection);
            isMoving = false;
        }
        else // なければMove呼び出し
        {
            StartCoroutine(Move(animation));
        }
    }

    IEnumerator Move(string animation)
    {
        isMoving = true;
        Vector3 nowPosition = transform.position;
        Vector3 targetPosition = nowPosition + currentDirection;

        float elapsedTime = 0f;

        while (elapsedTime < moveSpeed)
        {
            transform.position = Vector3.Lerp(nowPosition, targetPosition, (elapsedTime / moveSpeed));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        isMoving = false;

        //animator.SetBool(animation, false);
    }

    IEnumerator Attack()
    {
        Vector3 direction = (playerTransform.position - transform.position).normalized;

        // 敵の方を向く
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

        currentDirection = direction;

        Collider2D hit = Physics2D.OverlapPoint(transform.position + currentDirection, 7);
        if (hit != null && hit.CompareTag("Player"))
        {
            isMoving = true;

            DealDamage dealDamage = hit.GetComponent<DealDamage>();

            if (dealDamage != null)
            {
                dealDamage.Damage(attackPower);
            }

            ParticleSystem attackParticle = Instantiate(attackEffect, transform.position + currentDirection, Quaternion.identity);
            attackParticle.Play();

            yield return new WaitForSeconds(attackParticle.main.duration / 2);

            isMoving = false;

            Destroy(attackParticle.gameObject);
        }
        else
        {
            Debug.Log("攻撃対象が見つかりませんでした");
        }
    }
}
