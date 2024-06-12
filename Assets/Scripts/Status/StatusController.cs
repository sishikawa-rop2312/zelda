using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusController : MonoBehaviour
{
    // StatusController型のインスタンスを保持
    public static StatusController instance;

    void Awake()
    {
        if (instance == null)
        {
            //このインスタンスをinstanceに登録
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // 2回目以降重複して作成してしまったgameObjectを削除
            Destroy(gameObject);
        }
    }
}
