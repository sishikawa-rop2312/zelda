using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMapDirector : MonoBehaviour
{
    public GameObject DoorLeft;
    public GameObject DoorRight;
    Animator DoorLeftAnimator;
    Animator DoorRightAnimator;
    BoxCollider2D DoorLeftCollider;
    BoxCollider2D DoorRightCollider;
    PlayerController playerController;
    bool isOpen;

    //音源
    public AudioClip sound;
    AudioSource audioSource;
    //音源を一回鳴らしたか
    bool firstSound = false;

    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        DoorLeftAnimator = DoorLeft.GetComponent<Animator>();
        DoorRightAnimator = DoorRight.GetComponent<Animator>();
        DoorLeftAnimator = DoorLeft.GetComponent<Animator>();
        DoorLeftCollider = DoorLeft.GetComponent<BoxCollider2D>();
        DoorRightCollider = DoorRight.GetComponent<BoxCollider2D>();

        // 初期状態でコライダーを無効に設定
        DoorLeftCollider.enabled = false;
        DoorRightCollider.enabled = false;

        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // プレイヤーが部屋に入ったら扉を閉める
        if (playerController.transform.position.y >= 7.5f && !isOpen)
        {
            //音源がある場合
            if (sound != null && !firstSound)
            {
                firstSound = true;
                Debug.Log("おとをさいせい");
                //音を鳴らす
                audioSource.PlayOneShot(sound);


            }
            DoorLeftAnimator.SetTrigger("CloseTrigger");
            DoorRightAnimator.SetTrigger("CloseTrigger");

            // BoxCollider2Dを有効に設定
            DoorLeftCollider.enabled = true;
            DoorRightCollider.enabled = true;

            isOpen = true;
        }
    }
}
