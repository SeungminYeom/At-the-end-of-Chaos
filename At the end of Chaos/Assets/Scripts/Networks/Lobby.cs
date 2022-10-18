using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using System.Linq;
using Photon.Pun.UtilityScripts;

public class Lobby : MonoBehaviourPunCallbacks
{
    private string gameVersion = "1.0";
    private bool createGameEnabled = false;
    private PhotonView pv;

    public GameObject[] players = new GameObject[4];

    public Vector2[] playerPos = new Vector2[4];

    public Text connectionInfoText;
    public Button createBtn;
    public Button joinBtn;
    public Button connectBtn;
    public Button cancelBtn;
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
        if(playerName.text.Length == 0)
        {
            PhotonNetwork.LocalPlayer.NickName = "unknown";
        } else
        {
            PhotonNetwork.LocalPlayer.NickName = playerName.text;
        }
        
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
        cancelBtn.gameObject.SetActive(true);

    }

    public void CreateButton()
    {
        if (!createGameEnabled)
        {
            joinBtn.gameObject.SetActive(false);
            roomCodeInput.gameObject.SetActive(true);
            cancelBtn.gameObject.SetActive(true);
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

    public void Cancel()
    {
        createBtn.gameObject.SetActive(true);
        joinBtn.gameObject.SetActive(true);
        connectBtn.gameObject.SetActive(false);
        roomCodeInput.gameObject.SetActive(false);
        cancelBtn.gameObject.SetActive(false);
        createGameEnabled = false;
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
        createBtn.gameObject.SetActive(false);
        joinBtn.gameObject.SetActive(false);
        connectBtn.gameObject.SetActive(false);
        roomCodeInput.gameObject.SetActive(false);
        playerName.gameObject.SetActive(false);
        cancelBtn.gameObject.SetActive(false);
        connectionInfoText.text = "게임에 참가하였습니다.\nGameCode : " + roomCodeInput.text;
    }

    public override void OnCreatedRoom()
    {
        if (playerName.text.Length == 0)
        {
            Debug.Log("unknown");
            PhotonNetwork.NickName = "unknown";
        }
        else
        {
            Debug.Log(PhotonNetwork.NickName);
            PhotonNetwork.NickName = playerName.text;
        }
        createBtn.gameObject.SetActive(false);
        joinBtn.gameObject.SetActive(false);
        connectBtn.gameObject.SetActive(false);
        roomCodeInput.gameObject.SetActive(false);
        cancelBtn.gameObject.SetActive(false);
        connectionInfoText.text = "게임 생성됨.\nGameCode : " + roomCodeInput.text;
        players[0] = PhotonNetwork.Instantiate("player", new Vector3(playerPos[0].x, 0.5f, playerPos[0].y), Quaternion.identity);
        players[0].transform.GetComponentInChildren<TextMesh>().text = PhotonNetwork.NickName;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        for (int i = 1; i < 4; i++)
        {
            if (players[i] == null)
            {
                players[i] = PhotonNetwork.Instantiate("Player", new Vector3(playerPos[i].x, 0.5f, playerPos[i].y), Quaternion.identity);
                players[i].transform.GetComponentInChildren<TextMesh>().text = newPlayer.NickName;
                return;
            }
        }
    }
    

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //나간 플레이어 오브젝트를 찾아서 지운다.
        for (int i = 1; i < 4; i++)
        {
            if (players[i] != null && players[i].GetComponentInChildren<TextMesh>().text == otherPlayer.NickName)
            {
                Destroy(players[i]);
                players[i] = null;
                return;
            }
            
        }

        //나간 플레이어가 Master
        if (otherPlayer.IsMasterClient)
        {

        }
    }

    public void UpdatePlayerName()
    {
        for (int i = 1; i < 4; i++)
        {
            if (players[i] != null)
            {
                string pName = players[i].GetComponentInChildren<TextMesh>().text;
                pv.RPC("ChangePlayerName", RpcTarget.Others, players[i], pName);
                return;
            }
        }
    }

    public void ChangePlayerName(GameObject p, string n)
    {
        p.transform.GetComponentInChildren<TextMesh>().text = n;
    }
}
