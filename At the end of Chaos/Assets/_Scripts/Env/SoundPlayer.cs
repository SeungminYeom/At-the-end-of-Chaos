using Photon.Pun.Demo.Cockpit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

using Random = UnityEngine.Random;

public class SoundPlayer : MonoBehaviour
{
    public static SoundPlayer instance;

    public AudioClip[] pistolFire;
    public AudioClip[] pistolCocking;
    public AudioClip[] pistolRM;
    public AudioClip[] pistolIM;

    public AudioClip[] shotgunFire;
    public AudioClip[] shotgunCocking;
    public AudioClip[] shotgunRM;
    public AudioClip[] shotgunIM;

    public AudioClip[] assaultFire;
    public AudioClip[] assaultCocking;
    public AudioClip[] assaultRM;
    public AudioClip[] assaultIM;

    public AudioClip[] sniperFire;
    public AudioClip[] sniperCocking;
    public AudioClip[] sniperRM;
    public AudioClip[] sniperIM;
    
    public AudioClip[] TrainAttacked;

    public AudioClip[] backgroundMusics;

    float time = 0f;

    byte nowPlayer = 0;
    GameObject audioPlayerAnchor;
    Dictionary<string, AudioClip> audioClipDictionary;
    AudioSource previousSource;
    int playTime;
    bool interrupt = false;
    float previousVolume;

    int fadeTime = 5;

    void Awake()
    {
        instance = this;
        pistolFire = Resources.LoadAll<AudioClip>("Sounds/GunSound/Pistol/Fire");
        pistolCocking = Resources.LoadAll<AudioClip>("Sounds/GunSound/Pistol/Cocking");
        pistolRM = Resources.LoadAll<AudioClip>("Sounds/GunSound/Pistol/RemoveMag");
        pistolIM = Resources.LoadAll<AudioClip>("Sounds/GunSound/Pistol/InsertMag");
        TrainAttacked = Resources.LoadAll<AudioClip>("Sounds/TrainSound/Attacked");
        backgroundMusics = Resources.LoadAll<AudioClip>("Sounds/Environment/ImminentThreat");
    }

    void Start()
    {
        audioClipDictionary = new Dictionary<string, AudioClip>();

        audioPlayerAnchor = GameObject.Find("BGM");
        for (int i = 0; i < backgroundMusics.Length; i++)
        {
            audioClipDictionary.Add(backgroundMusics[i].name, backgroundMusics[i]);
        }

        BGMChange("CharacterSelect");
    }

    public void PlaySound(AudioClip[] _clip, Vector3 _pos)
    {
        AudioSource.PlayClipAtPoint(_clip[Random.Range(0, _clip.Length)], _pos);
    }

    public void BGMChange(string _BGMName)
    {
        nowPlayer++;
        GameObject audio = new GameObject("Audio" + nowPlayer, typeof(AudioSource));
        audio.transform.SetParent(audioPlayerAnchor.transform);
        AudioSource source = audio.GetComponent<AudioSource>();

        AudioClip clip;
        audioClipDictionary.TryGetValue(_BGMName, out clip);

        source.loop = true;
        source.volume = 0;
        source.clip = clip;

        if (nowPlayer != 1)
        {
            if (!previousSource || previousSource.volume != 1)
            {
                interrupt = true;
            }
        }
        
        StartCoroutine(Fader(source));
    }

    IEnumerator Fader(AudioSource _source)
    {
        time = 0;
        playTime = DateTime.Now.Second;
        while ((DateTime.Now.Second - playTime) % 4 == 0)
        {
            yield return null;
        }

        _source.Play();
        while (_source.volume != 1 && !interrupt)
        {
            time += Time.deltaTime / fadeTime;

            _source.volume = Mathf.Lerp(0, 1, time);
            if (previousSource)
            {
                previousSource.volume = Mathf.Lerp(previousVolume, 0, time);
                Destroy(previousSource.gameObject, fadeTime);
            }
            yield return null;
        }
        previousSource = _source;
        previousVolume = previousSource.volume;
        interrupt = false;
        
    }
}
