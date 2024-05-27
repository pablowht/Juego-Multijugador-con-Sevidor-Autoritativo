using Cinemachine;
using System;
using System.Collections;
using Unity.Collections;
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
        mapScene = "NascarScene";

        networkManager = NetworkManager.Singleton;
        networkManager.OnServerStarted += OnServerStarted;
        networkManager.OnClientConnectedCallback += OnClientConnected;
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
        print("Network: "+ mapSelected.Value);
    }

    #region Network
    private void OnServerStarted()
    {
        print("El servidor está listo");
    }

    private void OnClientConnected(ulong obj)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            StartCoroutine(WaitTillSceneLoaded());
            ConnectToRace();

            UIManager.Instance._raceCodeUI.SetText(RelayManager.Instance.joinCode);

            //DUDA 2
            Transform playerStartingPosition = currentCircuit._playersPositions[connectedPlayers].transform;
            var player = Instantiate(prefabPlayer, playerStartingPosition);

            //Quizás lo siguiente hay que cambiarlo, pero de momento lo dejaría
            actualPlayer = player.GetComponent<Player>();

            player.GetComponent<NetworkObject>().SpawnAsPlayerObject(obj);
            
            connectedPlayers++;
        }
    }

    private IEnumerator WaitTillSceneLoaded()
    {
        yield return new WaitUntil(()=> SceneManager.GetActiveScene().name == mapScene);
    }

    public NetworkVariable<FixedString32Bytes> mapSelected = new NetworkVariable<FixedString32Bytes>();

    public void SetMapSelected(string map)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            FixedString32Bytes fixedStringMap = new FixedString32Bytes(map);
            mapSelected.Value = fixedStringMap;
        }
    }


    public void ConnectToRace()
    {
        //DUDA 1
        print("Variable" + mapScene);
        print("Scena" + SceneManager.GetActiveScene().name);
        //mapScene = SceneManager.GetActiveScene().name;
        currentCircuit = GameObject.FindGameObjectWithTag("CircuitManager").GetComponent<CircuitController>();
        currentRace = GameObject.FindGameObjectWithTag("CircuitManager").GetComponent<RaceController>();
        virtualCamera = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
        prefabPlayer = networkManager.NetworkConfig.Prefabs.Prefabs[actualPlayerInfo.playerCar].Prefab;
        SetMapSelected(mapScene);
    }
    #endregion

}