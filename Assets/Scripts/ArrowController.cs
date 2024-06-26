using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ArrowController : MonoBehaviour
{
    // 矢の攻撃力
    public int attackPower = 1;
    // 矢が飛んでいくスピード
    public float speed = 3f;
    // 矢の向き
    public Vector3 direction = Vector3.down;

    private Camera mainCamera;//メインカメラ

    // Start is called before the first frame update
    void Start()
    {
        // GameObject player = GameObject.Find("Player");
        // if (player == null) { Debug.Break(); }
        // else
        // {
        //     PlayerController playerController = player.GetComponent<PlayerController>();
        //     if (playerController == null) { Debug.Break(); }
        //     playerController.currentDirection = direction;
        //     Debug.Log(direction);
        //     Debug.Break();
        // }


        mainCamera = Camera.main;//メインカメラの取得
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsVisible())
        {
            Destroy(gameObject);
        }
        // 指定された方向に移動する
        Vector3 velocity = direction * speed * Time.deltaTime;
        transform.Translate(velocity, Space.Self);
    }

    bool IsVisible()
    {
        Vector3 screenPoint = mainCamera.WorldToViewportPoint(transform.position);
        return screenPoint.z > 0 && screenPoint.x >= 0 && screenPoint.x <= 1 && screenPoint.y >= 0 && screenPoint.y <= 1;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("OnTriggerEnterが呼び出された");
        if (other.gameObject.CompareTag("Enemy") && !other.gameObject.CompareTag("Player"))
        {
            DealDamage enemyDealDamage = other.gameObject.GetComponent<DealDamage>();
            enemyDealDamage.Damage(attackPower);
            Destroy(gameObject);
        }
        else if (!other.gameObject.CompareTag("Player"))
        {
            //Destroy(gameObject);
        }
    }


}
