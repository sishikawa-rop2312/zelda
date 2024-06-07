using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New MaxHealthUp", menuName = "Items/MaxHealthUp")]
public class MaxHealthUp : Item
{
    public override void Use(PlayerController player)
    {
        player.GetComponent<ItemManager>().IncreaseMaxHealth();
    }
}
