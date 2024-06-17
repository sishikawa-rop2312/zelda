using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleDirector : MonoBehaviour
{
    // 移動先のシーン名
    public string targetScene;

    //音源
    public AudioClip sound;
    AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }


    public void OnButtonClicked()
    { //音源がある場合
        if (sound != null)
        {

            Debug.Log("おとをさいせい");
            //音を鳴らす
            audioSource.PlayOneShot(sound);
            // 音の再生終了を待ってからシーンを変更する
            Debug.Log("おとをさいせい中");
            StartCoroutine(WaitForSoundAndChangeScene());
            Debug.Log("移動完了");
        }
        else
        {
            SceneManager.LoadScene(targetScene);
        }


    }

    IEnumerator WaitForSoundAndChangeScene()
    {
        // 音源が再生終了するまで待機
        while (audioSource.isPlaying)
        {
            yield return null; // 次のフレームまで待機
        }
        // シーン変更を行う
        Debug.Log("おと終了");
        SceneManager.LoadScene(targetScene);
    }
}
