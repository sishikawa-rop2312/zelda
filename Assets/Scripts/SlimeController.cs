// using System.Collections;
// using System.Collections.Generic;
// using Unity.VisualScripting;
// using UnityEngine;
// using UnityEngine.Events;

// public class Collider2DEvent : UnityEvent<Collider2D> { }
// public class SlimeController : MonoBehaviour
// {
//     //HP
//     public int enemyHp = 1;
//     // 移動速度
//     public float moveSpeed = 0.2f;

//     // プレイヤーのTransform
//     private Transform player;
//     //敵を索敵
//     private bool seach = false;

//     Animator animator;
//     private DealDamage dealDamage;
//     private slimeSeach slimeSeach;

//     //索敵範囲外の時にコルーチンを呼び出しているか
//     bool runActive = false;
//     //索敵コルーチンが動いているか
//     bool seachrun = false;

//     //索敵範囲外の時の敵の進行間隔
//     public float interval = 1.1f;

//     public LayerMask obstacleMask;


//     void Start()
//     {
//         player = GameObject.FindGameObjectWithTag("Player").transform;
//         this.animator = GetComponent<Animator>();
//         dealDamage = GetComponent<DealDamage>();

//         LayerMask obstacleMask = LayerMask.GetMask("Obstacle");
//         collisionEventDispacher2D._OnColliderEvent.AddListener(OnColliderEventTriggered);


//     }


//     void Update()
//     {

//         if (seach && seachrun == false)
//         {
//             StartCoroutine(Chase());


//         }
//         else if (runActive == false)
//         {
//             StartCoroutine(RandomRun());
//         }



//     }

//     IEnumerator RandomRun()
//     {
//         while (!seach)
//         {
//             runActive = true;

//             Run();
//             yield return new WaitForSeconds(interval);
//             Debug.Log("停止中");

//         }

//     }



//     //プレイヤーが索敵範囲にいない時
//     void Run()
//     {
//         //ランダムに動くようにする
//         int randomRun = Random.Range(0, 4);
//         switch (randomRun)
//         {
//             case 0:
//                 // 上
//                 animator.SetTrigger("Up");
//                 transform.position += new Vector3(0, moveSpeed, 0);

//                 Debug.Log("上");
//                 break;
//             case 1:
//                 // 下
//                 animator.SetTrigger("Down");
//                 transform.position += new Vector3(0, -moveSpeed, 0);
//                 Debug.Log("下");

//                 break;
//             case 2:
//                 // 右
//                 animator.SetTrigger("Right");
//                 transform.position += new Vector3(moveSpeed, 0, 0);
//                 Debug.Log("右");

//                 break;
//             case 3:
//                 // 左
//                 animator.SetTrigger("Left");
//                 transform.position += new Vector3(-moveSpeed, 0, 0);
//                 Debug.Log(" 左");

//                 break;
//         }
//     }


//     IEnumerator Chase()
//     {
//         while (seach)
//         {
//             seachrun = true;
//             MoveTowardsPlayer();
//             yield return new WaitForSeconds(interval);


//         }
//     }


//     void MoveTowardsPlayer()
//     {




//         // プレイヤーの方向を向く（正面のみで実装）
//         Vector3 direction = player.position - transform.position;

//         RaycastHit2D hitX = Physics2D.Raycast(transform.position, Vector2.right, 1f, detectionMask);
//         RaycastHit2D hitY = Physics2D.Raycast(transform.position, Vector2.up, 1f, detectionMask);

//         if (hitX.collider == null || hitY.collider == null)
//         {
//             Debug.Log("障害物がない！");
//             if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
//             {
//                 // 横方向の距離が縦方向より大きい場合、横方向に移動
//                 direction = new Vector3(Mathf.Sign(direction.x), 0, 0);
//                 if (direction.x > 0)
//                 {
//                     animator.SetTrigger("Right");
//                     transform.position += new Vector3(moveSpeed, 0, 0);
//                 }
//                 else
//                 {
//                     animator.SetTrigger("Left");
//                     transform.position += new Vector3(-moveSpeed, 0, 0);
//                 }
//             }
//             else
//             {
//                 // 縦方向の距離が横方向より大きい場合、縦方向に移動
//                 direction = new Vector3(0, Mathf.Sign(direction.y), 0);
//                 if (direction.y > 0)
//                 {
//                     animator.SetTrigger("Up");
//                     transform.position += new Vector3(0, moveSpeed, 0);
//                 }
//                 else
//                 {
//                     animator.SetTrigger("Down");
//                     transform.position += new Vector3(0, -moveSpeed, 0);
//                 }
//             }

//         }
//         else if (hitX.collider == null)
//         {
//             Debug.Log("左右どちらかに障害物がある");
//             int randomnum = Random.Range(0, 2);
//             if (randomnum == 0)
//             {
//                 animator.SetTrigger("Up");
//                 transform.position += new Vector3(0, moveSpeed, 0);
//             }
//             else
//             {
//                 animator.SetTrigger("Down");
//                 transform.position += new Vector3(0, -moveSpeed, 0);
//             }

//         }
//         else
//         {
//             Debug.Log("上下どちらかに障害物がある");
//             int randomnum = Random.Range(0, 2);
//             if (randomnum == 0)
//             {
//                 animator.SetTrigger("Right");
//                 transform.position += new Vector3(moveSpeed, 0, 0);
//             }
//             else
//             {
//                 animator.SetTrigger("Left");
//                 transform.position += new Vector3(-moveSpeed, 0, 0);
//             }
//         }


//     }


//     public void TakeDamage(int damage)
//     {
//         dealDamage.Damage(damage);
//     }


//     IEnumerator AttackPlayer()
//     {
//         Debug.Log("スライムはプレイヤーに攻撃した！");
//         yield return new WaitForSeconds(10);

//     }
//     bool attackActive = false;

//     private void OnTriggerEnter2D(Collider2D collision)
//     {

//         if (collision.CompareTag("Player") && !attackActive)
//         {
//             StartCoroutine(AttackPlayer());
//         }

//     }
// }