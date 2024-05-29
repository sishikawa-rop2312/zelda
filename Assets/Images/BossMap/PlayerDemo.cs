using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDemo : MonoBehaviour
{
    void Update()
    {
        transform.Translate(Input.GetAxis("Horizontal") * 5.0f * Time.deltaTime, 0, 0);
        transform.Translate(0, Input.GetAxis("Vertical") * 5.0f * Time.deltaTime, 0);
    }
}
