using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NamePin : MonoBehaviour
{
    void Update()
    {
        //�̸��� ������ ī�޶� �ٶ󺻴�.
        transform.rotation = Quaternion.identity * Camera.main.transform.rotation;
    }
}
