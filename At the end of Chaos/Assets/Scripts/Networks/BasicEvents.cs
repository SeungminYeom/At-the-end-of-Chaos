using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class BasicEvents : MonoBehaviourPunCallbacks
{
    PhotonView pv;
    void Start()
    {
        pv = gameObject.GetComponent<PhotonView>();
    }

    void Update()
    {
        if (pv.IsMine);
        {
            float axisY = Input.GetAxisRaw("Vertical");
            float axisX = Input.GetAxisRaw("Horizontal");

            transform.position += new Vector3(axisX * Time.deltaTime * 3,0,  axisY * Time.deltaTime * 3);
        }
    }

    public void rpcCallback(GameObject _pObj, string _name)
    {
        pv.RPC("UpdateNickname", RpcTarget.Others, _pObj, _name);
    }

    public void FixedUpdate()
    {
        
    }

    [PunRPC]
    void UpdateNickname(GameObject _pObj, string _name)
    {
        _pObj.GetComponentInChildren<TextMesh>().text = _name;
    }
}
