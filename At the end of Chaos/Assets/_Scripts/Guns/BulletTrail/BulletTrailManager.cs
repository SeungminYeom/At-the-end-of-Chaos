using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTrailManager : MonoBehaviour
{
    [SerializeField] GameObject bulletTrailGameObject;
    [SerializeField] int BTPoolSize;

    public PhotonView pv;
    GameObject[] bulletTrails;
    int bulletTrailPoolSize;

    void Start()
    {
        pv = GetComponent<PhotonView>();

        bulletTrailPoolSize = PhotonNetwork.CurrentRoom.PlayerCount * BTPoolSize;
        bulletTrails = new GameObject[bulletTrailPoolSize];
        for (int i = 0; i < bulletTrailPoolSize; i++)
        {
            bulletTrails[i] = Instantiate(bulletTrailGameObject, Vector3.zero, Quaternion.identity);
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
