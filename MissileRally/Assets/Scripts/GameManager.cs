using Cinemachine;
using Unity.Netcode;
using UnityEngine;

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
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        actualPlayer = new PlayerInfo();
        //ntwManager = NetworkManager.Singleton;

        //prefabPlayer = ntwManager.NetworkConfig.Prefabs.Prefabs[0].Prefab;

        //ntwManager.OnServerStarted += OnServerStarted;
        //ntwManager.OnClientConnectedCallback += OnClientConnected;

    }

    #region Network Spawn
    private void OnServerStarted()
    {
        print("El servidor está listo");
    }

    private void OnClientConnected(ulong obj)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            Transform playerStartingPosition = currentCircuit._playersPositions[connectedPlayers].transform;
            var player = Instantiate(prefabPlayer, playerStartingPosition);
            player.GetComponent<NetworkObject>().SpawnAsPlayerObject(obj);
            
            connectedPlayers++;
        }
    }
    #endregion

}