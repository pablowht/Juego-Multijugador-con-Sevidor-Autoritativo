using Cinemachine;
using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;


public class Player : NetworkBehaviour
{
     
    // Player Info
    public string Name { get; set; }
    public int ID { get; set; }
    public ulong ntID { get; set;}
    

    // Race Info
    public GameObject car;
    public CarController carController; //Esta variable la he puesto pública para poder cambiar la aguja en la UI pero no se si hay mejores opciones
    //CONEJO: que vaya sumando cada vez que visita un checkpoint y se resetee por cada vuelta

    public int CurrentPosition { get; set; }
    public int CurrentLap { get; set; }

    // Player Objects
    private CinemachineVirtualCamera _vPlayerCamera;
    public bool[] visitedCheckpoint = new bool[20];
    public int count = 0;
    public int rankPosition = 0;
    public float coefRed = 1;
    public float distancia = 0;

    // información color
    NetworkVariable<int> networkColorIdx = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    //esta variable SOLO puede ser escrita por el propio owner, el servidor NO puede escribirla, solo leerla
    //para actualizar la información al resto de clientes
    int colorCocheIdx;

    public TextMeshProUGUI _playerNameUI;
    NetworkVariable<NetworkString> networkPlayerName = new NetworkVariable<NetworkString>("Pepona", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public override string ToString()
    {
        return Name;
    }

    private void Start()
    {
        carController = car.GetComponent<CarController>();
        networkColorIdx.OnValueChanged += OnSetColor;
        networkPlayerName.OnValueChanged += OnSetName;
    }

    

    public void DisablePlayerInput()
    {
        if (IsOwner)
        {
            GetComponent<PlayerInput>().enabled = false;
        }
    }

    #region Network

    public override void OnNetworkSpawn()
    {
        SetupPlayer();
    }
    void SetupPlayer()
    {
        SetupColor();
        SetupName();
        if (IsOwner)
        {
            ntID = OwnerClientId;
            ID = (int)ntID;
            
            GameManager.Instance.actualPlayer = this;
            //GameManager.Instance.currentRace.AddPlayer(this);

            //Name = GameManager.Instance.actualPlayerInfo.playerName;
            //GameManager.Instance.nombrePlayer = Name;

            SetupCamera();

            UIManager.Instance.botonCarReady.SetActive(true);
        }

        if (IsServer)
        {
            print("jugador añadido");
            GameManager.Instance.ntGameInfo.currentPlayerInstance.Add(this);
            GameManager.Instance.currentRace.AddPlayer(this);

            GameManager.Instance.ntGameInfo.sendCodeClientRpc(GameManager.Instance.ntGameInfo.code);
        }
    }

    private void SetupName()
    {
        if (IsOwner)
        {
            print("entro a actualizar Nombre");
            Name = GameManager.Instance.actualPlayerInfo.playerName;
            GameManager.Instance.nombrePlayer = Name;
            networkPlayerName.Value = Name;
            OnSetName(networkPlayerName.Value, networkPlayerName.Value);
        }
        else
        {
            OnSetName(networkPlayerName.Value, networkPlayerName.Value);
        }
    }

    void SetupColor()
    {
        if (IsOwner)
        {
            print("entro a actualizar color");
            colorCocheIdx = GameManager.Instance.actualPlayerInfo.playerCar;
            networkColorIdx.Value = colorCocheIdx;
            OnSetColor(0, networkColorIdx.Value);
        }
        else
        {
            OnSetColor(0, networkColorIdx.Value);
        }
    }


    void OnSetColor(int previous, int newM)
    {
        MeshRenderer body = car.transform.GetChild(0).GetComponent<MeshRenderer>();
        Material[] mat = body.materials;
        print("material: " + newM);
        mat[1] = GameManager.Instance.coloresMaterial[newM];
        body.materials = mat;
        /*Se iguala el material otra vez, porque por defecto body.materials devuelve una copia del valor,
         por lo que hay que reflejar los cambios*/
    }

    private void OnSetName(NetworkString previousValue, NetworkString newValue)
    {
        _playerNameUI.SetText(newValue.ToString());
    }

    public void EnablePlayerInput()
    {
        if (IsOwner)
        {
            GetComponent<PlayerInput>().enabled = true;
        }
    }

    void SetupCamera()
    {
        _vPlayerCamera = GameManager.Instance.virtualCamera;

        _vPlayerCamera.Follow = car.GetComponent<Transform>();
        _vPlayerCamera.LookAt = car.GetComponent<Transform>();
    }
    #endregion

    [ClientRpc]
    public void orderRaceClientRpc(int orden)
    {
        print("clientRPc");
        if (IsOwner)
        {
            //actualizar ui
            UIManager.Instance.UpdateCarOrderNumberUI(orden);
            print("mi puesto es: " + orden);
            print(ID);
        }
    }

    #region Input

    public void OnMove(InputAction.CallbackContext context)
    {
        OnMoveServerRpc(context.ReadValue<Vector2>());
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        OnAttackServerRpc();
    }

    public void OnBrake(InputAction.CallbackContext context)
    {
        OnBrakeServerRpc(context.ReadValue<float>());
    }

    // Server RPCs
    [ServerRpc]
    public void OnMoveServerRpc(Vector2 input)
    {
        //COMENTAR - ELEFANTE: no es buena idea reducirlo aqui, no se reduce
        carController.InputAcceleration = input.y * coefRed;
        carController.InputSteering = input.x * coefRed;
    }
    [ServerRpc]
    public void OnAttackServerRpc()
    {
        print("DISPARANDO...");
    }
    [ServerRpc]
    public void OnBrakeServerRpc(float input)
    {
        carController.InputBrake = input;
    }
    #endregion
}