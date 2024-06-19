using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleGimmickController : MonoBehaviour
{
    public GameObject obstacleObject;
    AudioSource audioSource;
    public AudioClip se;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (obstacleObject != null)
            {
                obstacleObject.SetActive(true);
                audioSource.PlayOneShot(se);
            }
        }
    }
}
