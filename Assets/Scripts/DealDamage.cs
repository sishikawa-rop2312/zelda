using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class DealDamage : MonoBehaviour
{
    public float hp = 1;
    public float maxHp = 3;
    public float defense = 1f;

    // 詰み防止用無敵フラグ
    bool isNoDamage = false;

    // HealthDisplay（HPのGUI）
    HealthDisplay healthDisplay;

    // SpriteRenderer
    SpriteRenderer spriteRenderer;
    // ダメージを受けた時のスタン時間
    public float stunTime = 1.0f;

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

        // ダメージ演出用のSpriteRenderを取得
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            Debug.Log(gameObject.name + "のspriteRendererを取得しました");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Damage(int damage)
    {
        if (!isNoDamage) // 無敵じゃないとき
        {
            int actualDamage = Mathf.RoundToInt(damage * defense);
            hp -= actualDamage;
            Debug.Log(this.gameObject.name + "のHPが" + actualDamage + "減った");
            if (hp <= 0)
            {
                Die();
            }

            StartCoroutine(DamageFlash());

            // 主人公のHP GUI更新
            if (healthDisplay != null && this.gameObject.name == "Player")
            {
                healthDisplay.SetHealth(hp, maxHp);
            }
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

    IEnumerator DamageFlash()
    {
        // 無敵フラグオン
        isNoDamage = true;

        // 経過時間
        float elapsedTime = 0f;

        while (elapsedTime < stunTime)
        {
            //0.2秒ごとに
            yield return new WaitForSeconds(0.1f);

            //spriteRendererをオフ
            spriteRenderer.enabled = false;

            //0.2秒待機
            yield return new WaitForSeconds(0.1f);

            //spriteRendererをオン
            spriteRenderer.enabled = true;

            // 経過時間の更新
            elapsedTime += 0.2f;

            yield return null;
        }

        // 無敵フラグオフ
        isNoDamage = false;
    }
}
