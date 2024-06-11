using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treasurebox : MonoBehaviour
{
    Animator animator;
    bool isOpen = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isOpen)
        {
            OpenTreasurebox();
        }
    }

    void OpenTreasurebox()
    {
        animator.SetTrigger("OpenTrigger");
        isOpen = true;
    }
}
