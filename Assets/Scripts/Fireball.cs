using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float lifetime = 3f;  // 火の玉が存在する時間
    public LayerMask obstacleLayer;  // 障害物レイヤー
    public int damage = 1;  // ファイアボールのダメージ量

    void Start()
    {
        // 一定時間後に火の玉を破壊
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
