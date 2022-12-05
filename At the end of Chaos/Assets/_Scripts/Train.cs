using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Train : MonoBehaviour
{
    [SerializeField] int health = 10;
    [SerializeField] bool invincible;

    void Start()
    {
        
    }

    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Zombie" && !invincible)
        {
            health -= 1;
            SoundPlayer.instance.PlaySound(SoundPlayer.instance.TrainAttacked, collision.transform.position);
            if (health == 0)
            {
                StartCoroutine(trainTimeEffect());
                //GameManager.instance.trainCount--;
            }
        }
    }

    IEnumerator trainTimeEffect()
    {
        float sTime = Time.time;
        gameObject.transform.parent.Find("TrainExplosion_A").GetComponent<ParticleSystem>().Play();
        gameObject.transform.parent.Find("TrainExplosion_B").GetComponent<ParticleSystem>().Play();
        gameObject.transform.parent.Find("TrainExplosion_A").GetComponent<AudioSource>().Play();
        gameObject.transform.parent.Find("TrainExplosion_B").GetComponent<AudioSource>().Play();
        transform.Find("default").gameObject.SetActive(false);
        Time.timeScale = 0.5f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        while (true)
        {
            if (Time.timeScale == 0.1f) break;
            Time.timeScale = Mathf.Lerp(Time.timeScale, 0.1f, (Time.time - sTime) / 1f);
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            yield return null;
        }
        Debug.Log("느리게 만들기 끝");

        sTime = Time.time;
        while (true)
        {
            if (Time.timeScale == 1f) break;
            Time.timeScale = Mathf.Lerp(Time.timeScale, 1f, (Time.time - sTime) / 0.3f);
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            Debug.Log(Time.timeScale);
            Debug.Log(Time.time);
            yield return null;
        }
        Debug.Log("빠르게 만들기 끝");

        transform.Find("default").gameObject.SetActive(true);
        GameManager.instance.trainCount--;


        yield break;
    }
}
