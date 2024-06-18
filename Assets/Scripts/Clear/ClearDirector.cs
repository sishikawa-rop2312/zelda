using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ClearDirector : MonoBehaviour
{
    public TextMeshProUGUI scrollingText; // スクロールするテキスト
    public GameObject creditsContent; // スタッフロールオブジェクト
    public float scrollSpeed = 80f; // スクロール速度
    bool isScrolling = true;
    Camera mainCamera;
    public string targetScene;  // 移動先のシーン名

    void Start()
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        StatusController statusBar = FindObjectOfType<StatusController>();
        HelpManager helpManager = FindObjectOfType<HelpManager>();
        if (player != null)
        {
            // プレイヤーオブジェクトを削除
            Destroy(player.gameObject);
        }

        if (statusBar != null)
        {
            // ステータスバーオブジェクトを削除
            Destroy(statusBar.gameObject);
        }

        if (helpManager != null)
        {
            // ヘルプオブジェクトを削除
            Destroy(helpManager.gameObject);
        }

        creditsContent.gameObject.SetActive(false); // 初期状態では非表示
        mainCamera = Camera.main; // メインカメラを取得
    }

    void Update()
    {
        if (isScrolling)
        {
            scrollingText.transform.Translate(Vector3.up * scrollSpeed * Time.deltaTime);
            if (IsTextOutOfView(scrollingText))
            {
                isScrolling = false;
                creditsContent.gameObject.SetActive(true);
                scrollingText.gameObject.SetActive(false);
            }
        }

        if (Input.GetKey(KeyCode.Space))
        {
            SceneManager.LoadScene(targetScene);
        }
    }

    bool IsTextOutOfView(TextMeshProUGUI text)
    {
        // テキストのRectTransformの境界を取得
        RectTransform rectTransform = text.rectTransform;
        Vector3[] worldCorners = new Vector3[4];
        rectTransform.GetWorldCorners(worldCorners);

        // 4つの角のうち、最も下にある角のY座標を取得
        float bottomY = worldCorners[0].y;

        // カメラのビューポートの上端を超えたかどうかを判定
        Vector3 viewportPoint = mainCamera.WorldToViewportPoint(new Vector3(0, bottomY, 0));
        return viewportPoint.y > 1;
    }
}
