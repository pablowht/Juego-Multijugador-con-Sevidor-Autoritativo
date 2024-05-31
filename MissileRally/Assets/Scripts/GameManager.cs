using Cinemachine;
using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int numPlayers = 6;
    public int connectedPlayers = 0;

    public RaceController currentRace;
    public CircuitController currentCircuit;
    public Chronometer _chronometer;

    public CinemachineVirtualCamera virtualCamera;

    public GameObject prefabPlayer;

    public NetworkManager networkManager;

    public PlayerInfo actualPlayerInfo;
    public Player actualPlayer;

    public string mapScene;
    public string joinCodeNumber;

    public NetworkGameManager ntGameInfo;

    //public NetworkVariable<int> carsReadyToRace_ntw = new NetworkVariable<int>(0);
    public int carsReadyToRace = 0;
    public static GameManager Instance { get; private set; }

    public int finishedPlayers = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        actualPlayerInfo = new PlayerInfo();

        networkManager = NetworkManager.Singleton;
        networkManager.OnServerStarted += OnServerStarted;
        networkManager.OnClientConnectedCallback += OnClientConnected;
        networkManager.OnClientDisconnectCallback += OnClientDisconnected;

        ntGameInfo = transform.GetChild(0).GetComponent<NetworkGameManager>();
    }

    private void OnDestroy()
    {
        networkManager.OnServerStarted -= OnServerStarted;
        networkManager.OnClientConnectedCallback -= OnClientConnected;
        networkManager.OnClientDisconnectCallback -= OnClientDisconnected;

    }

    public void BeginRace()
    {
        UIManager.Instance._waitingPlayersText.SetText("Race Starting...");
        StartCoroutine(UpdateSemaphoreOrange());
    }

    IEnumerator StopSemaphore()
    {
        yield return new WaitForSeconds(1);
        print("SALIENDOOOOOOOOOOOOOOOOOOOO");
        UIManager.Instance._semaphoreCamera.SetActive(false);
    }

    IEnumerator UpdateSemaphoreOrange()
    {
        UIManager.Instance._semaphore.UpdateToRed();
        yield return new WaitForSeconds(2);
        UIManager.Instance._semaphore.UpdateToOrange();
        StartCoroutine(UpdateSemaphoreGreen());
    }
    IEnumerator UpdateSemaphoreGreen()
    {
        yield return new WaitForSeconds(2);
        UIManager.Instance._semaphore.UpdateToGreen();
        //ntGameInfo.activateInputClientRpc(); // ESTO QUIZäs HAY QUE ACTIVARLO no lo he llegado a probar, pero con él funciona todo bien
        EnablePlayerInputs();
        UIManager.Instance.DisableUIToStartRace();
        StartCoroutine(StopSemaphore());
    }

    public void EnablePlayerInputs()
    {
        //print("numero readys: " + carsReadyToRace);

        //print(NetworkManager.Singleton.SpawnManager.SpawnedObjectsList.Count);
        
        //ELEFANTE - DUDA: no entiendo porque motivo solo se activa el server!!!
        foreach (var networkObject in NetworkManager.Singleton.SpawnManager.SpawnedObjectsList)
        {
            //print("fuera " + networkObject.name);

            // Verificamos si el objeto tiene el componente Player
            if (networkObject.TryGetComponent<Player>(out var player))
            {
                //print("dentro" + player.name);
                player.EnablePlayerInput();
            }
        }
    }

    //bool cocheEnCarrera = false;
    //private void Update()
    //{
    //    //EVENTO para gestionar mejor lo siguiente:
    //    //if ((SceneManager.GetActiveScene().name == mapScene) && !cocheEnCarrera)
    //    //{
    //    //    print("Hola");
    //    //    currentCircuit = GameObject.FindGameObjectWithTag("CircuitManager").GetComponent<CircuitController>();
    //    //    currentRace = GameObject.FindGameObjectWithTag("CircuitManager").GetComponent<RaceController>();
    //    //    virtualCamera = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();


    //    //    ConnectToRace();
    //    //    cocheEnCarrera = true;
    //    //}
    //    print("Local: " + mapScene);
    //    //print("coches readys: " + carsReadyToRace);
    //    //print("Network: "+ mapSelected.Value);
    //}

    #region Network
    private void OnServerStarted()
    {
        print("El servidor está listo");
    }

    private void OnClientDisconnected(ulong obj)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            connectedPlayers--;
            ntGameInfo.removeCarServerRpc();

            //PENDIENTE - ELEFANTE: disminuir variable de personajes listos
        }
    }

    public string nombrePlayer;
    private void OnClientConnected(ulong obj)
    {
        StartCoroutine(WaitTillSceneLoaded());
        ConnectToRace();
        

        if (NetworkManager.Singleton.IsServer)
        {
            Transform playerStartingPosition = currentCircuit._playersPositions[connectedPlayers].transform;
            var player = Instantiate(prefabPlayer, playerStartingPosition);
            player.GetComponent<NetworkObject>().SpawnAsPlayerObject(obj);

            //currentRace.AddPlayer(player.GetComponent<Player>());
            
            //actualPlayer = player.GetComponent<Player>();
            connectedPlayers++;
        }
    }
    private IEnumerator WaitTillSceneLoaded()
    {
        yield return new WaitUntil(() => SceneManager.GetActiveScene().isLoaded);
    }


    public Material[] coloresMaterial;


    public void ConnectToRace()
    {
        currentCircuit = GameObject.FindGameObjectWithTag("CircuitManager").GetComponent<CircuitController>();
        virtualCamera = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
        currentRace = GameObject.FindGameObjectWithTag("CircuitManager").GetComponent<RaceController>();
        ntGameInfo.checkpoints = GameObject.FindWithTag("Checkpoint").GetComponent<FindCheckPoints>().points;
        _chronometer = UIManager.Instance._chronometer.GetComponent<Chronometer>();
        UIManager.Instance._raceCodeUI.SetText(UIManager.Instance.joinCode);
        prefabPlayer = networkManager.NetworkConfig.Prefabs.Prefabs[0].Prefab;
    }
    #endregion

}