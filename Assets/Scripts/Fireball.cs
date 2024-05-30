using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float lifetime = 3f;  // 火の玉が存在する時間

    void Start()
    {
        // 一定時間後に火の玉を破壊
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // プレイヤーに当たったときの処理
        if (other.CompareTag("Player"))
        {
            // プレイヤーへのダメージ未実装のためここに追加予定
            Debug.Log("火の玉がプレイヤーに当たった！");
            Destroy(gameObject);
        }
    }
}

