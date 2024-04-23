using Unity.Netcode;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int numPlayers = 50;

    public RaceController currentRace;

    // Cosas nuestras
    NetworkManager _ntmanager;
    GameObject _prefabPlayer;

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
        _ntmanager = NetworkManager.Singleton;

        _prefabPlayer = _ntmanager.NetworkConfig.Prefabs.Prefabs[0].Prefab;

        _ntmanager.OnServerStarted += OnServerStarted;
        _ntmanager.OnClientConnectedCallback += OnClientConnected;
    }

    private void OnServerStarted()
    {
        print("El servidor está listo");
    }

    private void OnClientConnected(ulong obj)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            var player = Instantiate(_prefabPlayer);
            player.GetComponent<NetworkObject>().SpawnWithOwnership(obj);
        }
    }

    public void OnDestroy()
    {
        _ntmanager.OnServerStarted -= OnServerStarted;
        _ntmanager.OnClientConnectedCallback -= OnClientConnected;
    }
}