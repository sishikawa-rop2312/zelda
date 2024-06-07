using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item item;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PickUp(other.GetComponent<PlayerController>());
        }
    }

    void PickUp(PlayerController player)
    {
        item.Use(player);
        Destroy(gameObject);
    }
}
