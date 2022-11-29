using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Timeline;

public enum TimeState
{
    none, //로딩용 대기
    characterSelect, //캐릭터 선택창
    startPhase, //캐릭터 선택후 로딩용
    afternoon,
    upgrade,
    nightStart,
    night,
    nightEnd
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int seed;
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

    [Header("LightSetting")]
    [SerializeField] float dayLightColor = 5000f;
    [SerializeField] float nightLightColor = 20000f;
    [SerializeField] float dayLightIntensity = 2f;
    [SerializeField] float nightLightIntensity = 0.5f;

    [Header("ResourcesCollecting")]
    [SerializeField] int woodResource = 0;
    [SerializeField] int ironResource = 0;
    public GameObject WoodResource;
    public GameObject IronResource;
    float mapScaleX = 60;
    float mapScaleZ = 15;

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
            if (value - 1 == _trainCount)
            {
                if (_trainCount >= maxTrainCount) return;
                GameObject.Find("TrainManager").gameObject.SendMessage("AddTrain", _trainCount);
                _trainCount = value;
            }
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
        select_UI = GameObject.Find("Canvas").transform.Find("Select_UI").gameObject;
        timeUI_afternoon = GameObject.Find("Canvas").transform.Find("Afternoon").gameObject;
        timeUI_night = GameObject.Find("Canvas").transform.Find("Night").gameObject;
        joystick = GameObject.Find("Canvas").transform.Find("Joystick").gameObject;
        shootBtn = GameObject.Find("Canvas").transform.Find("ShootBtn").gameObject;
        timeUI_Afternoon_Image = timeUI_afternoon.GetComponent<Image>();
        timeUI_Night_Image = timeUI_night.GetComponent<Image>();
        trainCount = 2;

        //GameServer가 준비되기 전에 넘어가는 문제가 있어서 잠시 기다렸다가 넘어간다.
        timeState = TimeState.none;
        StartCoroutine(LoadDelay());

        GameObject.Find("TrainManager").gameObject.SendMessage("SortTrain", trainCount - 1);
    }

    void Update()
    {
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
            case TimeState.startPhase:
                timeState = TimeState.nightStart;
                StartCoroutine(NightStart());
                break;

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
        //2 ~ range(15)
        Vector3 spawnPos = new Vector3(Random.Range(mapScaleX, -mapScaleX), 0f, Random.Range(2f, mapScaleZ));
        if (Random.Range(0, 2) == 1)
        {
            spawnPos.z *= -1;
        }

        //반복문 범위 조정
        for (int i = 0; i < 10; i++)
        {
            Instantiate(WoodResource, spawnPos, Quaternion.identity);
        }
    }

    IEnumerator FromAfternoonToUpgrade()
    {
        stateStartTime = Time.time + timeAfternoonValue;
        yield return new WaitForSeconds(timeAfternoonValue);
        joystick.SetActive(false);
        shootBtn.SetActive(false);
        timeState = TimeState.upgrade;
        //Time.timeScale = 0;
        //하이어라키 창의 순서 변경 코드
        timeUI_night.transform.SetAsLastSibling();
        timeUI_Afternoon_Image.fillAmount = 1f;
        timeUI_night.SetActive(false);
        timeUI_afternoon.SetActive(false);
        select_UI.SetActive(true);
        StartCoroutine(FromUpgradeToNight());
    }

    IEnumerator FromUpgradeToNight()
    {
        stateStartTime = Time.time + timeUpgradeValue;
        yield return new WaitForSeconds(timeUpgradeValue);
        timeState = TimeState.nightStart;
        //Time.timeScale = 1;
        //TrainManager tm = GameObject.Find("TrainManager").GetComponent<TrainManager>();
        //GameObject player = GameObject.Find("Player");
        //player.transform.parent = tm.trains[trainCount - 1].transform;
        timeUI_night.SetActive(true);
        timeUI_afternoon.SetActive(true);
        select_UI.SetActive(false);
        //player.transform.position = player.transform.parent.position + new Vector3(0, 2.5f, 0);

        StartCoroutine(NightStart());
    }

    IEnumerator NightStart()
    {
        TrainStart();
        stateStartTime = Time.time;
        yield return new WaitForSeconds(timeNightStartValue);
        StartCoroutine(ZombieManager.instance.SpawnZombie());
        joystick.SetActive(true);
        shootBtn.SetActive(true);
        timeState = TimeState.night;
        stage++;
        StartCoroutine(FromNightToAfternoon());
    }

    IEnumerator FromNightToAfternoon()
    {
        stateStartTime = Time.time + timeNightValue;
        yield return new WaitForSeconds(timeNightValue);
        joystick.SetActive(false);
        shootBtn.SetActive(false);
        timeState = TimeState.nightEnd;
        groundSpeed = 10f;
        //하이어라키 창의 순서 변경 코드
        timeUI_afternoon.transform.SetAsLastSibling();
        timeUI_Night_Image.fillAmount = 1f;
        StartCoroutine(NightEnd());
    }

    IEnumerator NightEnd()
    {
        stateStartTime = Time.time;
        yield return new WaitForSeconds(timeNightEndValue);
        StopCoroutine(ZombieManager.instance.SpawnZombie());
        joystick.SetActive(true);
        //shootBtn.SetActive(true);
        timeState = TimeState.afternoon;
        groundSpeed = 0f;
        SpawnResource();
        TrainManager tm = GameObject.Find("TrainManager").GetComponent<TrainManager>();
        StartCoroutine(FromAfternoonToUpgrade());
    }

    IEnumerator LoadDelay()
    {
        yield return new WaitForSeconds(0.5f);
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

}
