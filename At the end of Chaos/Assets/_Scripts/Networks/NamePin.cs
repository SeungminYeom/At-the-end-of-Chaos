using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NamePin : MonoBehaviour
{
    void Update()
    {
        //이름은 언제나 카메라를 바라본다.
        transform.rotation = Quaternion.identity * Camera.main.transform.rotation;
    }
}
