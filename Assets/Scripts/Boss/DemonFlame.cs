using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonFlame : MonoBehaviour
{
    public float lifetime = 3f;  // DemonFlameが存在する時間
    public LayerMask obstacleLayer;  // 障害物レイヤー
    public float damage = 1;  // DemonFlameのダメージ量

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // プレイヤーに当たったときの処理
        if (collision.collider.CompareTag("Player"))
        {
            DealDamage dealDamage = collision.collider.GetComponent<DealDamage>();
            if (dealDamage != null)
            {
                dealDamage.Damage(damage);
            }
            Destroy(gameObject);
        }
        // 障害物に当たったときの処理
        else if (((1 << collision.gameObject.layer) & obstacleLayer) != 0)
        {
            Destroy(gameObject);
        }
    }
}
