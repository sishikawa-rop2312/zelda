using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthPotionDisplay : MonoBehaviour
{
    public Image healthPotionIcon;
    public TextMeshProUGUI healthPotionCountText;

    private void Start()
    {
        // 初期状態でアイテムを非表示
        healthPotionIcon.gameObject.SetActive(false);
        healthPotionCountText.gameObject.SetActive(false);
    }

    public void UpdateHealthPotionCount(int count)
    {
        if (count > 0)
        {
            healthPotionIcon.gameObject.SetActive(true);
            healthPotionCountText.gameObject.SetActive(true);
            healthPotionCountText.text = "x" + count.ToString();
        }
        else
        {
            healthPotionIcon.gameObject.SetActive(false);
            healthPotionCountText.gameObject.SetActive(false);
        }
    }
}
