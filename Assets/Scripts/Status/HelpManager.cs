using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpManager : MonoBehaviour
{
    public GameObject helpPanel;
    bool isHelpActive = false;

    void Start()
    {
        helpPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            if (isHelpActive)
            {
                HideHelp();
            }
            else
            {
                ShowHelp();
            }
        }
    }

    void ShowHelp()
    {
        helpPanel.SetActive(true);
        Time.timeScale = 0f; // ゲームを一時停止
        isHelpActive = true;
    }

    void HideHelp()
    {
        helpPanel.SetActive(false);
        Time.timeScale = 1f; // ゲームを再開
        isHelpActive = false;
    }
}
