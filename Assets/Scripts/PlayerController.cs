using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    // 移動中かどうか
    bool isMoving = false;
    // 1マス当たりの移動速度
    public float moveSpeed = 0.2f;
    // 向き
    Vector3 targetDirection;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // 動ける状態だったら
        if (!isMoving)
        {
            if (Input.GetKey("up"))
            {
                // 向きを決める
                targetDirection = Vector3.up;
                // 一マスずつ歩かせたいのでコルーチンで移動と速度を監視
                StartCoroutine(Move());
            }
            else if (Input.GetKey("down"))
            {
                targetDirection = Vector3.down;
                StartCoroutine(Move());
            }
            else if (Input.GetKey("left"))
            {
                targetDirection = Vector3.left;
                StartCoroutine(Move());
            }
            else if (Input.GetKey("right"))
            {
                targetDirection = Vector3.right;
                StartCoroutine(Move());
            }
        }
        transform.Translate(Input.GetAxis("Horizontal") * 5.0f * Time.deltaTime, 0, 0);
        transform.Translate(0, Input.GetAxis("Vertical") * 5.0f * Time.deltaTime, 0);
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

        // 移動が入力され続けてれば引き続き移動する
        if (Input.GetKey("up"))
        {
            targetDirection = Vector3.up;
            StartCoroutine(Move());
        }
        else if (Input.GetKey("down"))
        {
            targetDirection = Vector3.down;
            StartCoroutine(Move());
        }
        else if (Input.GetKey("left"))
        {
            targetDirection = Vector3.left;
            StartCoroutine(Move());
        }
        else if (Input.GetKey("right"))
        {
            targetDirection = Vector3.right;
            StartCoroutine(Move());
        }
    }
}
