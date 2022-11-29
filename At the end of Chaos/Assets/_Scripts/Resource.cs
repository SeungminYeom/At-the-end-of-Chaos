using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    public int wood = 0;
    public int iron = 0;

    private void OnEnable()
    {
        if (tag == "WoodResource")
            wood = new System.Random().Next(3) + 1;
        else
            iron = new System.Random().Next(3) + 1;
    }

    private void OnDestroy()
    {
        GameManager.instance.inCreaseResource(wood, iron);
        Debug.Log("РќДо : " + wood + ", " + iron);
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.tag == "Player")
    //    {
    //        GameObject.Find("ShootBtn").SetActive(true);
    //    }
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.tag == "Player")
    //    {
    //        GameObject.Find("ShootBtn").SetActive(false);
    //    }
    //}
}
