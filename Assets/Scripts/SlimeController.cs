using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeController : MonoBehaviour
{
    //HP
    public int enemyHp = 1;
    // 移動速度
    public float moveSpeed = 1f;
    // 攻撃範囲（マス数）
    public float attackRange = 1f;
    // プレイヤーのTransform
    private Transform player;
    //敵を索敵
    private bool seach = false;
    //攻撃
    // private bool isPlayerInAttackRange = false;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

    }


    void Update()
    {

        if (seach)
        {
            MoveTowardsPlayer();
        }



    }

    void MoveTowardsPlayer()
    {

        // プレイヤーの方向を向く（正面のみで実装）
        Vector3 direction = player.position - transform.position;
        direction.Normalize();
        transform.position += direction * moveSpeed * Time.deltaTime;

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("勇者見つけた");
        seach = true;

    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("いなくなった");
            seach = false;
        }

    }

}
