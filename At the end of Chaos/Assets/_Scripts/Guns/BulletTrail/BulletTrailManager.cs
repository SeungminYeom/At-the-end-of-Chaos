using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTrailManager : MonoBehaviour
{
    public static BulletTrailManager instance;

    public PhotonView pv;
    GameObject[] bulletTrails;
    GameObject bulletTrailPool;
    public GameObject bulletTrailGameObject;
    int bulletTrailPoolSize;
    int BTPoolSize = 5;

    void Start()
    {
        instance = this;
        pv = GetComponent<PhotonView>();

        bulletTrailPoolSize = PhotonNetwork.CurrentRoom.PlayerCount * BTPoolSize;

        bulletTrails = new GameObject[bulletTrailPoolSize];
        bulletTrailPool = new GameObject("BulletTrailsPool");
        bulletTrailGameObject = Resources.Load<GameObject>("BulletTrail");
        
        for (int i = 0; i < bulletTrailPoolSize; i++)
        {
            bulletTrails[i] = Instantiate(bulletTrailGameObject, Vector3.zero, Quaternion.identity);
            bulletTrails[i].transform.SetParent(bulletTrailPool.transform);
        }
    }

    [PunRPC] public void PlayEffect(Vector3 from, Vector3 to)
    {   
        for (int i = 0; i < bulletTrailPoolSize; i++)
        {
            if (!bulletTrails[i].activeSelf)
            {
                bulletTrails[i].SetActive(true);
                bulletTrails[i].GetComponent<BulletTrail>().PlayEffect(ref from, ref to);
                break;
            }
        }
    }
}
