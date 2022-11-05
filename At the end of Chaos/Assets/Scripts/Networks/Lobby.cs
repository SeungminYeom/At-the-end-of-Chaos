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
using ExitGames.Client.Photon;

public class Lobby : MonoBehaviourPunCallbacks, IPunObservable
{
    private string gameVersion = "1.0";
    private bool createGameEnabled = false;

    private byte requiredPlayer = 1;

    public string[] playerNames = new string[4];
    public GameObject[] players = new GameObject[4];

    public Vector2[] playerPos = new Vector2[4];

    public Text connectionInfoText;
    public Button createBtn;
    public Button joinBtn;
    public Button connectBtn;
    public Button cancelBtn;
    public Button startBtn;
    public TMP_InputField roomCodeInput;
    public TMP_InputField playerName;



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
        if (playerName.text.Length == 0)
        {
            PhotonNetwork.LocalPlayer.NickName = "unknown";
        }
        else
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
        }
        else
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

    public void StartGame()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount >= requiredPlayer)
        {
            PhotonNetwork.LoadLevel("GameScene");
        } else
        {
            Debug.Log(PhotonNetwork.CurrentRoom.PlayerCount + " <= " + requiredPlayer);
        }
        
    }

    public void Cancel()
    {
        createBtn.gameObject.SetActive(true);
        joinBtn.gameObject.SetActive(true);
        connectBtn.gameObject.SetActive(false);
        roomCodeInput.gameObject.SetActive(false);
        cancelBtn.gameObject.SetActive(false);
        playerName.gameObject.SetActive(true);
        connectionInfoText.text = "Online";
        createGameEnabled = false;

        for (int i = 0; i < 4; i++)
        {
            players[i].SetActive(false);
        }
        
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }

        if (PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.LeaveRoom();

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
        //UI 변경
        createBtn.gameObject.SetActive(false);
        joinBtn.gameObject.SetActive(false);
        connectBtn.gameObject.SetActive(false);
        roomCodeInput.gameObject.SetActive(false);
        playerName.gameObject.SetActive(false);
        cancelBtn.gameObject.SetActive(true);
        connectionInfoText.text = "게임에 참가하였습니다.\nGameCode : " + roomCodeInput.text;

        StartCoroutine(InitPlayer());
    }

    public override void OnCreatedRoom()
    {
        if (playerName.text.Length == 0)
        {
            PhotonNetwork.NickName = "unknown";
        }
        else
        {
            PhotonNetwork.NickName = playerName.text;
        }
        playerNames[0] = PhotonNetwork.NickName;
        createBtn.gameObject.SetActive(false);
        joinBtn.gameObject.SetActive(false);
        connectBtn.gameObject.SetActive(false);
        roomCodeInput.gameObject.SetActive(false);
        cancelBtn.gameObject.SetActive(true);
        connectionInfoText.text = "게임 생성됨.\nGameCode : " + roomCodeInput.text;
        StartCoroutine(InitPlayer());
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        //플레이어가 들어오면 MasterClient가 플레이어 닉네임 리스트를 업데이트 해준다.
        for (int i = 1; i < 4; i++)
        {
            if (PhotonNetwork.IsMasterClient && !players[i].activeSelf)
            {
                playerNames[i] = newPlayer.NickName;
                break;
            }
        }

        //모든 플레이어가 다른 플레이어의 이름을 오브젝트에 업데이트 한다.
        StartCoroutine(InitPlayer());

        //플레이어가 MasterClient이면서 필요한 인원이 충족되면 게임시작 버튼을 Enable해준다.
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount >= requiredPlayer)
        {
            startBtn.gameObject.SetActive(true);
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        //나간 플레이어 오브젝트를 찾아서 지운다.
        for (int i = 1; i < 4; i++)
        {
            if (players[i].GetComponentInChildren<TextMesh>().text == otherPlayer.NickName)
            {
                playerNames[i] = "";
                players[i].SetActive(false);
                return;
            }

        }

        //나간 플레이어가 Master
        if (otherPlayer.IsMasterClient)
        {

        }
    }

    IEnumerator InitPlayer()
    {
        yield return new WaitForSeconds(0.1f);
        for(int i = 0; i<4; i++)
        {
            if (playerNames[i] != "")
            {
                players[i].GetComponentInChildren<TextMesh>().text = playerNames[i];
                players[i].SetActive(true);
            }
        }
    }

    //변수 동기화
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(playerNames);
        } else
        {
            playerNames = (string[])stream.ReceiveNext();
        }
    }
}
