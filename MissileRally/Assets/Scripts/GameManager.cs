using Cinemachine;
using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int numPlayers = 50;
    public int connectedPlayers = 0;

    public RaceController currentRace;
    public CircuitController currentCircuit;

    public CinemachineVirtualCamera virtualCamera;

    public GameObject prefabPlayer;

    public NetworkManager networkManager;

    public PlayerInfo actualPlayerInfo;
    public Player actualPlayer;

    public string mapScene;
    public string joinCodeNumber;

    public NetworkVariable<int> carsReadyToRace_ntw = new NetworkVariable<int>(0);
    public int carsReadyToRace = 0;

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
        actualPlayerInfo = new PlayerInfo();

        networkManager = NetworkManager.Singleton;
        networkManager.OnServerStarted += OnServerStarted;
        networkManager.OnClientConnectedCallback += OnClientConnected;
        networkManager.OnClientDisconnectCallback += OnClientDisconnected;

        carsReadyToRace_ntw.OnValueChanged += OnCarsReadyChanged;
    }

    private void OnDestroy()
    {
        networkManager.OnServerStarted -= OnServerStarted;
        networkManager.OnClientConnectedCallback -= OnClientConnected;
        networkManager.OnClientDisconnectCallback -= OnClientDisconnected;

        carsReadyToRace_ntw.OnValueChanged -= OnCarsReadyChanged;
    }

    public void OnCarsReadyChanged(int previousValue, int newValue)
    {
        carsReadyToRace = newValue;
        Debug.Log($"Cars Ready Changed: {previousValue} -> {newValue}");
        UIManager.Instance._numberCarReadyUI.SetText(newValue.ToString());
        if (carsReadyToRace >= 2)
        {
            BeginRace();
        }
    }

    private void BeginRace()
    {
        EnablePlayerInputs();
        UIManager.Instance.DisableUIToStartRace();
    }

    [ServerRpc(RequireOwnership = false)]
    public void IncrementCarReadyServerRpc()
    {
        Debug.Log($"Before Increment: {carsReadyToRace_ntw.Value}");
        carsReadyToRace_ntw.Value += 1;
        Debug.Log($"After Increment: {carsReadyToRace_ntw.Value}");
    }

    private void EnablePlayerInputs()
    {
        
        foreach (var networkObject in NetworkManager.Singleton.SpawnManager.SpawnedObjectsList)
        {
            // Verificamos si el objeto tiene el componente Player
            if (networkObject.TryGetComponent<Player>(out var player))
            {
                player.EnablePlayerInput();
            }
        }
    }

    //bool cocheEnCarrera = false;
    private void Update()
    {
        //EVENTO para gestionar mejor lo siguiente:
        //if ((SceneManager.GetActiveScene().name == mapScene) && !cocheEnCarrera)
        //{
        //    print("Hola");
        //    currentCircuit = GameObject.FindGameObjectWithTag("CircuitManager").GetComponent<CircuitController>();
        //    currentRace = GameObject.FindGameObjectWithTag("CircuitManager").GetComponent<RaceController>();
        //    virtualCamera = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();


        //    ConnectToRace();
        //    cocheEnCarrera = true;
        //}
        print("Local: " + mapScene);
        //print("Network: "+ mapSelected.Value);
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
        }
    }

    private void OnClientConnected(ulong obj)
    {
        mapaNumeroLocal = mapaNumero.Value;
        StartCoroutine(WaitTillSceneLoaded());
        ConnectToRace();

        if (NetworkManager.Singleton.IsServer)
        {
            Transform playerStartingPosition = currentCircuit._playersPositions[connectedPlayers].transform;
            var player = Instantiate(prefabPlayer, playerStartingPosition);
            player.GetComponent<NetworkObject>().SpawnAsPlayerObject(obj);
            //actualPlayer = player.GetComponent<Player>();
            connectedPlayers++;
        }
    }
    private IEnumerator WaitTillSceneLoaded()
    {
        yield return new WaitUntil(() => SceneManager.GetActiveScene().isLoaded);
    }

    //public NetworkVariable<FixedString32Bytes> mapSelected = new NetworkVariable<FixedString32Bytes>();
    public NetworkVariable<int> mapaNumero = new NetworkVariable<int>();
    public int mapaNumeroLocal;
    public string[] mapasNombre = { "NascarScene", "RainyScene", "OasisScene", "OwlPlainsScene" };
    public Material[] coloresMaterial;

    public void SetMapSelected(int mapNumber)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            mapaNumero.Value = mapNumber;
            mapaNumeroLocal = mapaNumero.Value;
            //mapScene = mapasNombre[mapNumber];
        }
    }


    public void ConnectToRace()
    {
        //if (NetworkManager.Singleton.IsServer)
        //{
        //    //mapaNumero.Value = mapNumber;

        //    //mapScene = mapasNombre[mapNumber];
        //}

        //DUDA 1

        print("Variable: " + mapaNumeroLocal);
        print("Escena: " + SceneManager.GetActiveScene().name);
        //mapScene = SceneManager.GetActiveScene().name;
        currentCircuit = GameObject.FindGameObjectWithTag("CircuitManager").GetComponent<CircuitController>();
        currentRace = GameObject.FindGameObjectWithTag("CircuitManager").GetComponent<RaceController>();
        virtualCamera = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
        UIManager.Instance._raceCodeUI.SetText(joinCodeNumber);
        prefabPlayer = networkManager.NetworkConfig.Prefabs.Prefabs[0].Prefab;


        //SetMapSelected(mapScene);
    }
    #endregion

}