using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;

public class ZombieManager : MonoBehaviour
{
    //좀비들의 총 스탯 퍼센트를 관리하는 매니저

    public static ZombieManager instance;

    [SerializeField] GameObject zombie;
    public TrainManager trainManager;
    public Transform spawnSpot;
    [SerializeField] List<GameObject> zombieList = new List<GameObject>();
    [SerializeField] float spawnDistance;
    [SerializeField] float spawnTimeInterval = 1f;

    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != null) Destroy(gameObject);
    }

    void Start()
    {
        zombie = Resources.Load<GameObject>("Zombie");
        trainManager = GameObject.Find("TrainManager").GetComponent<TrainManager>();
    }

    void Update()
    {

    }

    public IEnumerator SpawnZombie()
    {
        while(true)
        {
            int angle = Random.Range(0, 360);
            float x = Mathf.Cos(angle * Mathf.Deg2Rad) * spawnDistance;
            float z = Mathf.Sin(angle * Mathf.Deg2Rad) * spawnDistance;
            Vector3 pos = trainManager.GetTrain(GameManager.instance.trainCount).transform.position + new Vector3(x, 1f, z);
            zombieList.Add(Instantiate(zombie, pos, Quaternion.identity));

            yield return new WaitForSeconds(spawnTimeInterval);
        }
    }
}
