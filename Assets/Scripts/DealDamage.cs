using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class DealDamage : MonoBehaviour
{
    public float hp = 1;
    public float maxHp = 3;
    public float defense = 1f;

    // HealthDisplay（HPのGUI）
    HealthDisplay healthDisplay;

    // Start is called before the first frame update
    void Start()
    {
        // プレイヤーのHP GUI表示
        if (this.gameObject.name == "Player")
        {
            healthDisplay = FindObjectOfType<HealthDisplay>();
            if (healthDisplay != null)
            {
                healthDisplay.SetHealth(hp, maxHp);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Damage(int damage)
    {
        int actualDamage = Mathf.RoundToInt(damage * defense);
        hp -= actualDamage;
        Debug.Log(this.gameObject.name + "のHPが" + actualDamage + "減った");
        if (hp <= 0)
        {
            Die();
        }

        // 主人公のHP GUI更新
        if (healthDisplay != null && this.gameObject.name == "Player")
        {
            healthDisplay.SetHealth(hp, maxHp);
        }
    }

    void Die()
    {
        if (this.gameObject.name == "Player")
        {
            Debug.Log("ゲームオーバー！");
            SceneManager.LoadScene("Scenes/GameOverScene");
        }
        else
        {
            Destroy(this.gameObject);
            Debug.Log(this.gameObject.name + "は倒れた");
        }
    }
}
