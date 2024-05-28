using Cinemachine;
using System;
using System.Collections;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;

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

    public GameObject prefabPrueba;

    private void OnClientConnected(ulong obj)
    {
        mapaNumeroLocal = mapaNumero.Value;
        StartCoroutine(WaitTillSceneLoaded());
        ConnectToRace();

        prefabPrueba = prefabPlayer;
        //print(prefabPrueba);

        InstantiatePlayerServerRpc(actualPlayerInfo.playerCar);

        if (NetworkManager.Singleton.IsServer)
        {
            Transform playerStartingPosition = currentCircuit._playersPositions[connectedPlayers].transform;

            var player = Instantiate(prefabPlayer, playerStartingPosition);
            actualPlayer = player.GetComponent<Player>();
            player.GetComponent<NetworkObject>().SpawnAsPlayerObject(obj);
            print(prefabPlayer.name);
            connectedPlayers++;
        }
    }

    [ServerRpc]
    private void InstantiatePlayerServerRpc(int pos)
    {
        //prefabPrueba = prefabPlayerToInstantiate;
        print("ServerRPC");
        print("prefab: " + prefabPlayer.name);
        //print("argumento: " + prefabPlayerToInstantiate.name);
        print(pos);
        prefabPlayer = networkManager.NetworkConfig.Prefabs.Prefabs[pos].Prefab;
        print("prefab: " + prefabPlayer.name);

        //prefabPlayer = prefabPlayerToInstantiate;
        //prefabPrueba = prefabPlayerToInstantiate;

        //actualPlayer = player.GetComponent<Player>();

    }



    private IEnumerator WaitTillSceneLoaded()
    {
        yield return new WaitUntil(()=> SceneManager.GetActiveScene().name == mapasNombre[mapaNumeroLocal]);
    }

    //public NetworkVariable<FixedString32Bytes> mapSelected = new NetworkVariable<FixedString32Bytes>();
    public NetworkVariable<int> mapaNumero = new NetworkVariable<int>();
    public int mapaNumeroLocal;
    public string[] mapasNombre = { "NascarScene", "RainyScene", "OasisScene", "OwlPlainsScene"};

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
        prefabPlayer = networkManager.NetworkConfig.Prefabs.Prefabs[actualPlayerInfo.playerCar].Prefab;
        //SetMapSelected(mapScene);
    }
    #endregion

}