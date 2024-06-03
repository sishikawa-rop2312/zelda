using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    public int attackPower = 1;

    // 移動中かどうか
    bool isMoving = false;
    // 1マス当たりの移動速度
    public float moveSpeed = 0.2f;
    // 移動先
    Vector3 targetDirection;

    // 向き
    public Vector3 currentDirection = Vector3.down;
    // ひとつ前の向き
    public Vector3 previousDirection = Vector3.zero;

    SpriteRenderer spriteRenderer;

    Animator animator;

    // 当たり判定用のレイヤーを取得
    public LayerMask detectionMask;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        // スプライト処理用にコンポーネントを取得
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        SetSptite();

        // 動ける状態だったら
        if (!isMoving)
        {
            if (Input.GetKey("up"))
            {
                currentDirection = Vector3.up;
            }
            else if (Input.GetKey("down"))
            {
                currentDirection = Vector3.down;
            }
            else if (Input.GetKey("left"))
            {
                currentDirection = Vector3.left;
            }
            else if (Input.GetKey("right"))
            {
                currentDirection = Vector3.right;
            }

            // もし方向転換してたらアニメーションを変更
            if (currentDirection != previousDirection && currentDirection != Vector3.zero)
            {
                previousDirection = currentDirection;
                int hoge = 0;
                Debug.Log("方向転換を検知" + hoge);
                MoveDirection(currentDirection);
            }

            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                Debug.Log("Attack呼び出し");
                StartCoroutine(Attack());
            }
        }


    }

    public Sprite up;
    public Sprite down;
    public Sprite left;
    public Sprite right;

    void SetSptite()
    {
        if (currentDirection == Vector3.up)
        {
            spriteRenderer.sprite = up;
        }
        else if (currentDirection == Vector3.down)
        {
            spriteRenderer.sprite = down;
        }
        else if (currentDirection == Vector3.left)
        {
            spriteRenderer.sprite = left;
        }
        else if (currentDirection == Vector3.right)
        {
            spriteRenderer.sprite = right;
        }
    }

    void SetSpriteAnimaton(Vector3 direction)
    {
        string walkAnimationTrigger = "";

        // 進行方向に応じてアニメーションを変更
        if (direction == Vector3.up)
        {
            walkAnimationTrigger = "WalkUp";
        }
        else if (direction == Vector3.down)
        {
            walkAnimationTrigger = "WalkDown";
        }
        else if (direction == Vector3.left)
        {
            walkAnimationTrigger = "WalkLeft";
        }
        else if (direction == Vector3.right)
        {
            walkAnimationTrigger = "WalkRight";
        }

        animator.SetTrigger(walkAnimationTrigger);
    }

    void ResetAnimation()
    {
    }

    void MoveDirection(Vector3 direction)
    {
        targetDirection = direction;

        // 進行方向に障害物がないかチェック
        if (Physics2D.Raycast(transform.position, targetDirection, 1f, detectionMask).collider != null)
        {
            Debug.Log(targetDirection + "に障害物検知");
            isMoving = false;
        }
        else // なければMove呼び出し
        {
            StartCoroutine(Move());
        }
    }

    public IEnumerator Move()
    {
        // 移動中かどうか
        isMoving = true;
        // 現在地
        Vector3 nowPosition = transform.position;
        // 目的地
        Vector3 targetPosition = nowPosition + targetDirection;


        // 移動にかかった時間
        float elapsedTime = 0f;

        // 移動が完了するまでLerpでマス間の位置を補完
        while (elapsedTime < moveSpeed)
        {
            transform.position = Vector3.Lerp(nowPosition, targetPosition, (elapsedTime / moveSpeed));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 移動時間分経過したら目的地へ強制的に移動（処理落ち保険）
        transform.position = targetPosition;
        // 移動中フラグを戻す
        isMoving = false;

        // アニメーションの速度を一時的に保存
        float savedAnimationSpeed = animator.speed;
        // アニメーションの速度を0に設定
        animator.speed = 0f;

        // 移動が入力され続けてれば引き続き移動する
        if (Input.GetKey("up"))
        {
            MoveDirection(Vector3.up);
            currentDirection = Vector3.up;
        }
        else if (Input.GetKey("down"))
        {
            MoveDirection(Vector3.down);
            currentDirection = Vector3.down;
        }
        else if (Input.GetKey("left"))
        {
            MoveDirection(Vector3.left);
            currentDirection = Vector3.left;
        }
        else if (Input.GetKey("right"))
        {
            MoveDirection(Vector3.right);
            currentDirection = Vector3.right;
        }
    }

    public ParticleSystem attackEffect;

    public IEnumerator Attack()
    {
        Debug.Log("Attackが呼び出されました");
        Collider2D hit = Physics2D.Raycast(transform.position, currentDirection, 1f, detectionMask).collider;
        if (hit != null)
        {
            if (hit.CompareTag("Enemy"))
            {
                // 行動中かどうか
                isMoving = true;

                Debug.Log(hit.gameObject.name + "に攻撃");

                // 相手の被ダメージメソッドを取得
                DealDamage dealDamage = hit.GetComponent<DealDamage>();

                if (dealDamage != null)
                {
                    dealDamage.Damage(attackPower);
                }

                // エフェクトを再生
                ParticleSystem attackParticle = Instantiate(attackEffect, transform.position + currentDirection, Quaternion.identity);
                attackParticle.Play();

                // エフェクトが再生し終わるまで待機する
                yield return new WaitForSeconds(attackParticle.main.duration / 2);

                // 行動中フラグを戻す
                isMoving = false;

                // エフェクトオブジェクトを削除
                Destroy(attackParticle.gameObject);
            }
            yield return null;
        }
        else
        {
            Debug.Log("攻撃対象が見つかりませんでした");
        }
    }

}
