using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UIElements;

public class BasicEvents : MonoBehaviourPunCallbacks
{
    PhotonView pv;
    void Start()
    {
        pv = gameObject.GetComponent<PhotonView>();
    }

    void Update()
    {
        float axisY = Input.GetAxisRaw("Vertical");
        float axisX = Input.GetAxisRaw("Horizontal");
        Vector3 nPos = new Vector3(axisX * Time.deltaTime * 3, 0, axisY * Time.deltaTime * 3);
        //MoveAxis(nPos);
        if (pv.IsMine)
        {
            pv.RPC("MoveAxis", RpcTarget.All, nPos);
        }
    }

    [PunRPC]
    void MoveAxis(Vector3 _pos) { 
        gameObject.transform.Translate(_pos);
    }
}
