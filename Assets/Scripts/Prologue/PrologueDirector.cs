using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PrologueDirector : MonoBehaviour
{
    public TextMeshProUGUI scrollingText; // スクロールするテキスト
    public TextMeshProUGUI pressSpaceText; // "press Space Key"のテキスト
    public float scrollSpeed = 20f; // スクロール速度
    bool isScrolling = true;

    void Start()
    {
        pressSpaceText.gameObject.SetActive(false); // 初期状態では非表示
    }

    void Update()
    {
        if (isScrolling)
        {
            scrollingText.transform.Translate(Vector3.up * scrollSpeed * Time.deltaTime);
            if (scrollingText.rectTransform.position.y > Screen.height + scrollingText.rectTransform.rect.height / 2)
            {
                isScrolling = false;
                pressSpaceText.gameObject.SetActive(true);
            }
        }

        if (pressSpaceText.gameObject.activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            // Spaceキーが押された時の処理をここに書く
            Debug.Break();
            Debug.Log("Space Key Pressed");
        }
    }
}
