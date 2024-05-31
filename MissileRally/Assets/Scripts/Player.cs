using Cinemachine;
using System;
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

    // información color
    NetworkVariable<int> networkColorIdx = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    //esta variable SOLO puede ser escrita por el propio owner, el servidor NO puede escribirla, solo leerla
    //para actualizar la información al resto de clientes
    int colorCocheIdx;

    public override string ToString()
    {
        return Name;
    }

    private void Start()
    {
        carController = car.GetComponent<CarController>();
        networkColorIdx.OnValueChanged += OnSetColor;
    }

    #region Network

    public override void OnNetworkSpawn()
    {
        SetupPlayer();
    }

    void SetupPlayer()
    {
        SetupColor();
        if (IsOwner)
        {
            ntID = OwnerClientId;
            ID = (int)ntID;
            
            GameManager.Instance.actualPlayer = this;
            GameManager.Instance.currentRace.AddPlayer(this);

            Name = GameManager.Instance.actualPlayerInfo.playerName;
            print(Name);
            GameManager.Instance.nombrePlayer = Name;
            SetupCamera();

            UIManager.Instance.botonCarReady.SetActive(true);
        }

        if (IsServer)
        {
            print("jugador añadido");
            GameManager.Instance.ntGameInfo.currentPlayerInstance.Add(this);
            GameManager.Instance.ntGameInfo.sendCodeClientRpc(GameManager.Instance.ntGameInfo.code);
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