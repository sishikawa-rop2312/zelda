using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    public int attackPower = 1;

    // 移動中かどうか
    public bool isMoving = false;
    // 1マス当たりの移動速度
    public float moveSpeed = 0.2f;
    // 移動先
    Vector3 targetDirection;

    // 向き
    public Vector3 currentDirection = Vector3.zero;

    SpriteRenderer spriteRenderer;

    Animator animator;

    // 当たり判定用のレイヤーを取得
    public LayerMask detectionMask;

    // PlayerController型のインスタンスを保持
    public static PlayerController instance;

    void Awake()
    {
        if (instance == null)
        {
            //このインスタンスをinstanceに登録
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // 2回目以降重複して作成してしまったgameObjectを削除
            Destroy(gameObject);
        }
    }

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
                MoveDirection(Vector3.up, "WalkUp");
            }
            else if (Input.GetKey("down"))
            {
                currentDirection = Vector3.down;
                MoveDirection(Vector3.down, "WalkDown");
            }
            else if (Input.GetKey("left"))
            {
                currentDirection = Vector3.left;
                MoveDirection(Vector3.left, "WalkLeft");
            }
            else if (Input.GetKey("right"))
            {
                currentDirection = Vector3.right;
                MoveDirection(Vector3.right, "WalkRight");
            }
            else if (!Input.GetKey("up") && !Input.GetKey("down") && !Input.GetKey("left") && !Input.GetKey("right"))
            {
                ResetAnimation();
            }

            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                animator.SetTrigger("WalkStop");
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

    void ResetAnimation()
    {
        animator.SetBool("WalkDown", false);
        animator.SetBool("WalkUp", false);
        animator.SetBool("WalkLeft", false);
        animator.SetBool("WalkRight", false);
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

    void MoveDirection(Vector3 direction, string animation)
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
            StartCoroutine(Move(animation));
        }
    }

    public IEnumerator Move(string animation)
    {
        // 移動中かどうか
        isMoving = true;
        // 現在地
        Vector3 nowPosition = transform.position;
        // 目的地
        Vector3 targetPosition = nowPosition + targetDirection;

        // 移動にかかった時間
        float elapsedTime = 0f;

        // アニメーション再生
        WalkAnimation(animation);

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

        // 移動が入力され続けてれば引き続き移動する
        if (Input.GetKey("up"))
        {
            MoveDirection(Vector3.up, "WalkUp");
            currentDirection = Vector3.up;
        }
        else if (Input.GetKey("down"))
        {
            MoveDirection(Vector3.down, "WalkDown");
            currentDirection = Vector3.down;
        }
        else if (Input.GetKey("left"))
        {
            MoveDirection(Vector3.left, "WalkLeft");
            currentDirection = Vector3.left;
        }
        else if (Input.GetKey("right"))
        {
            MoveDirection(Vector3.right, "WalkRight");
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
