using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class Lobby : MonoBehaviourPunCallbacks
{
    private string gameVersion = "1.0";
    private bool createGameEnabled = false;

    private GameObject[] players = new GameObject[4];

    public Vector2[] playerPos = new Vector2[4];

    public Text connectionInfoText;
    public Button createBtn;
    public Button joinBtn;
    public Button connectBtn;
    public TMP_InputField roomCodeInput;
    public TMP_InputField playerName;
    public GameObject playerPrefab;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = this.gameVersion;
    }

    void Start()
    {
        
        PhotonNetwork.ConnectUsingSettings();
        connectionInfoText.text = "접속중...";

    }

    public override void OnConnectedToMaster()
    {
        joinBtn.interactable = true;
        createBtn.gameObject.SetActive(true);
        joinBtn.gameObject.SetActive(true);
        playerName.gameObject.SetActive(true);
        connectionInfoText.text = "Online";
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        joinBtn.interactable = false;
        connectionInfoText.text = "Offline : 접속 재시도 중...";
        PhotonNetwork.ConnectUsingSettings();
    }

    public void ConnectGame()
    {
        if (roomCodeInput.text.Length == 5)
        {
            joinBtn.interactable = false;
            if (PhotonNetwork.IsConnected)
            {
                connectionInfoText.text = "게임에 접속중...";
                PhotonNetwork.JoinRoom(roomCodeInput.text);
            }
            else
            {
                connectionInfoText.text = "Offline : 메인 서버에 연결 실패. 재 시도중...";
                PhotonNetwork.ConnectUsingSettings();
            }
        }
        else
        {
            connectionInfoText.text = "게임 코드 5자리를 입력해주세요";
        }
        
    }

    public void JoinButton()
    {
        createBtn.gameObject.SetActive(false);
        joinBtn.gameObject.SetActive(false);
        connectBtn.gameObject.SetActive(true);
        roomCodeInput.gameObject.SetActive(true);

    }

    public void CreateButton()
    {
        if (!createGameEnabled)
        {
            joinBtn.gameObject.SetActive(false);
            roomCodeInput.gameObject.SetActive(true);
            createGameEnabled = true;
        } else
        {
            if (roomCodeInput.text.Length == 5)
            {
                connectionInfoText.text = "새 게임을 만드는중...";
                PhotonNetwork.CreateRoom(roomCodeInput.text, new RoomOptions { MaxPlayers = 4 });
            }
            else
            {
                connectionInfoText.text = "게임 코드 5자리를 입력해주세요";
            }
        }
        
    }


    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log(returnCode + "///" + PhotonErrorHandler.Instance.Err(returnCode));
        connectionInfoText.text = "게임 참가에 실패하였습니다. \n" + PhotonErrorHandler.Instance.Err(returnCode);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        connectionInfoText.text = "게임 생성에 실패하였습니다.\n" + PhotonErrorHandler.Instance.Err(returnCode);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LocalPlayer.NickName = playerName.text;
        connectionInfoText.text = "게임에 참가하였습니다.\nGameCode : " + roomCodeInput.text;
    }

    public override void OnCreatedRoom()
    {
        PhotonNetwork.LocalPlayer.NickName = playerName.text;
        connectionInfoText.text = "게임 생성됨.\nGameCode : " + roomCodeInput.text;
        players[0] = Instantiate(playerPrefab);
        players[0].transform.position = new Vector3(playerPos[0].x, 0.5f, playerPos[0].y);
        players[0].transform.GetComponentInChildren<TextMesh>().text = PhotonNetwork.LocalPlayer.NickName;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        for (int i = 1; i < 4; i++)
        {
            if (players[i].IsDestroyed())
            {
                players[i] = Instantiate(playerPrefab);
                players[i].transform.position = new Vector3(playerPos[i].x, 0.5f, playerPos[i].y);
                players[i].transform.GetComponentInChildren<TextMesh>().text = newPlayer.NickName;
            }
        }
    }
}
