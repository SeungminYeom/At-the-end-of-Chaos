using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;

public class ZombieManager : MonoBehaviour
{
    public GameObject zombie;
    public TrainManager trainManager;
    public Transform spawnSpot;
    [SerializeField] List<GameObject> zombieList = new List<GameObject>();
    [SerializeField] float spawnDistance;

    void Start()
    {
        InvokeRepeating("SpawnZombie", 0f, 2f);
        trainManager = GameObject.Find("TrainManager").GetComponent<TrainManager>();
    }

    void Update()
    {

    }

    void SpawnZombie()
    {
        int angle = Random.Range(0, 360);
        float x = Mathf.Cos(angle * Mathf.Deg2Rad) * spawnDistance;
        float z = Mathf.Sin(angle * Mathf.Deg2Rad) * spawnDistance;
        Vector3 pos = trainManager.GetTrain(GameManager.instance.trainCount).transform.position + new Vector3(x, 1f, z);
        zombieList.Add(Instantiate(zombie, pos, Quaternion.identity));
    }
}
