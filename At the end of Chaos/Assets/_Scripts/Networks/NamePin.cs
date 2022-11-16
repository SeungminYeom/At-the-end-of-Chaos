using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NamePin : MonoBehaviour
{
    void Update()
    {
        transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
    }
}
