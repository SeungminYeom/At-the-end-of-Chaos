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

        //플레이어 State Sync용
        PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "isReady", false } }); //플레이어 각각의 Ready 상태
        PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable { { "waitToSync", true} }); //현재 Sync를 위해 Wait중인가?
        PhotonNetwork.CurrentRoom.IsOpen = false; //방에 못들어오게 잠근다.
    }

    void Update()
    {

    }

    #region BUTTON_ACTION
    public void ReadyToGame1()
    {
        if (!characterSelected)
        {
            pv.RPC("SelectCharactor", RpcTarget.All, 1, PhotonNetwork.LocalPlayer.NickName);
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
    #endregion

    //플레이어 생성
    public void initPlayer(short _num)
    {
        characterSelected = true;
        GameObject player = PhotonNetwork.Instantiate("Player_" + _num, Vector3.zero, Quaternion.identity);
        //카메라는 current player를 따라가도록 설정
        mainCamera.GetComponent<CameraMovement>().player = player;
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        //현재 방의 상태가 Sync대기중이면서, 모든 플레이어가 Ready상태일때만 작동
        if ((bool)PhotonNetwork.CurrentRoom.CustomProperties["waitToSync"] && AllPlayerReady) 
        {
            switch (GameManager.instance.timeState)
            {
                case TimeState.none:
                    break;
                case TimeState.characterSelect:
                    pv.RPC("StartGame", RpcTarget.AllBuffered);
                    break;
                case TimeState.startPhase:
                    break;
                case TimeState.afternoon:
                    break;
                case TimeState.upgrade:
                    break;
                case TimeState.nightStart:
                    break;
                case TimeState.night:
                    break;
                case TimeState.nightEnd:
                    break;
                default:
                    break;
            }
        }
        //Debug.Log("CHANGED : " + targetPlayer.NickName + " -> " + changedProps.Keys);
        
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


    #region PRCs
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
    #endregion

    #region PLAYER_READY
    public bool AllPlayerReady
    {
        get {
            //모든 플레이어의 isReady가 true이면 true를 반환한다.
            if (Array.TrueForAll<bool>(WeReady, x => x.Equals(true))) return true;
            else return false;
        }
        private set { }
    }

    //접속중인 플레이어들의 목록을 뽑아서 isReady부분만 모아서 확인한다.
    public bool[] WeReady
    {
        get {
            playersReady = new bool[PhotonNetwork.CurrentRoom.PlayerCount];
            otherPlayerProps = new Hashtable[PhotonNetwork.CurrentRoom.PlayerCount];

            //한명한명 뽑아서 배열에 저장한다.
            for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
            {
                otherPlayerProps[i] = PhotonNetwork.PlayerList[i].CustomProperties;
                if (otherPlayerProps[i]["isReady"] == null) continue;
                playersReady[i] = (bool)otherPlayerProps[i]["isReady"];
            }

            //그리고 bool배열을 반환한다.
            return playersReady;
        }

        private set { }
        
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
    #endregion
}
