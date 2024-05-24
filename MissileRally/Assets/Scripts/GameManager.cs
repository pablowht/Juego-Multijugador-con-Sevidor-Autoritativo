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

    public PlayerInfo actualPlayer;

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
        actualPlayer = new PlayerInfo();

        networkManager = NetworkManager.Singleton;
        networkManager.OnServerStarted += OnServerStarted;
        networkManager.OnClientConnectedCallback += OnClientConnected;
    }

    private bool cocheEnCarrera = false;
    private void Update()
    {
        //print(SceneManager.GetActiveScene().name);
        if ((SceneManager.GetActiveScene().name == "RaceScene") && !cocheEnCarrera)
        {
            print("Hola");
            currentCircuit = GameObject.FindGameObjectWithTag("CircuitManager").GetComponent<CircuitController>();
            currentRace = GameObject.FindGameObjectWithTag("CircuitManager").GetComponent<RaceController>();
            virtualCamera = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
            

            ConnectToRace();
            cocheEnCarrera = true;
        }
    }

    #region Network
    private void OnServerStarted()
    {
        print("El servidor está listo");
    }

    private void OnClientConnected(ulong obj)
    {
        print("Hola Cliente");
        if (NetworkManager.Singleton.IsServer)
        {
            print("Hola Creando");
            StartCoroutine(WaitTillSceneLoaded());
            Transform playerStartingPosition = currentCircuit._playersPositions[connectedPlayers].transform;
            var player = Instantiate(prefabPlayer, playerStartingPosition);
            player.GetComponent<NetworkObject>().SpawnAsPlayerObject(obj);
            
            connectedPlayers++;
        }
    }

    private IEnumerator WaitTillSceneLoaded()
    {
        yield return new WaitUntil(()=> SceneManager.GetActiveScene().name == "RaceScene");
    }

    public void ConnectToRace()
    {
        print("DentroMetodo");
        prefabPlayer = networkManager.NetworkConfig.Prefabs.Prefabs[actualPlayer.playerCar].Prefab;
    }
    #endregion

}