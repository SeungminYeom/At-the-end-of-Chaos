using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainManager : MonoBehaviour
{
    public static TrainManager instance;

    public GameObject[] trains = new GameObject[5];
    //value = { 0f, 6.34f, 6.05f, 5.6f, 6.975f }
    float[] train_1_Pos = { 0f, 6.34f, 12.39f, 17.99f, 24.965f};
    float[] train_2_Pos = { 0f, 0f, 6.05f, 11.65f, 18.625f};
    float[] train_3_Pos = { 0f, 0f, 0f, 5.6f, 12.575f};
    float[] train_4_Pos = { 0f, 0f, 0f, 0f, 6.975f};
    float[] train_5_Pos = { 0f, 0f, 0f, 0f, 0f};
    [SerializeField] SerializableDictionary<int, GameObject> _trains = new SerializableDictionary<int, GameObject>();

    void Start()
    {
        instance = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            GameManager.instance.trainCount++;
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            GameManager.instance.trainCount--;
        }
    }

    public void AddTrain(int n)
    {
        trains[n].SetActive(true);
        SortTrain(n);
    }

    public void SubTrain(int n)
    {
        trains[n - 1].SetActive(false);
        SortTrain(n - 2);
    }

    public void SortTrain(int n)
    {
        //for (int i = 0; i <= n; i++)
        //{
            //trains[n - i].transform.localPosition = new Vector3(i * 6f, 0, 0);
        //}
        trains[0].transform.localPosition = new Vector3(train_1_Pos[n], 0, 0);
        trains[1].transform.localPosition = new Vector3(train_2_Pos[n], 0, 0);
        trains[2].transform.localPosition = new Vector3(train_3_Pos[n], 0, 0);
        trains[3].transform.localPosition = new Vector3(train_4_Pos[n], 0, 0);
        trains[4].transform.localPosition = new Vector3(train_5_Pos[n], 0, 0);
    }

    public GameObject GetTrain(int n)
    {
        GameObject go;
        _trains.TryGetValue(n, out go);

        return go;
    }
}
