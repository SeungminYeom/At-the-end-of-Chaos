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

        transform.localPosition = new Vector3(transform.localPosition.x - GameManager.instance.groundSpeed * Time.deltaTime,
                                                transform.localPosition.y, transform.localPosition.z);
        if (transform.localPosition.x <= -60)
        {
            transform.localPosition = new Vector3(transform.localPosition.x + 120,
                                                    transform.localPosition.y,
                                                    transform.localPosition.z);
        }
    }
}
