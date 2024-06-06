using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverDirector : MonoBehaviour
{
    // 移動先のシーン名
    public string targetScene;

    void Start()
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            Debug.Break();
            // プレイヤーオブジェクトを削除
            Destroy(player.gameObject);
        }
    }

    public void OnButtonClicked()
    {
        SceneManager.LoadScene(targetScene);
    }
}
