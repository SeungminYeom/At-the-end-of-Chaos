using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;
using static UnityEngine.Rendering.DebugUI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameServerManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public static GameServerManager instance = null;

    public PhotonView pv;
    GameObject lobbyObject;
    GameObject mainCamera;

    public GameObject players;

    //캐릭터 선택창 UI 변수
    public GameObject characterSelectUI;
    public UnityEngine.UI.Button b1, b2, b3, b4;
    bool characterSelected = false;

    //플레이어 준비 상태 저장용
    bool localReady;
    public bool[] playersReady;
    Hashtable[] otherPlayerProps;



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

    void Start()
    {
        PhotonNetwork.UseRpcMonoBehaviourCache = true;
        //필요한 오브젝트를 찾는다.
        mainCamera = GameObject.Find("Main Camera");

        //로비는 더이상 사용되지 않으므로 지운다.
        Destroy(GameObject.Find("LobbyManager"));
        var a = PhotonNetwork.PlayerList;

        //
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "isReady", false } });
        PhotonNetwork.CurrentRoom.IsOpen = false;
    }

    void Update()
    {

    }

    public void ReadyToGame1()
    {
        if (!characterSelected)
        {
            pv.RPC("SelectCharactor", RpcTarget.All, 11, PhotonNetwork.LocalPlayer.NickName);
            IReady = true;
            initPlayer(1);
        }
        
    }
    public void ReadyToGame2()
    {
        if (!characterSelected)
        {
            pv.RPC("SelectCharactor", RpcTarget.All, 2, PhotonNetwork.LocalPlayer.NickName);
            IReady = true;
            initPlayer(2);
        }
    }
    public void ReadyToGame3()
    {
        if (!characterSelected)
        {
            pv.RPC("SelectCharactor", RpcTarget.All, 3, PhotonNetwork.LocalPlayer.NickName);
            IReady = true;
            initPlayer(3);
        }
    }
    public void ReadyToGame4()
    {
        if (!characterSelected)
        {
            pv.RPC("SelectCharactor", RpcTarget.All, 4, PhotonNetwork.LocalPlayer.NickName);
            IReady = true;
            initPlayer(4);
        }
    }

    public void initPlayer(short _num)
    {
        characterSelected = true;
        GameObject player = PhotonNetwork.Instantiate("Player_" + _num, Vector3.zero, Quaternion.identity);
        player.transform.Find("PlayerName").GetComponent<TextMesh>().text = PhotonNetwork.NickName;
        mainCamera.GetComponent<CameraMovement>().player = player;
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        Debug.Log("CHANGED : " + targetPlayer.NickName + " -> " + changedProps.Keys);
        if (Array.TrueForAll<bool>(WeReady, x => x.Equals(true)))
        {
            pv.RPC("StartGame", RpcTarget.AllBuffered);
        }
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
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "isReady", false } });
        characterSelectUI.SetActive(false);
        GameManager.instance.timeState = TimeState.startPhase;
    }

    [PunRPC]
    public void SelectCharactor(int _num, string _nick)
    {
        switch (_num)
        {
            case 1:
                b1.interactable = false;
                b1.GetComponentInChildren<TMP_Text>().text = _nick + "\nSelected!";
                break;

            case 2:
                b2.interactable = false;
                b2.GetComponentInChildren<TMP_Text>().text = _nick + "\nSelected!";
                break;

            case 3:
                b3.interactable = false;
                b3.GetComponentInChildren<TMP_Text>().text = _nick + "\nSelected!";
                break;

            case 4:
                b4.interactable = false;
                b4.GetComponentInChildren<TMP_Text>().text = _nick + "\nSelected!";
                break;

            default:
                break;
        }

        return;
    }

    //접속중인 플레이어들의 목록을 뽑아서 isReady부분만 모아서 확인한다.
    public bool[] WeReady
    {
        get {
            playersReady = new bool[PhotonNetwork.CountOfPlayers];
            otherPlayerProps = new Hashtable[PhotonNetwork.CountOfPlayers];

            for (int i = 0; i < PhotonNetwork.CountOfPlayers; i++)
            {
                otherPlayerProps[i] = PhotonNetwork.PlayerList[i].CustomProperties;
                if (otherPlayerProps[i]["isReady"] == null) continue;
                playersReady[i] = (bool)otherPlayerProps[i]["isReady"];
            }

            //그리고 bool배열을 반환한다.
            return playersReady;
        }

        set { }
        
    }
    
    //나의 Ready상태를 반환
    public bool IReady
    {
        get {
            if (PhotonNetwork.LocalPlayer.CustomProperties["isReady"] == null) return false;
            return (bool)PhotonNetwork.LocalPlayer.CustomProperties["isReady"]; 
        }

        set {
            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable() { { "isReady", value} });
        }
    }

    //[PunRPC]
    //public void Ready_RPC(int _who)
    //{
    //    PLAYER_IS_READY[_who] = true;
    //}
}
