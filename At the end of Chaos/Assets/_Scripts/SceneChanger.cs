using Photon.Pun.Demo.SlotRacer.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UIElements;
using Color = UnityEngine.Color;

public class SceneChanger : MonoBehaviour
{
    public UnityEngine.UI.Image img;

    float fadeTime = 5f;
    float time = 0f;
    Color color = Color.black;
    void Start()
    {
        StartCoroutine(Disappear());
    }

    void Update()
    {
        
    }

    IEnumerator Disappear()
    {
        yield return new WaitForSeconds(4f);
        while (color.a > 0f)
        {
            time += Time.deltaTime / fadeTime;
            color.a = Mathf.Lerp(1, 0, time);

            float t = Mathf.Pow(Mathf.Clamp01(time), 0.25f);

            Camera.main.transform.rotation = Quaternion.Euler(Mathf.Lerp(-5, 25, t), 10, 0); 

            img.color = color;
            yield return null;
        }
        img.gameObject.SetActive(false);
    }
}
