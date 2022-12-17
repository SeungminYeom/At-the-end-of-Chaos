using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundMove : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        if (!(GameManager.instance.timeState == TimeState.night
                || GameManager.instance.timeState == TimeState.nightStart
                || GameManager.instance.timeState == TimeState.nightEnd))
            return;


        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).position -= transform.GetChild(i).right * GameManager.instance.groundSpeed * Time.deltaTime;

            if (transform.GetChild(i).position.x < -12.5 * (transform.childCount - 1))
            {
                transform.GetChild(i).position += transform.GetChild(i).right * 25 * transform.childCount;
            }
        }
    }
}
