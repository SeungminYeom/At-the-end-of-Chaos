using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Timeline;
using UnityStandardAssets.CrossPlatformInput;

public enum TimeState
{
    none,
    characterSelect,
    startPhase,
    afternoon,
    upgrade,
    nightStart,
    night,
    nightEnd
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int trainSpeed = 3;
    public bool trainStarted = false;
    public float timec = 0;
    public GameObject trains;

    GameObject select_UI;
    GameObject timeUI_afternoon;
    GameObject timeUI_night;
    GameObject joystick;
    GameObject shootBtn;
    Light dirLight;
    Image timeUI_Afternoon_Image;
    Image timeUI_Night_Image;

    [SerializeField] int _stage = 0;

    [SerializeField] int _maxTrainCount = 5;
    [SerializeField] int _trainCount;
    [SerializeField] float _groundSpeed = 10f;
    [SerializeField] TimeState _timeState;

    [SerializeField] double stateStartTime;

    [Header("TimeStateValue")]
    [SerializeField] float timeAfternoonValue = 60f;
    [SerializeField] float timeUpgradeValue = 30f;
    [SerializeField] float timeNightValue = 120f;
    [SerializeField] float timeNightStartValue = 3f;
    [SerializeField] float timeNightEndValue = 3;
    [SerializeField] float timeStartPhase = 2;

    WaitForSeconds wfs_Afternoon;
    WaitForSeconds wfs_Upgrade;
    WaitForSeconds wfs_Night;
    WaitForSeconds wfs_NightStart;
    WaitForSeconds wfs_NightEnd;
    WaitForSeconds wfs_StartPhase;

    [Header("LightSetting")]
    [SerializeField] float dayLightColor = 5000f;
    [SerializeField] float nightLightColor = 20000f;
    [SerializeField] float dayLightIntensity = 2f;
    [SerializeField] float nightLightIntensity = 0.5f;

    [Header("ResourcesCollecting")]
    [SerializeField] int woodResource = 0;
    [SerializeField] int ironResource = 0;
    [SerializeField] int spawnValue = 10;
    public GameObject WoodResource;
    public GameObject IronResource;
    List<GameObject> resourcePool = new List<GameObject>();
    float mapScaleX = 60;
    float mapScaleZ = 15;
    int _seed;

    [SerializeField] float timeScale = 1f;

    Gun gun;

    IEnumerator spawnZombie;

    public int seed
    {
        get { return _seed; }
        set { _seed = value; }
    }

    public int stage
    {
        get { return _stage; }
        set { _stage = value; }
    }

    public int maxTrainCount
    {
        get { return _maxTrainCount; }
    }

    public int trainCount
    {
        get { return _trainCount; }
        set
        {

            //열차 추가
            if (value - 1 == _trainCount)
            {
                if (_trainCount >= maxTrainCount) return;
                GameObject.Find("TrainManager").gameObject.SendMessage("AddTrain", _trainCount);
                _trainCount = value;
            }
            //열차 감소
            else if (value + 1 == _trainCount)
            {
                if (_trainCount <= 0) return;
                GameObject.Find("TrainManager").gameObject.SendMessage("SubTrain", _trainCount);
                _trainCount = value;
            }
            else _trainCount = value;
        }
    }

    public float groundSpeed
    {
        get { return _groundSpeed; }
        set { _groundSpeed = value; }
    }

