using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameServerManager : MonoBehaviour, IOnEventCallback, IPunObservable
{
    public static GameServerManager instance = null;

    public PhotonView pv;
    GameObject lobbyObject;
    GameObject mainCamera;

    public GameObject players;

    //캐릭터 선택창 UI 변수
    public GameObject characterSelectUI;
    public Button b1, b2, b3, b4;
    bool characterSelected = false;
    public byte readyPlayer = 0;

    //시간 Sync용 - 현재 Flow가 끝난 플레이어들은 true로 설정
    //순서는 PhotonNetwork.PlayerList와 같이 설정함
    public bool[] PLAYER_IS_READY;
    public byte myPlayerNum;

    void Awake()
    {
        if (instance == null)
        {
            pv = gameObject.GetComponent<PhotonView>();
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    //RaiseEvent를 사용하려면 등록해야함
    void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    void Start()
    {
        PhotonNetwork.UseRpcMonoBehaviourCache = true;
        //필요한 오브젝트를 찾는다.
        mainCamera = GameObject.Find("Main Camera");

        //로비는 더이상 사용되지 않으므로 지운다.
        Destroy(GameObject.Find("LobbyManager"));

        //플레이어 수만큼 Ready 칸을 만든다.
        PLAYER_IS_READY = new bool[PhotonNetwork.CountOfPlayers];
    }

    void Update()
    {
        if (Array.TrueForAll<bool>(PLAYER_IS_READY, x => x.Equals(true)))
        {
            Debug.LogWarning("ALL READY");
        }
    }

    public void ReadyToGame1()
    {
        if (!characterSelected)
        {
            PhotonNetwork.RaiseEvent(1,
                                        new object[] { PhotonNetwork.NickName },
                                        new RaiseEventOptions { Receivers = ReceiverGroup.All },
                                        new SendOptions { Reliability = true }
                                    );

            initPlayer(1);
        }
        
    }
    public void ReadyToGame2()
    {
        if (!characterSelected)
        {
            PhotonNetwork.RaiseEvent(2,
                                        new object[] { PhotonNetwork.NickName },
                                        new RaiseEventOptions { Receivers = ReceiverGroup.All },
                                        new SendOptions { Reliability = true }
                                    );

            initPlayer(2);
        }
    }
    public void ReadyToGame3()
    {
        if (!characterSelected)
        {
            PhotonNetwork.RaiseEvent(3,
                                        new object[] { PhotonNetwork.NickName },
                                        new RaiseEventOptions { Receivers = ReceiverGroup.All },
                                        new SendOptions { Reliability = true }
                                    );

            initPlayer(3);
        }
    }
    public void ReadyToGame4()
    {
        if (!characterSelected)
        {
            
            PhotonNetwork.RaiseEvent(4,
                                        new object[] { PhotonNetwork.NickName },
                                        new RaiseEventOptions { Receivers = ReceiverGroup.All },
                                        new SendOptions { Reliability = true }
                                    );

            initPlayer(4);
        }
    }

    public void initPlayer(short _num)
    {
        characterSelected = true;
        GameObject player = PhotonNetwork.Instantiate("Player_" + _num, Vector3.zero, Quaternion.identity);
        player.transform.Find("PlayerName").GetComponent<TextMesh>().text = PhotonNetwork.NickName;
        mainCamera.GetComponent<CameraMovement>().player = player;

        if (readyPlayer == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            pv.RPC("StartGame", RpcTarget.AllBuffered);
        }
    }

    public void OnEvent(EventData _Event)
    {
        object[] data = (object[])_Event.CustomData;

        switch (_Event.Code)
        {
            case 0: //플레이어 Ready
                PLAYER_IS_READY[(int)data[0]] = true;
                break;
            case 1:
                readyPlayer++;
                b1.interactable = false;
                b1.GetComponentInChildren<TMP_Text>().text = data[0] + "\nSelected!";
                Ready();
                if (readyPlayer == PhotonNetwork.CurrentRoom.PlayerCount)
                {
                    pv.RPC("StartGame", RpcTarget.AllBuffered);
                }

                break;
            case 2:
                readyPlayer++;
                b2.interactable = false;
                b2.GetComponentInChildren<TMP_Text>().text = data[0] + "\nSelected!";
                Ready();
                if (readyPlayer == PhotonNetwork.CurrentRoom.PlayerCount)
                {
                    
                    pv.RPC("StartGame", RpcTarget.AllBuffered);
                }
                break;
            case 3:
                readyPlayer++;
                b3.interactable = false;
                b3.GetComponentInChildren<TMP_Text>().text = data[0] + "\nSelected!";
                Ready();
                if (readyPlayer == PhotonNetwork.CurrentRoom.PlayerCount)
                {
                    
                    pv.RPC("StartGame", RpcTarget.AllBuffered);
                }
                break;
            case 4:
                readyPlayer++;
                b4.interactable = false;
                b4.GetComponentInChildren<TMP_Text>().text = data[0] + "\nSelected!";
                Ready();
                if (readyPlayer == PhotonNetwork.CurrentRoom.PlayerCount)
                {
                    
                    pv.RPC("StartGame", RpcTarget.AllBuffered);
                }
                break;
            default:
                break;
        }

        return;
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

    [PunRPC]
    public void StartGame()
    {
        //각 클라이언트들이 만든 플레이어를 전부 찾아서
        GameObject[] pGO = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < pGO.Length; i++)
        {
            //Players 아래에 전부 넣어준다.
            pGO[i].transform.parent = players.transform;
            //그리고 주인의 이름을 달아준다.
            pGO[i].GetComponentInChildren<TextMesh>().text = pGO[i].GetComponent<PhotonView>().Owner.NickName;
        }

        characterSelectUI.SetActive(false);

        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }


    public void Ready()
    {
        PhotonNetwork.RaiseEvent(4,
                                        new object[] { PhotonNetwork.LocalPlayer.ActorNumber },
                                        new RaiseEventOptions { Receivers = ReceiverGroup.All },
                                        new SendOptions { Reliability = true }
                                    );
    }

    //[PunRPC]
    //public void Ready_RPC(int _who)
    //{
    //    PLAYER_IS_READY[_who] = true;
    //}
}
