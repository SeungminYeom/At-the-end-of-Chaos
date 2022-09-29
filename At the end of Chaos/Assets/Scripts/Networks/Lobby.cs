using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class Lobby : MonoBehaviourPunCallbacks
{
    private string gameVersion = "1.0";

    public Text connectionInfoText;
    public Button joinBtn;

    void Start()
    {
        PhotonNetwork.GameVersion = this.gameVersion;
        PhotonNetwork.ConnectUsingSettings();

        joinBtn.interactable = false;
        Debug.Log("접속중");
        connectionInfoText.text = "접속중...";
    }

    public override void OnConnectedToMaster()
    {
        joinBtn.interactable = true;
        connectionInfoText.text = "Online";
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        joinBtn.interactable = false;
        connectionInfoText.text = "Offline : 접속 재시도 중...";
        PhotonNetwork.ConnectUsingSettings();
    }

    public void Connect()
    {
        joinBtn.interactable = false;
        if (PhotonNetwork.IsConnected)
        {
            connectionInfoText.text = "게임에 접속중...";
            PhotonNetwork.JoinRandomRoom();
        } else
        {
            connectionInfoText.text = "Offline : 메인 서버에 연결 실패. 재 시도중...";
            PhotonNetwork.ConnectUsingSettings();
        }
    }


    public override void OnJoinRandomFailed(short returnCode, string msg)
    {
        connectionInfoText.text = "새 게임을 만드는중...";
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 4 });
    }

    public override void OnJoinedRoom()
    {
        connectionInfoText.text = "연결됨";
        PhotonNetwork.LoadLevel("GameScene");
    }

    void Update()
    {
        
    }
}
