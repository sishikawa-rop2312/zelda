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
    public float fadeDuration = 1.0f; // フェードアウトの時間

    public AudioClip sound;
    private AudioSource audioSource;

    void Start()
    {
        animator = GetComponent<Animator>();
        playerController = FindObjectOfType<PlayerController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
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
        RaycastHit2D hit = Physics2D.Raycast(playerController.transform.position, playerController.currentDirection, 1f, detectionMask);
        if (hit.collider != null && hit.collider.gameObject == gameObject)
        {
            return true;
        }
        return false;
    }

    // 宝箱を破棄してアイテムを出現させる
    IEnumerator FadeOutAndSpawnItem()
    {
        Debug.Log("宝箱あけました");
        audioSource.PlayOneShot(sound);
        // フェードアウト
        Color color = spriteRenderer.color;
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            color.a = Mathf.Lerp(1, 0, t / fadeDuration);
            spriteRenderer.color = color;
            yield return null;
        }
        color.a = 0;
        spriteRenderer.color = color;

        // アイテムを出現させる
        Instantiate(itemPrefab, transform.position, Quaternion.identity);

        // 完全に破棄する場合
        Destroy(gameObject);
    }
}
