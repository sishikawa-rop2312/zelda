using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{

    public int attackDamage = 1;

    // 向き
    public Vector3 currentDirection = Vector3.zero;

    // 当たり判定用のレイヤーを取得
    public LayerMask detectionMask;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Player"))
        {
            AttackPlayer();

        }




    }

    public void AttackPlayer()
    {
        Debug.Log("スライムはプレイヤーに攻撃した！");

    }
}
