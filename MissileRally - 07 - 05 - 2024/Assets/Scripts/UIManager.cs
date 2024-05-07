using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    // Se usa la variable de GameManager para el maxConnections (numPlayers)
    //const int maxConnections = 50;
    string joinCode = "Enter room code...";

    async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn +=
            () => print($"New player {AuthenticationService.Instance.PlayerId} connected");

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 400, 400));
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            StartButtons();
        }
        else
        {
            StatusLabels();
        }

        GUILayout.EndArea();
    }
    void StartButtons()
    {
        if (GUILayout.Button("Host")) StartHost();
        if (GUILayout.Button("Client")) StartClient();
        //if (GUILayout.Button("Server")) NetworkManager.Singleton.StartServer();

        joinCode = GUILayout.TextField(joinCode);
    }

    void StatusLabels()
    {
        var mode = NetworkManager.Singleton.IsHost ?
            "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";

        GUILayout.Label("Transport: " +
            NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
        GUILayout.Label("Mode: " + mode);
        GUILayout.Label("Room: " + joinCode);
    }

    //metodo asincrono: voy a tener una parte de mi codigo parada, me voy a otro lado
    //a seguir con mis cosas y luego sigo
    async void StartHost()
    {
        try
        {
            await UnityServices.InitializeAsync();
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }

            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(GameManager.Instance.numPlayers);
            NetworkManager.Singleton.GetComponent<UnityTransport>()
                .SetRelayServerData(new RelayServerData(allocation, "dtls"));
            joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            NetworkManager.Singleton.StartHost();
        }
        catch (RelayServiceException e)
        {
            print(e);
        }
    }

    async void StartClient()
    {
        try
        {
            await UnityServices.InitializeAsync();
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }

            var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode: joinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>()
                .SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
            NetworkManager.Singleton.StartClient();
        }
        catch (RelayServiceException e)
        {
            print(e);
        }
    }

    
}
