using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameServerManager : MonoBehaviour
{
    public static GameServerManager instance = null;
    public string[] playerNames = new string[4];

    public PhotonView pv;
    GameObject lobbyObject;
    GameObject mainCamera;

    GameObject players;

    public GameObject characterSelectUI;
    public Button b1, b2, b3, b4;
    bool characterSelected = false;
    byte readyPlayer = 0;

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
        lobbyObject = GameObject.Find("LobbyManager");
        mainCamera = GameObject.Find("Main Camera");
        players = GameObject.Find("Players");

        //로비의 있던 플레이어 이름을 갖고오고 로비는 없앤다.
        playerNames = lobbyObject.GetComponent<Lobby>().playerNames;
        Destroy(lobbyObject);

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

    public void ReadyToGame1()
    {
        initPlayer(1);
    }
    public void ReadyToGame2()
    {
        initPlayer(2);
    }
    public void ReadyToGame3()
    {
        initPlayer(3);
    }
    public void ReadyToGame4()
    {
        initPlayer(4);
    }

    public void initPlayer(short _num)
    {
        if (!characterSelected)
        {
            pv.RPC("SelectedButton", RpcTarget.All, _num);
            characterSelected = true;
            GameObject player = PhotonNetwork.Instantiate("Player_" + _num, Vector3.zero, Quaternion.identity);
            player.transform.parent = players.transform;
            player.transform.Find("PlayerName").GetComponent<TextMesh>().text = PhotonNetwork.NickName;
            mainCamera.GetComponent<CameraMovement>().player = player;
            

            Debug.Log(readyPlayer + " / " + PhotonNetwork.CurrentRoom.PlayerCount);
            if (readyPlayer == PhotonNetwork.CurrentRoom.PlayerCount)
            {
                pv.RPC("StartGame", RpcTarget.AllBuffered);
            }
        }
        
    }


    [PunRPC]
    public void SelectedButton(short _num)
    {
        switch (_num)
        {
            case 1:
                b1.interactable = false;
                b1.GetComponentInChildren<TMP_Text>().text = "Selected!";
                break;
            case 2:
                b2.interactable = false;
                b2.GetComponentInChildren<TMP_Text>().text = "Selected!";
                break;
            case 3:
                b3.interactable = false;
                b3.GetComponentInChildren<TMP_Text>().text = "Selected!";
                break;
            case 4:
                b4.interactable = false;
                b4.GetComponentInChildren<TMP_Text>().text = "Selected!";
                break;
            default:
                break;
        }
        readyPlayer++;
    }

    [PunRPC]
    public void StartGame()
    {
        for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {
            GameObject tPlayer = players.transform.GetChild(i).gameObject;
            tPlayer.GetComponentInChildren<TextMesh>().text = tPlayer.GetComponent<PhotonView>().Owner.NickName;
        }
        characterSelectUI.SetActive(false);
    }
}
