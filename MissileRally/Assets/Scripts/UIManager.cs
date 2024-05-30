using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    //private TMP_InputField _playerNameInput;
    public TextMeshProUGUI _raceCodeUI;
    private TextMeshProUGUI _userData;

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

    //private void Start()
    //{
    //_ui_Lobby = GameObject.FindGameObjectWithTag("UI_Lobby");
    //foreach (Transform child in _ui_Lobby.transform)
    //{
    //    if (child.tag == "RaceCode") _raceCodeInput = child.GetComponent<TMP_InputField>();
    //}

    //_ui_GameInfo = GameObject.FindGameObjectWithTag("UI_GameInfo");
    //foreach (Transform child in _ui_GameInfo.transform)
    //{
    //    if (child.tag == "RaceCode") _raceCodeUI = child.GetComponent<TextMeshProUGUI>();
    //    if (child.tag == "UserData") _userData = child.GetComponent<TextMeshProUGUI>();
    //}

    //_ui_Lobby.SetActive(true);
    //_ui_GameInfo.SetActive(false);
    //}

    //void OnGUI()
    //{
    //if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
    //{
    //    LobbyControlUI();
    //}
    //else
    //{
    //    StatusLabels();
    //}
    //}

    //private void LobbyControlUI()
    //{
    //    _ui_Lobby.SetActive(true);
    //    //RelayManager.Instance.joinCode = _raceCodeInput.ToString();
    //    //_raceCodeUI.SetText(RelayManager.Instance.joinCode);
    //}

    //private void FixedUpdate()
    //{
    //    //Crear un script aparte para el speedometer que active el gameobject al empezar la carrera y por tanto empiece su fixedupdate
    //    //if (carRaceOn) { updateSpeedometer(); }
    //}

    #region GameInfo UI

    [Header("Speedometer Info")]
    [SerializeField] public GameObject _speedometer;
    [SerializeField] private GameObject _speedometerNeedle;
    private float startNeedlePosition = 217.8f;
    private float endNeedlePosition = -39.4f;
    private float needlePosition;
    private float vehicleSpeed;
    //public CarController _carController;

    [Header("GameObjects Varios")]
    [SerializeField] public GameObject _chronometer;

    public void updateSpeedometer()
    {
        //print(GameManager.Instance.actualPlayerInfo.playerSpeed);
        //vehicleSpeed = GameManager.Instance.actualPlayerInfo.playerSpeed;
        //ELEFANTE ENORME GRANDISIMO
        vehicleSpeed = GameManager.Instance.actualPlayer.carController.Speed;
        needlePosition = startNeedlePosition - endNeedlePosition;
        float temp = vehicleSpeed / 60;
        _speedometerNeedle.transform.eulerAngles = new Vector3 (0, 0, (startNeedlePosition - temp * needlePosition));
        print("sale");
    }

    //private bool carRaceOn = false;
    [Header("Car Ready")]
    [SerializeField] private GameObject botonCarReady;
    [SerializeField] private GameObject _carReadyUI;
    [SerializeField] public TextMeshProUGUI _numberCarReadyUI;
    public void CarReadyForRace()
    {
        botonCarReady.GetComponent<Button>().interactable = false;
        _carReadyUI.SetActive(true);
        GameManager.Instance.ntGameInfo.IncrementCarReadyServerRpc();
        //GameManager.Instance.actualPlayer.IncrementCarReady();

    }

    public void DisableUIToStartRace()
    {
        _carReadyUI.SetActive(false);
        botonCarReady.SetActive(false);
        _speedometer.SetActive(true);
        _chronometer.SetActive(true);
    }

    #endregion

    #region Network Buttons
    private void StatusLabels()
    {
        //_ui_Lobby.SetActive(false);
        //_ui_GameInfo.SetActive(true);

        var mode = NetworkManager.Singleton.IsHost ?
            "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";

        _userData.SetText(mode + " : PlayerID");
    }

    public void StartHostButton(int mapNumber)
    {
        GameManager.Instance.mapScene = GameManager.Instance.mapasNombre[mapNumber];
        StartHostSequence(mapNumber, GameManager.Instance.mapScene);
    }
    private async void StartHostSequence(int mapNumber, string mapSceneName)
    {
        SceneManager.LoadSceneAsync(mapSceneName);
        await RelayManager.Instance.StartHost();
        GameManager.Instance.SetMapSelected(mapNumber);

        //GameManager.Instance.ConnectToRace();
        //_raceCodeUI.SetText(RelayManager.Instance.joinCode);
    }
    private async void StartClientButton()
    {
        await RelayManager.Instance.StartClient(_raceCodeInput.GetComponent<TMP_InputField>().text);
    }

    public void SetRaceCode()
    {
        //DUDA 1

        //StartClientButton();
        //ELEFANTE
        //GameManager.Instance.mapScene = GameManager.Instance.mapSelected.Value.ToString();
        //SceneManager.LoadSceneAsync(GameManager.Instance.mapasNombre[GameManager.Instance.mapaNumero.Value]);
        //GameManager.Instance.ConnectToRace();
        StartClientButton();
        //RelayManager.Instance.joinCode = _raceCodeInput.ToString();

    }

    public void SetVisibleRaceInput()
    {
        //RelayManager.Instance.StartClient(_raceCodeInput.text);
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

    public void SetPlayerName()
    {
        //if (_playerNameInput.text != "" && _playerNameInput.text != " ")
        if (!string.IsNullOrWhiteSpace(_playerNameInput.text.Trim()) && !_playerNameInput.text.Contains(" "))
        {
            GameManager.Instance.actualPlayerInfo.playerName = _playerNameInput.text;
            _playerNameUI.text = _playerNameInput.text;
            _playerNameInput.gameObject.SetActive(false);
            _fondoLobby.SetActive(false);
            _carSelectionUI.SetActive(true);
            if (_incorrectPlayerName.activeSelf) _incorrectPlayerName.SetActive(false);
        }
        else
        {
            _incorrectPlayerName.SetActive(true);
        }
    }

   

    public void CreateRaceButton()
    {
        _networkUI.SetActive(false);
        _mapSelectionUI.SetActive(true);
    }

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

    public void ChangeCarButton(int avanceNumber)
    {
        cars[indexCar].SetActive(false);
        if (avanceNumber == -1) indexCar = (indexCar + (avanceNumber) + cars.Length) % cars.Length;
        else indexCar = (indexCar + (avanceNumber)) % cars.Length;
        cars[indexCar].SetActive(true);
    }
    #endregion
}
