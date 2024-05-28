using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using System.Threading.Tasks;

public class RelayManager : MonoBehaviour
{
    const int maxConnections = 50;

    public string joinCode = "C�digo de la carrera...";

    public static RelayManager Instance { get; private set; }

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

    async public Task StartHost()
    {
        await UnityServices.InitializeAsync();
        if(!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));
        joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        GameManager.Instance.joinCodeNumber = joinCode;
        NetworkManager.Singleton.StartHost();
    }

    async public Task StartClient(string joinCodeInput)
    {
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode: joinCodeInput);
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
        GameManager.Instance.joinCodeNumber = joinCode;
        NetworkManager.Singleton.StartClient();
    }
}
