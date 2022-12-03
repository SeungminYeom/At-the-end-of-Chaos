using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    public static SoundPlayer instance;

    public AudioClip[] pistolFire;
    public AudioClip[] pistolCocking;
    public AudioClip[] pistolRM;
    public AudioClip[] pistolIM;
    void Awake()
    {
        instance = this;
        pistolFire = Resources.LoadAll<AudioClip>("Sounds/GunSound/Pistol/Fire");
        pistolCocking = Resources.LoadAll<AudioClip>("Sounds/GunSound/Pistol/Cocking");
        pistolRM = Resources.LoadAll<AudioClip>("Sounds/GunSound/Pistol/RemoveMag");
        pistolIM = Resources.LoadAll<AudioClip>("Sounds/GunSound/Pistol/InsertMag");
    }

    void Update()
    {
        
    }
}
