using Cinemachine;
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


    private void OnClientConnected(ulong obj)
    {
        mapaNumeroLocal = mapaNumero.Value;
        StartCoroutine(WaitTillSceneLoaded());
        ConnectToRace();
        //InstantiatePlayerServerRpc(actualPlayerInfo.playerCar, obj);

        if (NetworkManager.Singleton.IsServer)
        {
            //print("ISserverPrefab: " + prefabPlayer);
            Transform playerStartingPosition = currentCircuit._playersPositions[connectedPlayers].transform;
            var player = Instantiate(prefabPlayer, playerStartingPosition);
            player.GetComponent<NetworkObject>().SpawnAsPlayerObject(obj);
            connectedPlayers++;


            print("a - PLAYER ID: " + player.GetComponent<NetworkObject>().NetworkObjectId);
            print("d - PLAYER ID: " + obj);
            //pCopia = player.GetComponent<Player>();
            //print(prefabPlayer.name);
            //AssignPlayerClientRpc(player.GetComponent<NetworkObject>().NetworkObjectId, obj);
            
            //print("ISserverPrefabN: " + prefabPlayer);
            //actualPlayer = player.GetComponent<Player>();
        }
        
        //actualPlayer = NetworkManager.Singleton.SpawnManager.SpawnedObjects[obj+1].GetComponent<Player>();
    }

    //Player InstancePlayer(ulong obj)
    //{
    //    Transform playerStartingPosition = currentCircuit._playersPositions[connectedPlayers].transform;
    //    var player = Instantiate(prefabPlayer, playerStartingPosition);
    //    player.GetComponent<NetworkObject>().SpawnAsPlayerObject(obj);
    //    return player.GetComponent<Player>();
    //}


    [ClientRpc]
    private void AssignPlayerClientRpc(ulong playerId, ulong clientId)
    {
        print("ENTRO A CLIENTRPC: " + NetworkManager.Singleton.LocalClientId);
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            print("PLAYER ID: " + playerId);
            print("CLIENT ID: " + clientId);
            var playerObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[playerId];
            actualPlayer = playerObject.GetComponent<Player>();
            //NotifyServerOfPlayerInstantiationServerRpc(playerId);
        }
    }

    //[ServerRpc]
    //private void InstantiatePlayerServerRpc(int pos, ulong obj)
    //{
    //    //prefabPrueba = prefabPlayerToInstantiate;
    //    print("ServerRPC");
        
    //    //print("prefab: " + prefabPlayer.name);
    //    //print("argumento: " + prefabPlayerToInstantiate.name);
    //    print(pos);
    //    prefabPlayer = networkManager.NetworkConfig.Prefabs.Prefabs[pos].Prefab;
    //    print("prefab: " + prefabPlayer.name);

    //    Transform playerStartingPosition = currentCircuit._playersPositions[connectedPlayers].transform;

    //    var player = Instantiate(prefabPlayer, playerStartingPosition);
    //    actualPlayer = player.GetComponent<Player>();
    //    //mesh renderer
    //    player.GetComponent<NetworkObject>().SpawnAsPlayerObject(obj);
    //    print(prefabPlayer.name);
    //    connectedPlayers++;
                
    //    //prefabPlayer = prefabPlayerToInstantiate;
    //    //prefabPrueba = prefabPlayerToInstantiate;

    //    //actualPlayer = player.GetComponent<Player>();

    //}



    private IEnumerator WaitTillSceneLoaded()
    {
        yield return new WaitUntil(()=> SceneManager.GetActiveScene().name == mapasNombre[mapaNumeroLocal]);
    }

    //public NetworkVariable<FixedString32Bytes> mapSelected = new NetworkVariable<FixedString32Bytes>();
    public NetworkVariable<int> mapaNumero = new NetworkVariable<int>();
    public int mapaNumeroLocal;
    public string[] mapasNombre = { "NascarScene", "RainyScene", "OasisScene", "OwlPlainsScene"};
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