    public TimeState timeState
    {
        get { return _timeState; }
        set { _timeState = value; }
    }

    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
    }

    void Start()
    {
        dirLight = GameObject.Find("DirectionalLight").GetComponent<Light>();
        select_UI = GameObject.Find("StaticCanvas").transform.Find("Select_UI").gameObject;
        timeUI_afternoon = GameObject.Find("Canvas").transform.Find("Afternoon").gameObject;
        timeUI_night = GameObject.Find("Canvas").transform.Find("Night").gameObject;
        joystick = GameObject.Find("Canvas").transform.Find("Joystick").gameObject;
        shootBtn = GameObject.Find("Canvas").transform.Find("ShootBtn").gameObject;
        timeUI_Afternoon_Image = timeUI_afternoon.GetComponent<Image>();
        timeUI_Night_Image = timeUI_night.GetComponent<Image>();
        trainCount = 2;
        timeState = TimeState.none;
        StartCoroutine(LoadDelay());
        
        //재할당 회피용
        wfs_Afternoon = new WaitForSeconds(timeAfternoonValue);
        wfs_Upgrade = new WaitForSeconds(timeUpgradeValue);
        wfs_Night = new WaitForSeconds(timeNightValue);
        wfs_NightStart = new WaitForSeconds(timeNightStartValue);
        wfs_NightEnd = new WaitForSeconds(timeNightEndValue);
        wfs_StartPhase = new WaitForSeconds(timeStartPhase);

        spawnZombie = ZombieManager.instance.SpawnZombie();
        GameObject.Find("TrainManager").gameObject.SendMessage("SortTrain", trainCount - 1);
    }

    void Update()
    {
        //Time.timeScale = timeScale;
        //if (trainStarted)
        //{
        //    timec += Time.deltaTime;
        //    trains.transform.position = Vector3.Lerp(Vector3.zero, Vector3.right * 10 / (trainSpeed - timec), timec / trainSpeed);
        //    if (timec > trainSpeed)
        //    {
        //        trainStarted = false;
        //        timec = 0;
        //        trains.transform.position = Vector3.zero;
        //    }
        //}


        switch (timeState)
        {
            case TimeState.afternoon:
                timeUI_Afternoon_Image.fillAmount = (float)((stateStartTime - Time.time) / timeAfternoonValue);
                break;

            case TimeState.upgrade:
                break;

            case TimeState.nightStart:
                
                groundSpeed = Mathf.Lerp(0, 10f, (float)(Time.time - stateStartTime) / (timeNightStartValue - 1f));
                dirLight.colorTemperature = Mathf.Lerp(dayLightColor, nightLightColor,
                                                        (float)(Time.time - stateStartTime) / (timeNightStartValue - 1f));
                dirLight.intensity = Mathf.Lerp(dayLightIntensity, nightLightIntensity,
                                                (float)(Time.time - stateStartTime) / (timeNightStartValue - 1f));
                break;

            case TimeState.night:
                timeUI_Night_Image.fillAmount = (float)((stateStartTime - Time.time) / timeNightValue);
                break;

            case TimeState.nightEnd:
                groundSpeed = Mathf.Lerp(10, 0f, (float)(Time.time - stateStartTime) / (timeNightEndValue - 1));
                dirLight.colorTemperature = Mathf.Lerp(nightLightColor, dayLightColor,
                                                        (float)(Time.time - stateStartTime) / (timeNightEndValue - 1));
                dirLight.intensity = Mathf.Lerp(nightLightIntensity, dayLightIntensity,
                                                (float)(Time.time - stateStartTime) / (timeNightEndValue - 1));
                break;

            default:
                break;
        }
    }

    void SpawnResource()
    {
        GameObject parent = GameObject.Find("MasterGameObject");
        for (int i = 0; i < spawnValue; i++)
        {
            GameObject go;

            Vector3 spawnPos = new Vector3(new System.Random().Next((int)-mapScaleX, (int)mapScaleX), 0f, new System.Random().Next(2, (int)mapScaleZ));
            if (new System.Random().Next(0, 2) == 1) spawnPos.z *= -1;

            if (new System.Random().Next(2, 4) == 2)
            {
                go = Instantiate(WoodResource, spawnPos, Quaternion.identity);
            }
            else
            {
                go = Instantiate(IronResource, spawnPos, Quaternion.identity);
            }
            resourcePool.Add(go);
            go.transform.SetParent(parent.transform);
            seed += 10;
        }
    }

    public IEnumerator WaitDuration()
    {

        /* %%% 여기 있는 행동들은 해당 state가 시작되기 전에 호출됨 %%% */

        switch (timeState)
        {
            case TimeState.none:
                //호출될일 없음
                break;

            case TimeState.characterSelect:
                break;

            case TimeState.startPhase:
                select_UI.SetActive(false);
                yield return wfs_StartPhase;
                GameServerManager.instance.IReady = true;
                break;

            case TimeState.afternoon:
                CrossPlatformInputManager.SetButtonUp("Shoot");
                joystick.SetActive(true);
                groundSpeed = 0f;
                SpawnResource();
                TrainManager tm = GameObject.Find("TrainManager").GetComponent<TrainManager>();
                GameServerManager.instance.player.GetComponent<Gun>().Armoury(false);
                yield return wfs_Afternoon;
                GameServerManager.instance.IReady = true;
                break;

            case TimeState.upgrade:
                Debug.Log("A");
                CardManager.instance.ResetCard();
                CardManager.instance.EnableCard();
                joystick.SetActive(false);
                shootBtn.SetActive(false);
                timeUI_night.transform.SetAsLastSibling();
                timeUI_Afternoon_Image.fillAmount = 1f;
                timeUI_night.SetActive(false);
                timeUI_afternoon.SetActive(false);
                select_UI.SetActive(true);
                break;

            case TimeState.nightStart:
                yield return new WaitForSeconds(2f);
                select_UI.SetActive(false);
                timeUI_night.SetActive(true);
                timeUI_afternoon.SetActive(true);
                for (int i = 0; i < resourcePool.Count; i++)
                {
                    Destroy(resourcePool[i]);
                }
                resourcePool.Clear();

                TrainStart();
                stateStartTime = Time.time;
                yield return wfs_NightStart;
                GameServerManager.instance.IReady = true;
                break;

            case TimeState.night:
                stateStartTime = Time.time + timeNightValue;
                StartCoroutine(spawnZombie);
                joystick.SetActive(true);
                shootBtn.SetActive(true);
                stage++;
                GameServerManager.instance.player.GetComponent<Gun>().Armoury(true);
                yield return wfs_Night;
                GameServerManager.instance.IReady = true;
                break;

            case TimeState.nightEnd:
                stateStartTime = Time.time;
                joystick.SetActive(false);
                shootBtn.SetActive(false);
                groundSpeed = 10f;
                StopCoroutine(spawnZombie);
                ZombieManager.instance.DestroyZombies();
                ZombieManager.instance.StrongerZombies();
                timeUI_afternoon.transform.SetAsLastSibling();
                timeUI_Night_Image.fillAmount = 1f;
                yield return wfs_NightEnd;
                GameServerManager.instance.IReady = true;
                break;

            default:
                break;
        }
    }

    //IEnumerator FromAfternoonToUpgrade()
    //{
    //    stateStartTime = Time.time + timeAfternoonValue;
    //    yield return new WaitForSeconds(timeAfternoonValue);
    //    joystick.SetActive(false);
    //    shootBtn.SetActive(false);
    //    timeState = TimeState.upgrade;
    //    //Time.timeScale = 0;
    //    //占쏙옙占싱억옙占신?창占쏙옙 占쏙옙占쏙옙 占쏙옙占쏙옙 占쌘듸옙
    //    timeUI_night.transform.SetAsLastSibling();
    //    timeUI_Afternoon_Image.fillAmount = 1f;
    //    timeUI_night.SetActive(false);
    //    timeUI_afternoon.SetActive(false);
    //    select_UI.SetActive(true);
    //    StartCoroutine(FromUpgradeToNight());
    //}

    //IEnumerator FromUpgradeToNight()
    //{
    //    stateStartTime = Time.time + timeUpgradeValue;
    //    yield return new WaitForSeconds(timeUpgradeValue);
    //    timeState = TimeState.nightStart;
    //    //Time.timeScale = 1;
    //    //TrainManager tm = GameObject.Find("TrainManager").GetComponent<TrainManager>();
    //    //GameObject player = GameObject.Find("Player");
    //    //player.transform.parent = tm.trains[trainCount - 1].transform;
    //    timeUI_night.SetActive(true);
    //    timeUI_afternoon.SetActive(true);
    //    select_UI.SetActive(false);
    //    for (int i = 0; i < resourcePool.Count; i++)
    //    {
    //        Destroy(resourcePool[i]);
    //    }
    //    resourcePool.Clear();
    //    //player.transform.position = player.transform.parent.position + new Vector3(0, 2.5f, 0);

    //    StartCoroutine(NightStart());
    //}

    //public IEnumerator NightStart()
    //{
    //    TrainStart();
    //    stateStartTime = Time.time;
    //    yield return new WaitForSeconds(timeNightStartValue);
    //    StartCoroutine(spawnZombie);
    //    joystick.SetActive(true);
    //    shootBtn.SetActive(true);
    //    timeState = TimeState.night;
    //    stage++;
    //    StartCoroutine(FromNightToAfternoon());
    //}

    //IEnumerator FromNightToAfternoon()
    //{
    //    stateStartTime = Time.time + timeNightValue;
    //    yield return new WaitForSeconds(timeNightValue);
    //    joystick.SetActive(false);
    //    shootBtn.SetActive(false);
    //    timeState = TimeState.nightEnd;
    //    groundSpeed = 10f;
    //    //占쏙옙占싱억옙占신?창占쏙옙 占쏙옙占쏙옙 占쏙옙占쏙옙 占쌘듸옙
    //    timeUI_afternoon.transform.SetAsLastSibling();
    //    timeUI_Night_Image.fillAmount = 1f;
    //    StartCoroutine(NightEnd());
    //}

    //IEnumerator NightEnd()
    //{
    //    stateStartTime = Time.time;
    //    yield return new WaitForSeconds(timeNightEndValue);
    //    CrossPlatformInputManager.SetButtonUp("Shoot");
    //    StopCoroutine(spawnZombie);
    //    joystick.SetActive(true);
    //    //shootBtn.SetActive(true);
    //    timeState = TimeState.afternoon;
    //    groundSpeed = 0f;
    //    SpawnResource();
    //    TrainManager tm = GameObject.Find("TrainManager").GetComponent<TrainManager>();
    //    StartCoroutine(FromAfternoonToUpgrade());
    //}

    IEnumerator LoadDelay()
    {
        yield return new WaitForSeconds(0.1f);
        timeState = TimeState.characterSelect;
    }

    void TrainStart()
    {
        timec = 0;
        trainStarted = true;
    }

    public int SeedGenerate()
    {
        return new System.Random().Next(10000);
    }

    public void inCreaseResource(int _wood, int _iron)
    {
        Debug.Log("占쏙옙占쏙옙 : " + _wood + ", " + _iron);
        woodResource += _wood;
        ironResource += _iron;
    }

}
