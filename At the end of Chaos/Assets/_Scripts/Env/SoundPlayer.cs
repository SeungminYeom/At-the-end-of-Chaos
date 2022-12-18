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

    const int fadeTime = 3;

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

    public void BGMChange(string _BGMName, int _fadeTime = fadeTime)
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
            } else if (_BGMName == "LowHealthIntro" && GameManager.instance.timeState == TimeState.night)
            {
                audioClipDictionary.TryGetValue("Bridge", out clip);
                source.clip = clip;
                StartCoroutine(Fader(source, 1));
                return;
            } else if (_BGMName == "GameOver")
            {
                previousSource.loop = false;
                source.volume = 1;
                source.playOnAwake = false;
                source.loop = false;
                StartCoroutine(Fader(source, 1));
                return;
            }
        }
        
        StartCoroutine(Fader(source, _fadeTime));
    }

    IEnumerator Fader(AudioSource _source, int _fadeTime)
    {
        if (_source.clip.name == "GameOver")
        {
            while (previousSource.isPlaying)
            {
                Debug.Log("wait");
                yield return null;
            }
            Debug.Log("Over");
            _source.Play();
            interrupt = true;
        } 
        else
        {
            Debug.Log("노래 변경");
            time = 0;
            if (_source.clip.name != "LowHealthIntro")
            {
                _source.Play();
            }
            while (_source.volume != 1 && !interrupt)
            {
                time += Time.deltaTime / _fadeTime;

                _source.volume = Mathf.Lerp(0, 1, time);
                if (previousSource)
                {
                    previousSource.volume = Mathf.Lerp(previousVolume, 0, time);
                    Destroy(previousSource.gameObject, _fadeTime);
                }
                yield return null;
            }

            previousSource = _source;
            previousVolume = previousSource.volume;
            interrupt = false;

            if (_source.clip.name == "Bridge" || _source.clip.name == "LowHealthIntro")
            {
                AudioClip ac;
                audioClipDictionary.TryGetValue("LowHealthIntro", out ac);
                _source.clip = ac;
                _source.Play();
                yield return new WaitForSeconds(ac.length);
                audioClipDictionary.TryGetValue("LowHealth", out ac);
                _source.clip = ac;
                _source.Play();
            }
        }
    }
}
