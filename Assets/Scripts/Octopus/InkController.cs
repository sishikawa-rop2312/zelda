using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InkController : MonoBehaviour
{
    // 矢の攻撃力
    public int attackPower = 1;
    // 矢が飛んでいくスピード
    public float speed = 3f;
    // 矢の向き
    public Vector3 direction = Vector3.down;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // 指定された方向に移動する
        Vector3 velocity = direction * speed * Time.deltaTime;
        transform.Translate(velocity, Space.Self);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("OnTriggerEnterが呼び出された");
        if (other.gameObject.CompareTag("Player"))
        {
            DealDamage enemyDealDamage = other.gameObject.GetComponent<DealDamage>();
            enemyDealDamage.Damage(attackPower);
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
