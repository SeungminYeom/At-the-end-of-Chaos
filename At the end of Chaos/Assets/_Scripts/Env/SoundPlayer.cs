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

    public AudioClip[] shotgunFire;

    public AudioClip[] sniperFire;

    public AudioClip[] assaultFire;
    
    public AudioClip[] TrainAttacked;
    void Awake()
    {
        instance = this;
        pistolFire = Resources.LoadAll<AudioClip>("Sounds/GunSound/Pistol/Fire");
        pistolCocking = Resources.LoadAll<AudioClip>("Sounds/GunSound/Pistol/Cocking");
        pistolRM = Resources.LoadAll<AudioClip>("Sounds/GunSound/Pistol/RemoveMag");
        pistolIM = Resources.LoadAll<AudioClip>("Sounds/GunSound/Pistol/InsertMag");
        shotgunFire = Resources.LoadAll<AudioClip>("Sounds/GunSound/Shotgun/Fire");
        TrainAttacked = Resources.LoadAll<AudioClip>("Sounds/TrainSound/Attacked");
    }

    public void PlaySound(AudioClip[] _clip, Vector3 _pos)
    {
        AudioSource.PlayClipAtPoint(_clip[Random.Range(0, _clip.Length)], _pos);
    }
}
