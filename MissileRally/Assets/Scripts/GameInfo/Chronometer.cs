using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Chronometer : MonoBehaviour
{
    private float elapsedTime = 0f;
    private bool isRunning = true;
    private TextMeshProUGUI chronometerText;

    [Header("Lap Elements")]
    [SerializeField] private GameObject _lapTimesBackground;
    [SerializeField] private TextMeshProUGUI _lapTime1;
    [SerializeField] private TextMeshProUGUI _lapTime2;
    [SerializeField] private TextMeshProUGUI _lapTime3;
    public string[] _stringLapTimes = new string[3];
    private TextMeshProUGUI[] _TMPLapTimes = new TextMeshProUGUI[3];

    void Start()
    {
        _TMPLapTimes[0] = _lapTime1;
        _TMPLapTimes[1] = _lapTime2;
        _TMPLapTimes[2] = _lapTime3;

        chronometerText = GetComponent<TextMeshProUGUI>();
        ResetChronometer();
    }

    void Update()
    {
        if (isRunning)
        {
            elapsedTime += Time.deltaTime;
            UpdateChronometerUI();
        }
    }

    private void ResetChronometer()
    {
        elapsedTime = 0f;
        UpdateChronometerUI();
    }

    public void StartChronometer()
    {
        isRunning = true;
    }

    public void StopChronometer()
    {
        isRunning = false;
    }

    public void UpdateLapTime(int lapNumber)
    {
        print("entro a updateLapTime");
        //print("LapTime antes" + _stringLapTimes[lapNumber]);
        if (!_lapTimesBackground.activeSelf) _lapTimesBackground.SetActive(true);
        //_stringLapTimes[lapNumber] = chronometerText.text;
        //print("LapTime después" + _stringLapTimes[lapNumber]);
        //print("Chronometer " + chronometerText.text);
        //_TMPLapTimes[lapNumber].SetText(_stringLapTimes[lapNumber]);
        //_TMPLapTimes[lapNumber].gameObject.SetActive(true);

        // Capturar el tiempo actual del cronómetro
        string currentTime = GetFormattedTime(elapsedTime);

        _stringLapTimes[lapNumber] = currentTime;
        _TMPLapTimes[lapNumber].SetText(currentTime);
        _TMPLapTimes[lapNumber].gameObject.SetActive(true);
        Debug.Log($"Lap {lapNumber} time captured: {currentTime}");
    }

    private void UpdateChronometerUI()
    {
        string formattedTime = GetFormattedTime(elapsedTime);
        chronometerText.text = formattedTime;
    }

    private string GetFormattedTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60F);
        int seconds = Mathf.FloorToInt(time % 60F);
        int milliseconds = Mathf.FloorToInt((time * 100F) % 100F);

        return string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
    }
}
