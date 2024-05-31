using Cinemachine;
using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //Maximo número de players que se pueden conectar
    public int numPlayers = 6;
    //Players Conectados
    public int connectedPlayers = 0;
    //Players Preparados para comenzar la carrera
    public int carsReadyToRace = 0;
    //Players que han acabado la carrera
    public int finishedPlayers = 0;

    //Referencias de cada circuito
    public RaceController currentRace;
    public CircuitController currentCircuit;
    public Chronometer _chronometer;

    //Variables para el jugador
    public GameObject prefabPlayer;
    public Player actualPlayer;
    public PlayerInfo actualPlayerInfo;
    public CinemachineVirtualCamera virtualCamera;
    public Material[] coloresMaterial;

    //Referencia al NetworkManager para la suscripción a metodos
    public NetworkManager networkManager;
    //Referencia al NetworkGameManager
    public NetworkGameManager ntGameInfo;

    // Singleton DontDestroy
    public static GameManager Instance { get; private set; }
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
        //Búsqueda de referencias y suscripción a eventos de red
        actualPlayerInfo = new PlayerInfo();

        networkManager = NetworkManager.Singleton;
        networkManager.OnServerStarted += OnServerStarted;
        networkManager.OnClientConnectedCallback += OnClientConnected;
        networkManager.OnClientDisconnectCallback += OnClientDisconnected;

        ntGameInfo = transform.GetChild(0).GetComponent<NetworkGameManager>();
    }

    private void OnDestroy()
    {
        //Desuscripción de eventos de red
        networkManager.OnServerStarted -= OnServerStarted;
        networkManager.OnClientConnectedCallback -= OnClientConnected;
        networkManager.OnClientDisconnectCallback -= OnClientDisconnected;

    }

    public void BeginRace()
    {
        //Se actualiza el mensaje del Waiting players para avisar que comienza la carrera
        UIManager.Instance._waitingPlayersText.SetText("Race Starting...");
        //Corutina para cambiar el color del semáforo
        StartCoroutine(UpdateSemaphoreOrange());
    }

    IEnumerator StopSemaphore()
    {
        yield return new WaitForSeconds(1);
        //Se desactiva la cámara del semáforo para que no estorbe la visión
        UIManager.Instance._semaphoreCamera.SetActive(false);
    }

    IEnumerator UpdateSemaphoreOrange()
    {
        //Se cambia antes de empezar el color del material a rojo para evitar problemas por el guardado de materiales
        UIManager.Instance._semaphore.UpdateToRed();
        yield return new WaitForSeconds(2);
        UIManager.Instance._semaphore.UpdateToOrange();
        StartCoroutine(UpdateSemaphoreGreen());
    }
    IEnumerator UpdateSemaphoreGreen()
    {
        yield return new WaitForSeconds(2);
        UIManager.Instance._semaphore.UpdateToGreen();
        //Se activa el Player Input de todos los jugadores instanciados para que puedan moverse
        EnablePlayerInputs();
        //Se desactiva la UI de espera de race para cambiar a la de comienzo
        UIManager.Instance.DisableUIToStartRace();
        StartCoroutine(StopSemaphore());
    }

    public void EnablePlayerInputs()
    {
        //Se recorre la lista de objetos instanciados y, si tienen componente Player, se activa su Input
        foreach (var networkObject in NetworkManager.Singleton.SpawnManager.SpawnedObjectsList)
        {
            // Verificamos si el objeto tiene el componente Player
            if (networkObject.TryGetComponent<Player>(out var player))
            {
                player.EnablePlayerInput();
            }
        }
    }

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
        }
    }

    public string nombrePlayer; // Para depurar el nombre del player de cada cliente

    private void OnClientConnected(ulong obj)
    {
        //Corutina para esperar a que la escena esté cargada para buscar todas las referencias necesarias
        StartCoroutine(WaitTillSceneLoaded());
        //Búsqueda de referencias y asignación del prefab
        ConnectToRace();

        //El server se encarga de instanciar al jugador
        if (NetworkManager.Singleton.IsServer)
        {
            //Se elije la posición en la carrera donde se debe spawnear al jugador de la lista de cada circuito
            Transform playerStartingPosition = currentCircuit._playersPositions[connectedPlayers].transform;
            //Se instancia el jugador y se spawnea
            var player = Instantiate(prefabPlayer, playerStartingPosition);
            player.GetComponent<NetworkObject>().SpawnAsPlayerObject(obj);
            //Se lleva la cuenta en el servidor del número de players conectados
            connectedPlayers++;
        }
    }

    private IEnumerator WaitTillSceneLoaded()
    {
        yield return new WaitUntil(() => SceneManager.GetActiveScene().isLoaded);
    }

    public void ConnectToRace()
    {
        //Prefab de la lista de Networks
        prefabPlayer = networkManager.NetworkConfig.Prefabs.Prefabs[0].Prefab;
        //Referencias
        currentCircuit = GameObject.FindGameObjectWithTag("CircuitManager").GetComponent<CircuitController>();
        virtualCamera = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
        currentRace = GameObject.FindGameObjectWithTag("CircuitManager").GetComponent<RaceController>();
        ntGameInfo.checkpoints = GameObject.FindWithTag("Checkpoint").GetComponent<FindCheckPoints>().points;
        _chronometer = UIManager.Instance._chronometer.GetComponent<Chronometer>();
        //Se escribe en la UI el joinCode
        UIManager.Instance._raceCodeUI.SetText(UIManager.Instance.joinCode);
    }
    #endregion

}