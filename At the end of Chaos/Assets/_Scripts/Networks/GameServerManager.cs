using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.UI;
using UnityEngine.UI;
using WebSocketSharp;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class GameServerManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public static GameServerManager instance = null;

    public PhotonView pv;
    GameObject mainCamera;
    public GameObject player;

    public GameObject players;

    //캐릭터 선택창 UI 변수
    public GameObject characterSelectUI;
    public UnityEngine.UI.Button b1, b2, b3, b4;
    bool characterSelected = false;

    public GameObject pauseUI;

    //플레이어 준비 상태 저장용
    public bool[] playersReady;
    Hashtable[] otherPlayerProps;

    GameObject damageDisplayer;
    float returnTimeScale;
    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            pv.RPC("ResumeGame", RpcTarget.AllBuffered);
        } else
        {
            pv.RPC("PauseGame", RpcTarget.AllBuffered);
        }
        
    }
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

        characterSelectUI.SetActive(true);
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
        player = PhotonNetwork.Instantiate("Player_" + _num, Vector3.zero, Quaternion.identity);
        //카메라는 current player를 따라가도록 설정
        mainCamera.GetComponent<CameraMovement>().player = player;

        //소리는 자신이 기준으로 되야 하므로 자신의 플레이어에만 AudioListener를 추가시켜준다.
        player.AddComponent<AudioListener>();

    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        //현재 방의 상태가 Sync대기중이면서, 모든 플레이어가 Ready상태일때만 작동
        if ((bool)PhotonNetwork.CurrentRoom.CustomProperties["waitToSync"] && AllPlayerReady) 
        {
            IReady = false;

            /* %%% 여기 있는 행동들은 해당 state가 종료된 다음에 호출됨 %%% */

            switch (GameManager.instance.timeState)
            {
                case TimeState.none:
                    //시작시에만 사용됨
                    break;

                case TimeState.characterSelect:
                    GameManager.instance.timeState = TimeState.startPhase;
                    break;

                case TimeState.startPhase:
                    //각 클라이언트들이 만든 플레이어를 전부 찾아서
                    GameObject[] pGO = GameObject.FindGameObjectsWithTag("Player");

                    for (int i = 0; i < pGO.Length; i++)
                    {
                        //Players 아래에 전부 넣어준다.
                        pGO[i].transform.SetParent(players.transform);
                        //그리고 주인의 이름을 달아준다.
                        pGO[i].GetComponentInChildren<TMP_Text>().text = pGO[i].GetComponent<PhotonView>().Owner.NickName;
                        Debug.Log(pGO[i].GetComponent<PhotonView>().Owner.NickName);
                    }
                    PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "isReady", false } });
                    characterSelectUI.SetActive(false);
                    GameManager.instance.timeState = TimeState.nightStart;
                    break;

                case TimeState.afternoon:
                    GameManager.instance.timeState = TimeState.upgrade;
                    break;

                case TimeState.upgrade:
                    GameManager.instance.timeState = TimeState.nightStart;
                    break;

                case TimeState.nightStart:
                    GameManager.instance.timeState = TimeState.night;
                    break;

                case TimeState.night:
                    GameManager.instance.timeState = TimeState.nightEnd;
                    break;

                case TimeState.nightEnd:
                    GameManager.instance.timeState = TimeState.afternoon;
                    break;

                default:
                    break;
            }

            //위에서 정해준 State를 가지고 Manager에서 처리
            GameManager.instance.StartCoroutine(GameManager.instance.WaitDuration());
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
    public void SelectCharactor(int _num, string _nick)
    {
        switch (_num)
        {
            case 1:
                b1.interactable = false;
                StartCoroutine(CloseShutter(b1.gameObject));
                break;

            case 2:
                b2.interactable = false;
                StartCoroutine(CloseShutter(b2.gameObject));
                break;

            case 3:
                b3.interactable = false;
                StartCoroutine(CloseShutter(b3.gameObject));
                break;

            case 4:
                b4.interactable = false;
                StartCoroutine(CloseShutter(b4.gameObject));
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

    IEnumerator CloseShutter(GameObject _obj)
    {
        Debug.Log("Cor");
        RectTransform rt = b1.transform.GetChild(0).GetComponent<RectTransform>();
        Vector2 vec = new Vector2(rt.offsetMax.x, -650);
        Vector2 vec2 = new Vector2(rt.offsetMax.x, 650);
        
        float dtime = 0;
        float fadeTime = 2f;

        while (dtime <= fadeTime)
        {
            dtime += Time.deltaTime / fadeTime;
            vec.x = vec2.x = rt.offsetMax.x;
            var a = rt.gameObject.transform.position;
            rt.gameObject.transform.position = Vector3.down;
            yield return null;
        }
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

    [PunRPC]
    void PauseGame()
    {
        pauseUI.SetActive(true);
        returnTimeScale = Time.timeScale;
        Time.timeScale = 0.001f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        //Time.fixedDeltaTime = 0;
    }

    [PunRPC]
    void ResumeGame()
    {
        pauseUI.SetActive(false);
        //Time.timeScale = 1;
        Time.timeScale = returnTimeScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        
    }
}
