using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMgr : MonoBehaviour
{
    void Start()
    {
        PhotonNetwork.Instantiate("Prefabs/Player", Vector3.zero, Quaternion.identity);
    }
}
