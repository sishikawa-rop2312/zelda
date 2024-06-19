using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ClearDirector : MonoBehaviour
{
    public TextMeshProUGUI scrollingText; // スクロールするテキスト
    public GameObject creditsContent; // スタッフロールオブジェクト
    public float scrollSpeed = 80f; // スクロール速度
    public Image galenDisappearImage; // Galenが消滅するシーンの画像
    public Image peacefulSceneImage; // 平和なシーンの画像
    public float imageDisplayDuration = 3f; // 画像表示時間
    bool isScrolling = false;
    Camera mainCamera;
    public string targetScene; // 移動先のシーン名

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
        scrollingText.gameObject.SetActive(false); // 初期状態では非表示
        mainCamera = Camera.main; // メインカメラを取得

        // 初期状態では画像を非表示
        peacefulSceneImage.gameObject.SetActive(false);

        StartCoroutine(DisplaySequence());
    }

    void Update()
    {
        if (isScrolling)
        {
            scrollingText.transform.Translate(Vector3.up * scrollSpeed * Time.deltaTime);
            if (IsTextOutOfView(scrollingText))
            {
                isScrolling = false;
                scrollingText.gameObject.SetActive(false);
                StartCoroutine(DisplayPeacefulScene());
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

    IEnumerator DisplaySequence()
    {
        // 少し待ってからエンドロールテキストを表示
        yield return new WaitForSeconds(1f);
        scrollingText.gameObject.SetActive(true);
        isScrolling = true;

        // スクロールテキストが終わるまで待機
        while (isScrolling)
        {
            yield return null;
        }

        galenDisappearImage.gameObject.SetActive(false); // 背景を非表示
    }

    IEnumerator DisplayPeacefulScene()
    {
        // 平和なシーンを表示
        peacefulSceneImage.gameObject.SetActive(true);
        yield return new WaitForSeconds(imageDisplayDuration);
        peacefulSceneImage.gameObject.SetActive(false);

        // スタッフロールを表示
        creditsContent.gameObject.SetActive(true);
    }
}
