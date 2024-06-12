using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treasurebox : MonoBehaviour
{
    Animator animator;
    SpriteRenderer spriteRenderer;
    bool isOpen = false;
    PlayerController playerController;

    public LayerMask detectionMask;
    public GameObject itemPrefab; // 出現させるアイテムのプレハブ
    public float fadeDuration = 1f; // フェードアウトの時間

    void Start()
    {
        animator = GetComponent<Animator>();
        playerController = FindObjectOfType<PlayerController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isOpen && IsPlayerOpenTreasurebox())
        {
            OpenTreasurebox();
        }
    }

    void OpenTreasurebox()
    {
        animator.SetTrigger("OpenTrigger");
        isOpen = true;
        StartCoroutine(FadeOutAndSpawnItem());
    }

    // プレイヤーの向いてる1マス前に宝箱存在するか
    bool IsPlayerOpenTreasurebox()
    {
        Collider2D hit = Physics2D.Raycast(playerController.transform.position, playerController.currentDirection, 1f, detectionMask).collider;
        if (hit != null && hit.CompareTag("Treasurebox"))
        {
            return true;
        }
        return false;
    }

    IEnumerator FadeOutAndSpawnItem()
    {
        float startAlpha = spriteRenderer.color.a;
        float rate = 1.0f / fadeDuration;
        float progress = 0.0f;

        while (progress < 1.0f)
        {
            Color tmpColor = spriteRenderer.color;
            tmpColor.a = Mathf.Lerp(startAlpha, 0, progress);
            spriteRenderer.color = tmpColor;
            progress += rate * Time.deltaTime;

            yield return null;
        }

        // 最終的に完全に透明にする
        Color finalColor = spriteRenderer.color;
        finalColor.a = 0;
        spriteRenderer.color = finalColor;

        // アイテムを出現させる
        Instantiate(itemPrefab, transform.position, Quaternion.identity);

        // 宝箱オブジェクトを非アクティブにするか破棄する
        gameObject.SetActive(false);
        // またはDestroy(gameObject); // 完全に破棄する場合
    }
}
