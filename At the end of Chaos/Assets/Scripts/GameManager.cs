using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TimeState
{
    afternoon,
    upgrade,
    nightStart,
    night,
    nightEnd
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

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
        timeState = TimeState.nightStart;
        trainCount = 2;
        GameObject.Find("TrainManager").gameObject.SendMessage("SortTrain", trainCount - 1);
        StartCoroutine(FromUpgradeToNight());
    }

    void Update()
    {
        switch(timeState)
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
        shootBtn.SetActive(true);
        timeState = TimeState.afternoon;
        groundSpeed = 0f;
        TrainManager tm = GameObject.Find("TrainManager").GetComponent<TrainManager>();
        GameObject player = GameObject.Find("Player_1");
        player.transform.parent = null;
        player.transform.position = new Vector3(0, 0.01f, -1.5f);
        StartCoroutine(FromAfternoonToUpgrade());
    }
}
