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

    public GameObject select_UI;
    public GameObject timeUI_afternoon;
    public GameObject timeUI_night;
    Image timeUI_Afternoon_Image;
    Image timeUI_Night_Image;

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


    public int trainCount
    {
        get { return _trainCount; }
        set
        {
            if (value - 1 == _trainCount)
            {
                GameObject.Find("TrainManager").gameObject.SendMessage("AddTrain", _trainCount);
                _trainCount = value;
            }
            else if (value + 1 == _trainCount)
            {
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
        timeUI_Afternoon_Image = timeUI_afternoon.GetComponent<Image>();
        timeUI_Night_Image = timeUI_night.GetComponent<Image>();
        timeState = TimeState.upgrade;
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
                groundSpeed = Mathf.Lerp(groundSpeed, 10f, Time.deltaTime);
                break;
            case TimeState.night:
                timeUI_Night_Image.fillAmount = (float)((stateStartTime - Time.time) / timeNightValue);
                break;
            case TimeState.nightEnd:
                groundSpeed = Mathf.Lerp(groundSpeed, 0f, Time.deltaTime);
                break;
        }
    }

    IEnumerator FromAfternoonToUpgrade()
    {
        stateStartTime = Time.time + timeAfternoonValue;
        yield return new WaitForSecondsRealtime(timeAfternoonValue);
        timeState = TimeState.upgrade;
        Time.timeScale = 0;
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
        yield return new WaitForSecondsRealtime(timeUpgradeValue);
        timeState = TimeState.nightStart;
        Time.timeScale = 1;
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
        stateStartTime = Time.time + timeNightValue;
        yield return new WaitForSecondsRealtime(timeNightStartValue);
        timeState = TimeState.night;
        StartCoroutine(FromNightToAfternoon());
    }

    IEnumerator FromNightToAfternoon()
    {
        stateStartTime = Time.time + timeNightValue;
        yield return new WaitForSecondsRealtime(timeNightValue);
        timeState = TimeState.nightEnd;
        groundSpeed = 10f;
        timeUI_afternoon.transform.SetAsLastSibling();
        timeUI_Night_Image.fillAmount = 1f;
        StartCoroutine(NightEnd());
    }

    IEnumerator NightEnd()
    {
        stateStartTime = Time.time + timeNightValue;
        yield return new WaitForSecondsRealtime(timeNightEndValue);
        timeState = TimeState.afternoon;
        groundSpeed = 0f;
        TrainManager tm = GameObject.Find("TrainManager").GetComponent<TrainManager>();
        GameObject player = GameObject.Find("Player");
        player.transform.parent = null;
        player.transform.position = new Vector3(0, 0.5f, -1.5f);
        StartCoroutine(FromAfternoonToUpgrade());
    }
}
