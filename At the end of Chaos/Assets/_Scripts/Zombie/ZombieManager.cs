using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon.StructWrapping;
using Unity.VisualScripting;
using System;

public class ZombieManager : MonoBehaviour
{
    //좀비들의 총 스탯 퍼센트를 관리하는 매니저

    public static ZombieManager instance;

    string[] idleZombie = new string[3];
    public TrainManager trainManager;
    public Transform spawnSpot;
    [SerializeField] List<GameObject> zombieList = new List<GameObject>();
    [SerializeField] float spawnDistance;
    [SerializeField] float spawnTimeInterval = 1f;

    [NonSerialized] public float health = 500;
    [NonSerialized] public float def = 1;

    [SerializeField] LayerMask lm;
    [SerializeField] Collider[] colliders;
    float a = 80000f, b = 20, c = 0;
    GameObject zombieAnchor;

    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != null) Destroy(gameObject);
    }

    void Start()
    {
        idleZombie[0] = "zombie_body_standing";
        idleZombie[1] = "zombie_man_standing";
        idleZombie[2] = "zombie_woman_standing";
        trainManager = GameObject.Find("TrainManager").GetComponent<TrainManager>();
        zombieAnchor = GameObject.Find("ZombieManager");
    }

    void Update()
    {

    }

    public IEnumerator SpawnZombie()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            while (true)
            {
                int angle = UnityEngine.Random.Range(0, 360);
                float x = Mathf.Cos(angle * Mathf.Deg2Rad) * spawnDistance;
                float z = Mathf.Sin(angle * Mathf.Deg2Rad) * spawnDistance;
                Vector3 pos = trainManager.GetTrain(GameManager.instance.trainCount).transform.position + new Vector3(x, 1f, z);
                zombieList.Add(PhotonNetwork.InstantiateRoomObject(idleZombie[UnityEngine.Random.Range(0, 3)], pos, Quaternion.identity));
                zombieList[zombieList.Count - 1].transform.parent = zombieAnchor.transform;
                yield return new WaitForSeconds(spawnTimeInterval);
            }
        }
    }

    public void DestroyZombies()
    {
        for (int i = 0; i < zombieAnchor.transform.childCount; i++)
        {
            zombieAnchor.transform.GetChild(i).GetComponent<Zombie>().Die();
        }
    }

    public IEnumerator ExplosionZombies()
    {
        colliders = Physics.OverlapSphere(Vector3.zero, b, lm);
        Debug.Log("B");
        Vector3 vec = new Vector3(0, -5, 0);
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].GetComponent<Zombie>().targeting = false;
            colliders[i].GetComponent<Zombie>().GetComponent<Rigidbody>().AddExplosionForce(a, vec, b, c);
            int explosionDamage = colliders[i].GetComponent<Zombie>().health / 2;
            explosionDamage--;
            DamageDisplayManager.instance.Display(explosionDamage, colliders[i].transform.position);
            colliders[i].GetComponent<Zombie>().health -= explosionDamage;
        }
        yield return new WaitForSeconds(3f);
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].GetComponent<Zombie>().targeting = true;
        }
    }

    public void StrongerZombies()
    {
        health *= 1.1f;
        //def += 0.1f;
        //난이도 조절은 체력만으로 하고 방어력은 특수 좀비가 갖는다.
    }
}
