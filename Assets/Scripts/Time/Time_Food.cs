using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Slider = UnityEngine.UI.Slider;

public class Time_Food : MonoBehaviour
{
    private GameData _gameData;
    
    private bool inProgress;
    private DateTime TimerStart;
    private DateTime TimerEnd;

    private Coroutine lastTimer;
    private Coroutine lastDisplay;

    [Header("Production time")]
    public int Days;
    public int Hours;
    public int Minutes;
    public int Seconds;

    [Header("UI")]
    [SerializeField] private GameObject window;
    [SerializeField] private TextMeshProUGUI startTimetxt;
    [SerializeField] private TextMeshProUGUI endTimetxt;
    [SerializeField] private GameObject timeLeftObj;
    [SerializeField] private TextMeshProUGUI timeLeftText;
    [SerializeField] private Slider timeLeftSlider;
    [SerializeField] private Button skipButton;
    [SerializeField] private Button startButton;


    private void Awake()
    {
         SystemSave.Load(_gameData);
    }

    private void Start()
    {
        window.SetActive(false);
        startButton.onClick.AddListener(StartTimer);
        skipButton.onClick.AddListener(skip);

        if (!string.IsNullOrEmpty(SaveData.current.testData.timeStart))
        {
            TimerStart = DateTime.Parse(SaveData.current.testData.timeStart);
            TimerEnd = DateTime.Parse(SaveData.current.testData.timeEnd);
            lastTimer = StartCoroutine(Timer());
            inProgress = true;
        }
    }

    private void OnDestroy()
    {
        SystemSave.Save(_gameData);
        Debug.Log("saved");
    }

    private void InitializeWindow()
    {
        if (inProgress)
        {
            startTimetxt.text = "Start Time: \n" + TimerStart;
            endTimetxt.text = "End Time: \n" + TimerEnd;
            timeLeftObj.SetActive(true);
            lastDisplay=StartCoroutine(DisplayTime());
            startButton.gameObject.SetActive(false);
            skipButton.gameObject.SetActive(true);
        }
        else
        {
            startTimetxt.text = "Start Time: \n" ;
            endTimetxt.text = "End Time: \n";
        }
        
    }

    private IEnumerator DisplayTime()
    {
        DateTime start = DateTime.Now;
        TimeSpan timeLeft = TimerEnd - start;
        double totalSecondsLeft = timeLeft.TotalSeconds;
        double totalSeconds = (TimerEnd - TimerStart).TotalSeconds;
        string text;

        while (window.activeSelf && timeLeftObj.activeSelf)
        {
            text = "";
            timeLeftSlider.value=1-Convert.ToSingle((TimerEnd-DateTime.Now).TotalSeconds/totalSeconds);
            if (totalSecondsLeft>1)
            {
                if (timeLeft.Days != 0)
                {
                    text += timeLeft.Days + "d";
                    text += timeLeft.Hours + "h";
                    yield return new WaitForSeconds(timeLeft.Minutes*60);
                }
                else if (timeLeft.Hours != 0)
                {
                    text += timeLeft.Hours + "h";
                    text += timeLeft.Minutes + "m";
                    yield return new WaitForSeconds(timeLeft.Seconds);
                }else if (timeLeft.Minutes != 0)
                {
                    TimeSpan ts = TimeSpan.FromSeconds(totalSecondsLeft);
                    text += ts.Minutes + "m";
                    text += ts.Seconds + "s";
                }
                else
                {
                    text += Mathf.FloorToInt((float)(totalSecondsLeft)) + "s";
                }
                timeLeftText.text = text;
                totalSecondsLeft -= Time.deltaTime;
                yield return null;
            }
            else
            {
                timeLeftText.text = "fisnished";
                skipButton.gameObject.SetActive(true);
                inProgress = true;
                break;
            }

            yield return null;
        }
    }

    private void StartTimer()
    {
        TimerStart = DateTime.Now;
        TimeSpan time = new TimeSpan(Days, Hours, Minutes, Seconds);
        TimerEnd = TimerStart.Add(time);
        inProgress = true;

        SaveData.current.testData = new TestData(TimerStart, TimerEnd);
        
        lastTimer=StartCoroutine(Timer());
        InitializeWindow();
    }

    private IEnumerator Timer()
    {
        DateTime start = DateTime.Now;
        double secondsToFinished = (TimerEnd - start).TotalSeconds;
        yield return new   WaitForSeconds(Convert.ToSingle(secondsToFinished));

        inProgress = false;
        Debug.Log("Fisnished");
    }

    public void OpenWindow()
    {
        window.SetActive(true); 
        InitializeWindow(); 
    }

    public void CloseWindow()
    {
        window.SetActive(false);
    }

    private void skip()
    {
        TimerEnd = DateTime.Now;    
        inProgress = false;
        SaveData.current.testData.timeEnd = TimerEnd.ToString();
        StopCoroutine(lastTimer);
        timeLeftText.text = "Fisnished";
        timeLeftSlider.value = 1;
        StopCoroutine(lastDisplay);
        skipButton.gameObject.SetActive(false);
        startButton.gameObject.SetActive(true);
    }
}
