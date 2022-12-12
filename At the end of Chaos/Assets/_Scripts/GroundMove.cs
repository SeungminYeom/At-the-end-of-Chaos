using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundMove : MonoBehaviour
{
    Transform[] TerrainArr = new Transform[8];

    void Start()
    {
        //for (int i = 0; i < transform.childCount; i++)
        for (int i = 0; i < 8; i++)
        {
            TerrainArr[i] = transform.GetChild(i);
        }
    }

    void Update()
    {
        if (!(GameManager.instance.timeState == TimeState.night ||
            GameManager.instance.timeState == TimeState.nightStart ||
            GameManager.instance.timeState == TimeState.nightEnd))
            return;

        foreach (var t in TerrainArr)
        {
            t.localPosition += new Vector3(-GameManager.instance.groundSpeed * Time.deltaTime, 0f, 0f);

            if (t.localPosition.x <= -87.5f)
            {
                t.localPosition += new Vector3(200f, 0f, 0f);
            }

        }
        //transform.localPosition = new Vector3(transform.localPosition.x - GameManager.instance.groundSpeed * Time.deltaTime,
        //                                        transform.localPosition.y, transform.localPosition.z);
        //if (transform.localPosition.x <= -60f)
        //{
        //    //transform.localPosition = new Vector3(transform.localPosition.x + 120,
        //    //                                        transform.localPosition.y,
        //    //                                        transform.localPosition.z);
        //    //transform.GetSiblingIndex -> 인스펙터에서 내 오브젝트 번호 갖고 옴
        //    transform.localPosition = transform.parent.GetChild(
        //                                (transform.GetSiblingIndex() + transform.parent.childCount - 1)
        //                                    % transform.parent.childCount).localPosition
        //                                + new Vector3(25f, 0, 0);
        //}
    }
}
