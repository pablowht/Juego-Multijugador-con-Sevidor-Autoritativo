using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Chronometer : MonoBehaviour
{
    //Clase creada para apuntar el tiempo de cada coche
    private float elapsedTime = 0f; 
    private bool isRunning = true; //Booleano para poder parar el cronometro cuando sea necesario
    private TextMeshProUGUI chronometerText;

    //Elementos en la IU privados
    [Header("Lap Elements")]
    [SerializeField] private GameObject _lapTimesBackground; 
    [SerializeField] private TextMeshProUGUI _lapTime1;
    [SerializeField] private TextMeshProUGUI _lapTime2;
    [SerializeField] private TextMeshProUGUI _lapTime3;
    
    //ESTO QUE ES PABLO ELEFANTE
    public string[] _stringLapTimes = new string[3];

    private TextMeshProUGUI[] _TMPLapTimes = new TextMeshProUGUI[3]; 

    void Start()
    {
        //Inicializamos el array con las variables "TextMeshProUGUI" serializadas
        _TMPLapTimes[0] = _lapTime1;
        _TMPLapTimes[1] = _lapTime2;
        _TMPLapTimes[2] = _lapTime3;

        chronometerText = GetComponent<TextMeshProUGUI>();
        ResetChronometer(); 
    }

    void Update() //Metodo que modifica el cronometro con el paso del tiempo
    {
        if (isRunning) 
        {
            elapsedTime += Time.deltaTime;
            UpdateChronometerUI(); //Actualiza la UI
        }
    }

    private void ResetChronometer() //Resetear el cronometro
    {
        elapsedTime = 0f;
        UpdateChronometerUI();
    }

    public void StartChronometer() //Iniciar el cronometro
    {
        isRunning = true;
    }

    public void StopChronometer() //Parar el cronometro
    {
        isRunning = false;
    }

    public void UpdateLapTime(int lapNumber) //Actualizar el tiempo al dar la vuelta
    {
        //Comprueba si el gameobject está activo, si es false se activa
        if (!_lapTimesBackground.activeSelf) _lapTimesBackground.SetActive(true);

        // Capturar el tiempo actual del cronómetro
        string currentTime = GetFormattedTime(elapsedTime);

        
        _stringLapTimes[lapNumber] = currentTime;
        _TMPLapTimes[lapNumber].SetText(currentTime);
        _TMPLapTimes[lapNumber].gameObject.SetActive(true);
        Debug.Log($"Lap {lapNumber} time captured: {currentTime}"); //Muestra la vuelta y el tiempo en el que se ha dado esta
    }

    private void UpdateChronometerUI() //Actualiza el cronometro en la IU
    {
        string formattedTime = GetFormattedTime(elapsedTime);
        chronometerText.text = formattedTime;
    }

    private string GetFormattedTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60F);
        int seconds = Mathf.FloorToInt(time % 60F);
        int milliseconds = Mathf.FloorToInt((time * 100F) % 100F);

        return string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds); //Formato del cronometro
    }
}
