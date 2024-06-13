using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PrologueDirector : MonoBehaviour
{
    public TextMeshProUGUI scrollingText; // スクロールするテキスト
    public TextMeshProUGUI pressSpaceText; // "press Space Key"のテキスト
    public float scrollSpeed = 80f; // スクロール速度
    bool isScrolling = true;
    Camera mainCamera;
    public string targetScene;  // 移動先のシーン名

    void Start()
    {
        pressSpaceText.gameObject.SetActive(false); // 初期状態では非表示
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
                pressSpaceText.gameObject.SetActive(true);
            }
        }

        // if (pressSpaceText.gameObject.activeSelf && Input.GetKeyDown(KeyCode.Space))
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Spaceキーが押された時の処理
            Debug.Log("Space Key Pressed");
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
