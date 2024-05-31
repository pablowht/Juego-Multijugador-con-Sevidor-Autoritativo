using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    //Referencia para el código de la carrera en la UI
    public TextMeshProUGUI _raceCodeUI;

    //Singleton individual para cada escena
    public static UIManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #region Ranking
    [Header("Ranking")]
    [SerializeField] private GameObject _rankingUI;
    [SerializeField] private GameObject _restartButton;
    [SerializeField] public GameObject _exitButton;
    [SerializeField] private GameObject _cenitalCamera;
    [SerializeField] private TextMeshProUGUI[] _playerList;
    public string rankingText; //String donde se guarda la posición final del player, su nombre y su tiempo

    //Método para abrir la interfaz del ranking, ocultar el speedometer y desactivar el movimiento de los players
    public void OpenRanking()
    {
        _rankingUI.SetActive(true);
        _speedometer.SetActive(false);
        GameManager.Instance.actualPlayer.DisablePlayerInput();
        GameManager.Instance.actualPlayer.car.SetActive(false);
        GameManager.Instance.actualPlayer.car.GetComponent<BoxCollider>().enabled = false;
        //Se cambia a una vista cenital para poder ver toda la carrera mientras acaban el resto de jugadores
        _cenitalCamera.SetActive(true);
    }
    //Metodo que guarda el tiempo de cada jugador en el ranking formateado
    public string RankingTextInRanking(int orderPlayer, string playerName, string tempo)
    {
        rankingText = string.Format("{0}. {1, -20} {2}", orderPlayer + 1, playerName, tempo);
        return rankingText;
    }
    //Metodo que escribe en la UI cada linea del ranking
    public void WriteRankingUI(int orderPlayer, string text)
    {
        _playerList[orderPlayer].SetText(text);
    }

    //Metodo que cierra la aplicación
    public void ExitApplication()
    {
        Application.Quit();
    }
    #endregion

    #region Race Order
    [Header("Race Order UI")]
    [SerializeField] private TextMeshProUGUI _numberCarPosition;
    //Actualiza el numero de la IU con la posición del cliente actual en la carrera
    public void UpdateCarOrderNumberUI(int position) 
    {
        _numberCarPosition.SetText(position.ToString());
    }
    #endregion


    #region GameInfo UI

    [Header("Speedometer Info")]
    [SerializeField] public GameObject _speedometer;
    [SerializeField] private GameObject _speedometerNeedle;
    private float startNeedlePosition = 217.8f;
    private float endNeedlePosition = -39.4f;
    private float needlePosition;
    private float vehicleSpeed;

    [Header("GameObjects Varios")]
    [SerializeField] public GameObject _chronometer;
    [SerializeField] public GameObject _semaphoreCamera;
    [SerializeField] public SemaphoreController _semaphore;

    //Idea sacada de: https://youtu.be/WKF_3BLD4-8?list=PLhWBaV_gmpGXxscZr8PIcreyYkw8VlnKn
    public void updateSpeedometer()
    {
        vehicleSpeed = GameManager.Instance.actualPlayer.carController.Speed;
        print(GameManager.Instance.actualPlayer.carController.Speed);
        needlePosition = startNeedlePosition - endNeedlePosition;
        float temp = vehicleSpeed / 60;
        _speedometerNeedle.transform.eulerAngles = new Vector3 (0, 0, (startNeedlePosition - temp * needlePosition));
    }

    [Header("Car Ready")]
    [SerializeField] public GameObject botonCarReady;
    [SerializeField] private GameObject _carReadyUI;
    [SerializeField] public TextMeshProUGUI _waitingPlayersText;
    [SerializeField] public TextMeshProUGUI _numberCarReadyUI;
    //Avisa que el coche está listo
    public void CarReadyForRace()
    {
        //Se desactiva el boton de ready
        botonCarReady.GetComponent<Button>().interactable = false; 
        //Muestra la interfaz del número de coches listos y el semáfoto
        _carReadyUI.SetActive(true); 
        _semaphoreCamera.SetActive(true);
        _semaphore.UpdateToRed(); //El semaforo se actualiza a rojo para resetear el color del material
        GameManager.Instance.ntGameInfo.IncrementCarReadyServerRpc(); //Incrementamos la variable de coches preparados en el servidor

    }

    public void DisableUIToStartRace() //Elimina las partes de la IU que no son necesarias para jugar y activa las que sí (cronometro y semaforo)
    {
        _carReadyUI.SetActive(false);
        botonCarReady.SetActive(false);
        _speedometer.SetActive(true);
        _chronometer.SetActive(true);
    }
    #endregion

    #region Network Buttons

    public void StartHostButton(string mapName) //Método que llama al método asíncrono del servidor
    {
        StartHostSequence(mapName);
    }
    //Se carga asincronamente la escena deseada y se inicia el servidor
    private async void StartHostSequence(string mapName) 
    {
        SceneManager.LoadSceneAsync(mapName);
        await RelayManager.Instance.StartHost();
    }
    //Se conecta un cliente con el texto que se pase por el InputField (Carga directamente la escena donde está el host)
    private async void StartClientButton()
    {
        await RelayManager.Instance.StartClient(_raceCodeInput.GetComponent<TMP_InputField>().text); 
    }
    //Se llama al terminar de escribir el código de la carrera, empezando el servidor
    public void SetRaceCode()
    {
        StartClientButton();
    }
    //Muestra por pantalla el InputField donde escribir el código de la carrera 
    public void SetVisibleRaceInput() 
    {
        _raceCodeInput.SetActive(true);
    }
    #endregion

    #region Lobby
    [Header("Lobby UI")]
    [SerializeField] private GameObject _fondoLobby;

    [Header("Lobby - Player")]
    [SerializeField] private TextMeshProUGUI _playerNameUI;
    [SerializeField] private TMP_InputField _playerNameInput;
    [SerializeField] private GameObject _incorrectPlayerName;

    [Header("Lobby - Network UI")]
    [SerializeField] private GameObject _networkUI;
    [SerializeField] private GameObject _raceCodeInput;

    [Header("Lobby - Car")]
    [SerializeField] private GameObject _carSelectionUI;
    
    [Header("Lobby - Map Selection")]
    [SerializeField] private GameObject _mapSelectionUI;

    public string joinCode = "Código...";

    //Se guarda el valor del nombre del jugador verificando que no tenga espacios en él
    public void SetPlayerName()
    {
        if (!string.IsNullOrWhiteSpace(_playerNameInput.text.Trim()) && !_playerNameInput.text.Contains(" "))
        {
            GameManager.Instance.actualPlayerInfo.playerName = _playerNameInput.text;
            _playerNameUI.text = _playerNameInput.text;
            _playerNameInput.gameObject.SetActive(false);
            _fondoLobby.SetActive(false);
            //Se activan las flechas de selección de coches
            _carSelectionUI.SetActive(true);
            if (_incorrectPlayerName.activeSelf) _incorrectPlayerName.SetActive(false);
        }
        else
        {
            _incorrectPlayerName.SetActive(true);
        }
    }
    //Método al que se llama al elegir crear una carrera y pasa a la selección de nivel
    public void CreateRaceButton()
    {
        _networkUI.SetActive(false);
        _mapSelectionUI.SetActive(true);
    }
    //Se guarda el coche elegido y se pasa a la siguiente pantalla de UI
    public void ReadyButton()
    {
        GameManager.Instance.actualPlayerInfo.playerCar = indexCar;
        _carSelectionUI.SetActive(false);
        _fondoLobby.SetActive(true);
        _networkUI.SetActive(true);
    }

    #endregion

    #region CarSelection
    [Header("List of Cars")]
    [SerializeField] private GameObject[] cars;

    private int indexCar = 0;
    //Mediante modulación se recorre una lista de coches para que cambien en pantalla y poder guardar el índice elegido
    public void ChangeCarButton(int avanceNumber)
    {
        cars[indexCar].SetActive(false);
        if (avanceNumber == -1) indexCar = (indexCar + (avanceNumber) + cars.Length) % cars.Length;
        else indexCar = (indexCar + (avanceNumber)) % cars.Length;
        cars[indexCar].SetActive(true);
    }
    #endregion
}
