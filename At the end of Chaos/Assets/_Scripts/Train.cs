using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class Train : MonoBehaviour
{
    [SerializeField] int health = 20;
    [SerializeField] bool invincible;

    public int maxHealth = 20;

    void Start()
    {
        health = maxHealth;
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
        GameServerManager.instance.GetComponent<SceneChanger>().StartCoroutine("Cinema", true);
        yield return new WaitForSeconds(1.5f);
        float sTime = Time.time;
        StartCoroutine(gameObject.GetComponent<TraumaInducer>().ExplosionShake());
        gameObject.transform.parent.Find("TrainExplosion_A").GetComponent<ParticleSystem>().Play();
        gameObject.transform.parent.Find("TrainExplosion_B").GetComponent<ParticleSystem>().Play();
        gameObject.transform.parent.Find("TrainExplosion_A").GetComponent<AudioSource>().Play();
        gameObject.transform.parent.Find("TrainExplosion_B").GetComponent<AudioSource>().Play();
        transform.Find("default").gameObject.SetActive(false);
        ZombieManager.instance.StartCoroutine(ZombieManager.instance.ExplosionZombies());
        Time.timeScale = 0.5f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        while (true)
        {
            if (Time.timeScale == 0.1f) break;
            Time.timeScale = Mathf.Lerp(Time.timeScale, 0.1f, (Time.time - sTime) / 0.6f);
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            yield return null;
        }
        Debug.Log("느리게 만들기 끝");

        if (GameManager.instance.trainCount != 1)
        {
            Vector3 originPos = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z);
            Vector3 targetPos = new Vector3(Camera.main.transform.position.x + 6.34f, Camera.main.transform.position.y, Camera.main.transform.position.z);
            sTime = Time.time;
            while (true)
            {
                if (Vector3.Distance(Camera.main.transform.position, targetPos) <= 0.01) break;
                Camera.main.transform.position = Vector3.Lerp(originPos, targetPos, (Time.time - sTime) / 0.2f);
                yield return null;
            }
            Debug.Log("카메라 이동 끝");

            sTime = Time.time;
            GameServerManager.instance.GetComponent<SceneChanger>().StartCoroutine("Cinema", false);
            int trainCount = transform.GetSiblingIndex();
            Transform[] nextTrains = new Transform[trainCount];

            for (int i = 0; i < trainCount; i++)
            {
                nextTrains[i] = transform.parent.GetChild(trainCount - i - 1);
            }

            Vector3[] origins = new Vector3[trainCount + 1];
            origins[0] = Vector3.zero;
            for (int i = 0; i < trainCount; i++)
            {
                origins[i + 1] = nextTrains[i].position;
            }
            if (GameManager.instance.trainCount == 2)
            {
                SoundPlayer.instance.BGMChange("LowHealthIntro");
            }

            while (true)
            {
                if (Time.timeScale == 1f) break;
                Time.timeScale = Mathf.Lerp(Time.timeScale, 1f, (Time.time - sTime) / 0.5f);
                Camera.main.transform.position = Vector3.Lerp(targetPos, originPos, (Time.time - sTime) / 0.5f);
                for (int i = 0; i < trainCount; i++)
                {
                    nextTrains[i].position = Vector3.Lerp(origins[i + 1], origins[i], (Time.time - sTime) / 0.5f);
                }

                Time.fixedDeltaTime = 0.02f * Time.timeScale;
                yield return null;
            }
            Debug.Log("빠르게 만들기 끝");
            transform.Find("default").gameObject.SetActive(true);
            GameManager.instance.trainCount--;
        }
        else
        {
            SoundPlayer.instance.BGMChange("GameOver");
            GameServerManager.instance.GetComponent<SceneChanger>().StartCoroutine("Close", true);
        }
        
        
        


        yield break;
    }

    public int Hp
    {
        get {
            return health;
        }

        set {
            health = value;
        }
    }

    public void RestoreHealth()
    {
        health = maxHealth;
    }
}
