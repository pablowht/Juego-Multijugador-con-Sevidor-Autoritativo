using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI _raceCodeUI;

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
    [SerializeField] private GameObject _exitButton;
    [SerializeField] private GameObject _cenitalCamera;
    [SerializeField] private TextMeshProUGUI[] _playerList;
    public string rankingText;

    public void OpenRanking()
    {
        _rankingUI.SetActive(true);
        //WritePlayerInRanking(orderPlayer);
        _speedometer.SetActive(false);
        GameManager.Instance.actualPlayer.DisablePlayerInput();
        GameManager.Instance.actualPlayer.car.SetActive(false);
        GameManager.Instance.actualPlayer.car.GetComponent<BoxCollider>().enabled = false;

        _cenitalCamera.SetActive(true);
    }

    public string RankingTextInRanking(int orderPlayer, string playerName, string tempo)
    {
        rankingText = string.Format("{0}. {1, -20} {2}", orderPlayer + 1, playerName, tempo);
        return rankingText;
    }

    public void WriteRankingUI(int orderPlayer, string text)
    {
        _playerList[orderPlayer].SetText(text);
        print("Cliente estás ashí");
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    public void ExitApplication()
    {
        Application.Quit();
    }

    #endregion

    #region Race Order
    [Header("Race Order UI")]
    [SerializeField] private TextMeshProUGUI _numberCarPosition;

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
    //public CarController _carController;

    [Header("GameObjects Varios")]
    [SerializeField] public GameObject _chronometer;
    [SerializeField] public GameObject _semaphoreCamera;
    [SerializeField] public SemaphoreController _semaphore;



    public void updateSpeedometer()
    {
        //print(GameManager.Instance.actualPlayerInfo.playerSpeed);
        //vehicleSpeed = GameManager.Instance.actualPlayerInfo.playerSpeed;
        //ELEFANTE ENORME GRANDISIMO
        vehicleSpeed = GameManager.Instance.actualPlayer.carController.Speed;
        needlePosition = startNeedlePosition - endNeedlePosition;
        float temp = vehicleSpeed / 60;
        _speedometerNeedle.transform.eulerAngles = new Vector3 (0, 0, (startNeedlePosition - temp * needlePosition));
    }

    //private bool carRaceOn = false;
    [Header("Car Ready")]
    [SerializeField] public GameObject botonCarReady;
    [SerializeField] private GameObject _carReadyUI;
    [SerializeField] public TextMeshProUGUI _waitingPlayersText;
    [SerializeField] public TextMeshProUGUI _numberCarReadyUI;
    public void CarReadyForRace()
    {
        botonCarReady.GetComponent<Button>().interactable = false;
        _carReadyUI.SetActive(true);
        _semaphoreCamera.SetActive(true);
        _semaphore.UpdateToRed();
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

    public void StartHostButton(string mapName)
    {
        StartHostSequence(mapName);
    }
    private async void StartHostSequence(string mapName)
    {
        SceneManager.LoadSceneAsync(mapName);
        await RelayManager.Instance.StartHost();
    }
    private async void StartClientButton()
    {
        await RelayManager.Instance.StartClient(_raceCodeInput.GetComponent<TMP_InputField>().text);
    }

    public void SetRaceCode()
    {
        StartClientButton();
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

    public string joinCode = "Código...";


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
