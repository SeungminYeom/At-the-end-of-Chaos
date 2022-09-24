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
        if (!(GameManager.instance.timeState == TimeState.night ||
            GameManager.instance.timeState == TimeState.nightStart ||
            GameManager.instance.timeState == TimeState.nightEnd))
            return;

        transform.position = new Vector3(transform.position.x - GameManager.instance.groundSpeed * Time.deltaTime, transform.position.y, transform.position.z);
        if (transform.position.x <= -60)
        {
            transform.position = new Vector3(transform.position.x + 120, transform.position.y, transform.position.z);
        }
    }
}
