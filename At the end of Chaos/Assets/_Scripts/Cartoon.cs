using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Cartoon : MonoBehaviour
{
    GameObject[] cartoon = new GameObject[6];
    GameObject[] text = new GameObject[6];
    //몇번째 그림이 켜졌는지 확인
    int count = 0;

    private void Start()
    {
        for (int i = 0; i < 6; i++)
        {
            cartoon[i] = transform.Find("Cartoon").GetChild(i).gameObject;
            text[i] = transform.Find("Cartoon").Find("Text").GetChild(i).gameObject;
        }

        StartCoroutine(NextCut(count));
    }

    public void TouchScreen()
    {
        if (count >= 6)
        {
            ChangeScene();
            return;
        }

        Debug.Log("Touch");
        StartCoroutine(OnCartoon(count));
        count++;
    }

    public void ChangeScene()
    {
        SceneManager.LoadScene("Lobby");
    }

    IEnumerator OnCartoon(int i)
    {
        float alpha = 0;
        cartoon[i].SetActive(true);
        text[i].SetActive(true);

        while (true)
        {
            cartoon[i].GetComponent<Image>().color = new Color(1, 1, 1, alpha);
            text[i].GetComponent<Image>().color = new Color(1, 1, 1, alpha - 0.5f);
            alpha += Time.deltaTime;

            if (text[i].GetComponent<Image>().color.a >= 1)
            {
                StartCoroutine(NextCut(count));
                yield break;
            }

            yield return null;
        }
    }

    IEnumerator NextCut(int c)
    {
        yield return new WaitForSeconds(3f);
        if (c == count)
        {
            TouchScreen();
            yield break;
        }
    }
}
