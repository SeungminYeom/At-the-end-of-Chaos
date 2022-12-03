using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXPlayer : MonoBehaviour
{
    public static VFXPlayer instance;
    public ParticleSystem gunSpark;
    void Start()
    {
        instance = this;

        gunSpark = Resources.LoadAll<GameObject>("VFX/Gun/GunSpark")[0].GetComponent<ParticleSystem>();
    }

    void Update()
    {
        
    }
}
