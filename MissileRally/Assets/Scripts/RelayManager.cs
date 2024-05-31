using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using System.Threading.Tasks;

public class RelayManager : MonoBehaviour //se han implementado los métodos básicos de acuerdo a la documentación de Relay
{
    const int maxConnections = 6;

    //Singleton DontDestroy para persistencia entre escenas
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
        string code = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        NetworkManager.Singleton.StartHost();
        //print(code);
        GameManager.Instance.ntGameInfo.setJoinCodeServerRpc(code); //le envio al servidor el código de la sala
        UIManager.Instance._raceCodeUI.SetText(GameManager.Instance.ntGameInfo.code); //actualizo la UI del host con ese código
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
        NetworkManager.Singleton.StartClient();
    }
}
