using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public int healthPotionCount = 0;
    public HealthPotionDisplay healthPotionDisplay;

    DealDamage dealDamage;

    //音源
    public AudioClip useHealthPotion;
    public AudioClip addHealthPotion;
    AudioSource audioSource;

    void Start()
    {
        dealDamage = GetComponent<DealDamage>();
        healthPotionDisplay.UpdateHealthPotionCount(healthPotionCount);
        audioSource = GetComponent<AudioSource>();
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
        //音を鳴らす
        audioSource.PlayOneShot(addHealthPotion);
    }

    public void UseHealthPotion()
    {
        if (healthPotionCount > 0)
        {
            dealDamage.Heal(1.0f);
            healthPotionCount--;
            healthPotionDisplay.UpdateHealthPotionCount(healthPotionCount);
            //音を鳴らす
            audioSource.PlayOneShot(useHealthPotion);
        }
    }

    public void IncreaseMaxHealth()
    {
        dealDamage.IncreaseMaxHealth(1.0f);
    }
}
