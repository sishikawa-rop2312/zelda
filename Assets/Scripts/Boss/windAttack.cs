using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class windAttack : MonoBehaviour
{
    public float lifetime = 3f;
    public LayerMask obstacleLayer;
    public int damage = 1;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            DealDamage dealDamage = other.GetComponent<DealDamage>();
            if (dealDamage != null)
            {
                dealDamage.Damage(damage);
            }
            Destroy(gameObject);
        }
        else if (((1 << other.gameObject.layer) & obstacleLayer) != 0)
        {
            Destroy(gameObject);
        }
    }
}
