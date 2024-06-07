using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New HealthPotion", menuName = "Items/HealthPotion")]
public class HealthPotion : Item
{
    public override void Use(PlayerController player)
    {
        player.GetComponent<ItemManager>().AddHealthPotion();
    }
}