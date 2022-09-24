using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainManager : MonoBehaviour
{
    public GameObject[] trains = new GameObject[5];

    void Start()
    {
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            GameManager.instance.trainCount++;
            Debug.Log("Z");
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            GameManager.instance.trainCount--;
            Debug.Log("X");
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
        for (int i = 0; i <= n; i++)
        {
            trains[n - i].transform.position = new Vector3(i * 5.25f, 0, 0);
        }
    }
}
