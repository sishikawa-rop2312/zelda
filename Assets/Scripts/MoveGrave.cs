using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveGrave : MonoBehaviour
{
    //bottunを設定
    public GameObject gimmickButton;



    void Start()
    {
        // //ボタンのコンポーネント取得
        // GimmickButtonController buttonController = gimmickButton.GetComponent<GimmickButtonController>();
        StartCoroutine(CheckButton());

    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator CheckButton()
    {//ボタンのコンポーネント取得
        GimmickButtonController buttonController = gimmickButton.GetComponent<GimmickButtonController>();



        while (true)
        {
            yield return new WaitForSeconds(1.0f);

            if (buttonController.buttonPush)
            {
                Debug.Log("ボタンが押されました。これから墓の移動を開始します");

                //動く処理を入れる

                transform.position = new Vector3(-15.5f, -8.5f, 0);


                break;

            }
        }
    }

}
