using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    // 移動先のシーン名
    public string targetScene;

    // プレイヤーの出現位置
    public Vector3 spawnPosition;

    // トリガーに触れた際に呼び出される関数
    void OnTriggerEnter2D(Collider2D other)
    {
        // キャラクターがトリガーゾーンに入ったかを確認
        if (other.CompareTag("Player"))
        {
            // シーン変更イベントを登録
            SceneManager.sceneLoaded += OnSceneLoaded;

            Debug.Log("Loading Scene: " + targetScene);
            // シーンを変更
            SceneManager.LoadScene(targetScene);
        }
    }

    // シーンがロードされた後に呼び出される関数
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene Loaded: " + scene.name);

        // Playerオブジェクトを探して位置を更新
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            Debug.Log("Player found. Moving to position: " + spawnPosition);
            player.transform.position = spawnPosition;
        }
        else
        {
            Debug.Log("Player not found");
        }

        // イベントからこの関数を削除
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
