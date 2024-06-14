using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GimmickButtonController : MonoBehaviour
{

    // プレイヤーの初期位置
    Vector3 playerInitialPosition;

    // プレイヤーオブジェクト
    GameObject player;

    //音源
    public AudioClip sound;
    AudioSource audioSource;

    //押したかおしていないか
    bool buttonPush = false;

    // //動く墓
    // public GameObject moveGrave;

    Animator animator;


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
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    { // キャラクターがトリガーゾーンに入ったかを確認
        if (other.CompareTag("Player") && !buttonPush)
        {
            Debug.Log("プレイヤーがボタンを押しました");
            buttonPush = true;
            //音を鳴らす
            audioSource.PlayOneShot(sound);
        }
    }


}
