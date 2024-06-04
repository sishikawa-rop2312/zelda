using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    public Image heartPrefab;
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
            Image newHeart = Instantiate(heartPrefab, transform);
            hearts.Add(newHeart);
        }

        // ハートの表示を更新
        for (int i = 0; i < hearts.Count; i++)
        {
            if (i < currentHP)
            {
                if (i + 0.5f == currentHP)
                {
                    hearts[i].fillAmount = 0.5f;
                }
                else
                {
                    hearts[i].fillAmount = 1f;
                }
            }
            else
            {
                hearts[i].fillAmount = 0f;
            }
        }
    }
}
