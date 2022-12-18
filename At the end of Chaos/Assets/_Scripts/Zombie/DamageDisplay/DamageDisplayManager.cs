using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDisplayManager : MonoBehaviour
{
    public static DamageDisplayManager instance;

    GameObject[] damageDisplay;
    GameObject damageDisplayPool;
    public GameObject damageDisplayGameObject;
    int damageDisplayPoolSize;
    int DDPoolSize = 30;

    void Start()
    {
        instance = this;

        damageDisplayPoolSize = PhotonNetwork.CurrentRoom.PlayerCount * DDPoolSize;

        damageDisplay = new GameObject[damageDisplayPoolSize];
        damageDisplayPool = new GameObject("DamageDisplayPool");
        damageDisplayGameObject = Resources.Load<GameObject>("DamageDisplay");

        for (int i = 0; i < damageDisplayPoolSize; i++)
        {
            damageDisplay[i] = Instantiate(damageDisplayGameObject, Vector3.zero, Quaternion.identity);
            damageDisplay[i].transform.SetParent(damageDisplayPool.transform);
        }
    }

    public void Display(float _damage, Vector3 _pos)
    {
        for (int i = 0; i < damageDisplayPoolSize; i++)
        {
            if (!damageDisplay[i].activeSelf)
            {
                damageDisplay[i].SetActive(true);
                damageDisplay[i].GetComponent<DamageDisplayer>().Display(_damage, _pos, Color.yellow);
                break;
            }
        }
    }
}
