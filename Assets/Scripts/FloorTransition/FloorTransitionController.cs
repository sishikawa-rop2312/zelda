using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorTransitionController : MonoBehaviour
{
    public Vector3 newCameraPosition; // カメラの新しい位置
    public Vector3 newPlayerPosition; // プレイヤーの新しい位置
    CameraController cameraController;
    Collider2D triggerCollider;

    void Start()
    {
        cameraController = Camera.main.GetComponent<CameraController>();
        triggerCollider = GetComponent<Collider2D>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(Transition(other));
        }
    }

    IEnumerator Transition(Collider2D player)
    {
        // トリガーを無効化
        triggerCollider.enabled = false;

        // カメラを移動
        cameraController.MoveCamera(newCameraPosition);

        // カメラ移動が終わるのを待つ
        yield return new WaitForSeconds(cameraController.transitionTime);

        // プレイヤーを新しい位置に移動
        player.transform.position = newPlayerPosition;

        // プレイヤーがトリガーの範囲外に出るまで待つ
        while (triggerCollider.bounds.Contains(player.transform.position))
        {
            yield return null;
        }

        // トリガーを再度有効化
        triggerCollider.enabled = true;
    }
}
