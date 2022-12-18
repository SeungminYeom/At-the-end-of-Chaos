using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageDisplayer : MonoBehaviour
{
    float fadeTime = 0.5f;

    float time;
    TMP_Text text;
    Color color;
    Vector3 pos;
    void Start()
    {
        transform.rotation = Camera.main.transform.rotation;
        text = GetComponent<TMP_Text>();
        gameObject.SetActive(false);
    }

    public void Display(float _damage, Vector3 _pos, Color _color)
    {
        pos = _pos;
        pos += new Vector3(0.2f, 2, 0.4f);
        transform.position = pos;
        text.color = _color;
        color = _color;
        time = 0f;
        text.text = _damage.ToString();
        gameObject.SetActive(true);
        StartCoroutine(Disappear());
    }

    IEnumerator Disappear()
    {
        while (color.a > 0f)
        {
            time += Time.deltaTime / fadeTime;
            color.a = Mathf.Lerp(1, 0, time);
            text.color = color;
            yield return null;
        }
        gameObject.SetActive(false);
    }
}
