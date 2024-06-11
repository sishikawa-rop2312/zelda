using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthPotionDisplay : MonoBehaviour
{
    public Image healthPotionIcon;
    public TextMeshProUGUI healthPotionCountText;

    public void UpdateHealthPotionCount(int count)
    {
        healthPotionCountText.text = "x" + count.ToString();
    }
}
