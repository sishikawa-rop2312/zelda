using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealDamage : MonoBehaviour
{
    public int hp = 1;
    public float defense = 1f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Damage(int damage)
    {
        Debug.Log(this.gameObject.name + "のHPが減った");
        if (hp <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (this.gameObject.name == "Player")
        {
            Debug.Log("ゲームオーバー！");
        }
        else
        {
            Debug.Log(this.gameObject.name + "は倒れた");
        }
    }
}
