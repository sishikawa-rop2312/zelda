using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealSpot : MonoBehaviour
{
    // プレイヤーの初期位置
    Vector3 playerInitialPosition;

    // プレイヤーオブジェクト
    GameObject player;
    DealDamage dealDamage;

    //回復量５
    public float heal = 5.0f;

    //音源
    public AudioClip sound;
    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        // プレイヤーオブジェクトを探して初期位置を取得
        player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerInitialPosition = player.transform.position;
        }
        audioSource = GetComponent<AudioSource>();
        dealDamage = GetComponent<DealDamage>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        // キャラクターがトリガーゾーンに入ったかを確認
        if (other.CompareTag("Player"))
        {
            Debug.Log(other.gameObject.name + "がはいってきました");
            // 相手の被ダメージメソッドを取得
            DealDamage dealDamage = other.GetComponent<DealDamage>();

            if (dealDamage != null)
            {
                Debug.Log("プレイヤーの回復をします");
                dealDamage.Heal(heal);
                //音を鳴らす
                Debug.Log("音を鳴らします");
                audioSource.PlayOneShot(sound);
            }
            else
            {
                Debug.Log("ダメージメソッドがありません");
            }




        }
    }
}
