using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    public Image heartPrefab;
    public Image halfHeartPrefab;
    public Image noHeartPrefab;
    List<Image> hearts = new List<Image>();

    public void SetHealth(float currentHP, float maxHP)
    {
        // 必要なハートの数を計算
        int maxHearts = Mathf.CeilToInt(maxHP);

        // 既存のハートを全て削除して再生成
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        hearts.Clear();

        // ハートの数を調整して再生成
        for (int i = 0; i < maxHearts; i++)
        {
            Image newHeart;
            if (i < currentHP)
            {
                if (i + 0.5f == currentHP)
                {
                    newHeart = Instantiate(halfHeartPrefab, transform);
                }
                else
                {
                    newHeart = Instantiate(heartPrefab, transform);
                }
            }
            else
            {
                newHeart = Instantiate(noHeartPrefab, transform);
            }
            hearts.Add(newHeart);
        }
    }
}
