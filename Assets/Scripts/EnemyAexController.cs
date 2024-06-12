using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAexController : MonoBehaviour
{
    // 矢の攻撃力
    public int attackPower = 2;
    // 矢が飛んでいくスピード
    public float speed = 3f;
    // 矢の向き
    public Vector3 direction = Vector3.down;
    // 回転するスピード
    public float rotationSpeed = 720f;

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

        transform.GetChild(0).Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.gameObject.name + "に当たった");

        if (other.gameObject.CompareTag("Player"))
        {
            DealDamage enemyDealDamage = other.gameObject.GetComponent<DealDamage>();
            enemyDealDamage.Damage(attackPower);
            Destroy(gameObject);
        }
        else if (!other.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
