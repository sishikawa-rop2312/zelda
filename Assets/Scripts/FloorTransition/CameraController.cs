using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float transitionTime = 1.0f; // カメラの移動にかける時間
    public float cameraMoveYDistance = 10.0f; // カメラがY軸に移動する距離
    public float cameraMoveXDistance = 18.0f; // カメラがX軸に移動する距離
    DealDamage playerDealDamage;
    PlayerController playerController; // プレイヤーコントローラーへの参照
    Camera mainCamera; // メインカメラへの参照

    void Start()
    {
        // メインカメラを取得
        mainCamera = Camera.main;
        // プレイヤーの行動を制限するためにdealDamageを取得
        playerDealDamage = GameObject.Find("Player").GetComponent<DealDamage>();
        // シーン内のプレイヤーコントローラーを取得
        playerController = FindObjectOfType<PlayerController>();
    }

    void Update()
    {
        // playerControllerがnullであれば再取得を試みる
        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>();
        }

        // playerControllerが存在する場合のみ処理を行う
        if (playerController != null)
        {
            // プレイヤーがカメラの外に出た場合にカメラを移動
            if (IsPlayerOutOfCameraBounds())
            {
                if (playerDealDamage != null)
                {
                    // プレイヤーの移動を制限
                    playerDealDamage.isDead = true;
                }
                // 新しいカメラの位置を計算
                Vector3 newCameraPosition = GetNewCameraPosition();
                // カメラをスムーズに移動
                StartCoroutine(SmoothMoveCamera(newCameraPosition));
            }
        }
    }

    // プレイヤーがカメラのビューポートの外に出たかどうかをチェック
    bool IsPlayerOutOfCameraBounds()
    {
        // プレイヤーのワールド座標をビューポート座標に変換
        Vector3 playerViewportPosition = mainCamera.WorldToViewportPoint(playerController.transform.position);
        // ビューポートの範囲外にいるかどうかを返す
        return playerViewportPosition.x < 0 || playerViewportPosition.x > 1 || playerViewportPosition.y < 0 || playerViewportPosition.y > 1;
    }

    // プレイヤーの移動方向に応じて新しいカメラ位置を計算
    Vector3 GetNewCameraPosition()
    {
        Vector3 moveDirection = playerController.currentDirection; // プレイヤーの移動方向を取得
        Vector3 newCameraPosition = mainCamera.transform.position; // 現在のカメラ位置を基準に新しい位置を計算

        // プレイヤーの移動方向に応じてカメラの新しい位置を設定
        if (moveDirection == Vector3.right) // 右移動
        {
            newCameraPosition.x += cameraMoveXDistance;
        }
        else if (moveDirection == Vector3.left) // 左移動
        {
            newCameraPosition.x -= cameraMoveXDistance;
        }
        else if (moveDirection == Vector3.up) // 上移動
        {
            newCameraPosition.y += cameraMoveYDistance;
        }
        else if (moveDirection == Vector3.down) // 下移動
        {
            newCameraPosition.y -= cameraMoveYDistance;
        }

        // y座標を10の倍数に調整
        newCameraPosition.y = Mathf.Round(newCameraPosition.y / 10) * 10;

        return newCameraPosition;
    }

    // 指定された時間内でカメラをスムーズに移動
    IEnumerator SmoothMoveCamera(Vector3 newCameraPosition)
    {
        float elapsedTime = 0f;
        Vector3 startingPosition = transform.position;

        while (elapsedTime < transitionTime)
        {
            transform.position = Vector3.Lerp(startingPosition, newCameraPosition, elapsedTime / transitionTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 最終的に正確な新しい位置にカメラを設定
        transform.position = newCameraPosition;

        // 移動し終わったらプレイヤーの移動制限を解除
        if (playerDealDamage != null)
        {
            playerDealDamage.isDead = false;
        }
    }
}