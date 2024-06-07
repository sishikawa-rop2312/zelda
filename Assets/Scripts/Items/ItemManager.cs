using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public int healthPotionCount = 0;
    public HealthPotionDisplay healthPotionDisplay;

    DealDamage dealDamage;

    void Start()
    {
        dealDamage = GetComponent<DealDamage>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && healthPotionCount > 0)
        {
            UseHealthPotion();
        }
    }

    public void AddHealthPotion()
    {
        healthPotionCount++;
        healthPotionDisplay.UpdateHealthPotionCount(healthPotionCount);
    }

    public void UseHealthPotion()
    {
        if (healthPotionCount > 0)
        {
            dealDamage.Heal(1.0f);
            healthPotionCount--;
            healthPotionDisplay.UpdateHealthPotionCount(healthPotionCount);
        }
    }

    public void IncreaseMaxHealth()
    {
        dealDamage.IncreaseMaxHealth(1.0f);
    }
}
