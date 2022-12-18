using Photon.Pun.Demo.SlotRacer.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Color = UnityEngine.Color;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] UnityEngine.UI.Image img;
    [SerializeField] bool isLobby;
    [SerializeField] Canvas cvs;

    float time = 0;

    Color color = Color.black; //시작 색
    float fadeTime = 1f; // 배경 사라지는 시간
    float start = -5f; //시작시 각도
    float end = 25f; //종료 각도
    float easing = 2f; // x < 1 = ease in , x > 1 ease out x = 1 linear  ///  fadeTime이랑 무관한 시간

    RectTransform top, bottom;

    void Start()
    {
        StartCoroutine(Open());
        if (!isLobby)
        {
            top = img.gameObject.transform.GetChild(0).GetComponent<RectTransform>();
            bottom = img.gameObject.transform.GetChild(1).GetComponent<RectTransform>();
        }
    }

    void Update()
    {
        
    }

    IEnumerator Open()
    {
        img.color = color;
        yield return new WaitForSeconds(2f);
        if (isLobby)
        {
            fadeTime = 5f;
            yield return new WaitForSeconds(2f);
        }
        while (color.a > 0f)
        {
            time += Time.deltaTime / fadeTime;
            color.a = Mathf.Lerp(1, 0, time);

            if (isLobby)
            {
                Camera.main.transform.rotation = Quaternion.Euler(
                start + (end - start) * (1 - Mathf.Pow(1 - time, easing))
                , 10, 0);
            }

            img.color = color;
            yield return null;
        }
        img.gameObject.SetActive(false);
    }

    IEnumerator Close(bool _ending = false)
    {
        img.gameObject.SetActive(true);
        time = 0;
        fadeTime = 0.8f;
        AudioSource audio = GetComponent<AudioSource>();

        while (time <= fadeTime)
        {
            time += Time.deltaTime / fadeTime;
            color.a = Mathf.Lerp(0, 1, time / fadeTime);

            if (isLobby) audio.volume = Mathf.Lerp(1, 0, time / fadeTime);


            img.color = color;
            yield return null;
        }
        if (isLobby)
        {
            yield return new WaitForSeconds(2f);
            GetComponent<Lobby>().load.allowSceneActivation = true;
        }
            
    }

    public void ChangeScene()
    {
        StartCoroutine(Close());
    }

    public IEnumerator Cinema(bool _toCine)
    {
        if (_toCine)
        {
            img.gameObject.SetActive(true);
        } else
        {
            img.gameObject.SetActive(false);
        }

        time = 0;
        fadeTime = 1.2f;
        Vector2 size = new Vector2(top.sizeDelta.x, 0);

        while (time <= fadeTime)
        {
            time += Time.deltaTime / fadeTime;
            if (_toCine)
            {
                cvs.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(1, 0, time / fadeTime);
                Camera.main.orthographicSize = Mathf.Lerp(5, 3.5f, time / fadeTime);
                size.y = Mathf.Lerp(0, 120, time / fadeTime);
            } else
            {
                cvs.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(0, 1, time / fadeTime);
                Camera.main.orthographicSize = Mathf.Lerp(3.5f, 5, time / fadeTime);
                size.y = Mathf.Lerp(120, 0, time / fadeTime);
            }
            top.sizeDelta = size;
            bottom.sizeDelta = size;
            yield return null;
        }
    }
}


