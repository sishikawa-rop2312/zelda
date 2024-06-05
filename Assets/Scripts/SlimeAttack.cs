using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{

    public int attackDamage = 1;

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


        AttackPlayer();



    }

    public void AttackPlayer()
    {
        Debug.Log("スライムはプレイヤーに攻撃した！");

    }
}
