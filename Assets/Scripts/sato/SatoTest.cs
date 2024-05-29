using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public float speed = 5f;

    public KeyCode moveUpKey = KeyCode.W;
    public KeyCode moveDownKey = KeyCode.S;
    public KeyCode moveLeftKey = KeyCode.A;
    public KeyCode moveRightKey = KeyCode.D;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 position = transform.position;

        if (Input.GetKey(moveUpKey))
        {
            position.y += speed * Time.deltaTime;
        }
        if (Input.GetKey(moveDownKey))
        {
            position.y -= speed * Time.deltaTime;
        }
        if (Input.GetKey(moveLeftKey))
        {
            position.x -= speed * Time.deltaTime;
        }
        if (Input.GetKey(moveRightKey))
        {
            position.x += speed * Time.deltaTime;
        }

        // オブジェクトの位置を更新
        transform.position = position;
    }
}
