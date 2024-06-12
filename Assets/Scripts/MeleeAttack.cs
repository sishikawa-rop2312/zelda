using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    public int damage = 2; // 近接攻撃のダメージ量
    public float lifetime = 0.5f; // 攻撃エフェクトが存在する時間

    void Start()
    {
        // 一定時間後に攻撃エフェクトを破壊
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // プレイヤーに当たったときの処理
        if (other.CompareTag("Player"))
        {
            DealDamage playerDamage = other.GetComponent<DealDamage>();
            if (playerDamage != null)
            {
                playerDamage.Damage(damage);
                Debug.Log("プレイヤーに" + damage + "のダメージを与えた！");
            }
        }
    }
}
