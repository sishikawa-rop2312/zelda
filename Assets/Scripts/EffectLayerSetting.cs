using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectLayerSetting : MonoBehaviour
{
    [SerializeField]
    private string _sortingLayerName = "Particle";

    // Use this for initialization
    void Start()
    {
        GetComponent<Renderer>().sortingLayerName = _sortingLayerName;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
