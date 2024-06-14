using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemArrow : MonoBehaviour
{
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
        if (other != null && other.gameObject.CompareTag("Player"))
        {
            PlayerController pc = other.gameObject.GetComponent<PlayerController>();
            if (pc != null)
            {
                audioSource.PlayOneShot(se);
                pc.ObtainArrow(3);
            }



            Destroy(this.gameObject);
        }
    }
}
