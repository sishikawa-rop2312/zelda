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

    Animator animator;

    bool RunActive = false;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        this.animator = GetComponent<Animator>();

    }


    void Update()
    {

        if (seach)
        {
            MoveTowardsPlayer();
        }
        else if (RunActive == false)
        {
            StartCoroutine(RandomRun());
        }



    }

    IEnumerator RandomRun()
    {
        while (!seach)
        {
            RunActive = true;

            Run();
            yield return new WaitForSeconds(5);
            Debug.Log("止まってるよ");

        }

    }



    //プレイヤーが索敵範囲にいない時
    void Run()
    {

        //これから記載

        // this.animator.speed =

        //ランダムに動くようにする
        int randomRun = Random.Range(0, 4);
        switch (randomRun)
        {
            case 0:
                // 上
                Debug.Log("上");
                break;
            case 1:
                // 下
                Debug.Log("下");
                break;
            case 2:
                // 右
                Debug.Log("右");
                break;
            case 3:
                // 左
                Debug.Log(" 左");
                break;
        }




    }


    void MoveTowardsPlayer()
    {

        // プレイヤーの方向を向く（正面のみで実装）
        Vector3 direction = player.position - transform.position;
        direction.Normalize();
        transform.position += direction * moveSpeed * Time.deltaTime;

    }

    //プレイヤーが索敵範囲に入ったとき
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("プレイヤー見つけた");
        seach = true;
        RunActive = false;

    }

    //プレイヤーが索敵範囲に抜けた時
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("プレイヤーいなくなった");
            seach = false;
        }

    }

}
