using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Train : MonoBehaviour
{
    int health = 1000;
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
            if (health <= 0)
            {
                StartCoroutine(trainTimeEffect());
                //GameManager.instance.trainCount--;
            }
        }
    }

    IEnumerator trainTimeEffect()
    {
        Time.timeScale = 0.4f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        yield return new WaitForSeconds(2f);
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        GameManager.instance.trainCount--;
    }
}
