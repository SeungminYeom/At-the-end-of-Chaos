using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameServerManager : MonoBehaviour, IPunObservable
{
    public string[] playerNames = new string[4];

    GameObject LobbyObject;
    GameObject MainCamera;

    void Start()
    {
        PhotonNetwork.UseRpcMonoBehaviourCache = true;
        //필요한 오브젝트를 찾는다.
        LobbyObject = GameObject.Find("LobbyManager");
        MainCamera = GameObject.Find("Main Camera");

        //로비의 있던 플레이어 이름을 갖고오고 로비는 없앤다.
        playerNames = LobbyObject.GetComponent<Lobby>().playerNames;
        Destroy(LobbyObject);

        for (int i = 0; i < 4; i++)
        {
            if (playerNames[i] == PhotonNetwork.NickName)
            {
                //넘어온 닉네임과 내 닉네임을 비교해서 맞는 번호의 캐릭터를 만든다.
                string prefabName = "Player_" + (i + 1);
                //그리고 카메라 조정을 위해 만든 캐릭터를 등록한다.
                MainCamera.GetComponent<CameraMovement>().player = PhotonNetwork.Instantiate(prefabName, Vector3.zero, Quaternion.identity);

            }
        }
        
    }

    void Update()
    {
        
    }



    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(GameManager.instance.seed);
        }
        else
        {
            GameManager.instance.seed = (int)stream.ReceiveNext();
        }
    }
}
