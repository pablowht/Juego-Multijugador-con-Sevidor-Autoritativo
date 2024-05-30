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
    public int visitedCheckpoints = 0;

    public int CurrentPosition { get; set; }
    public int CurrentLap { get; set; }

    // Player Objects
    private CinemachineVirtualCamera _vPlayerCamera;

    // información color
    //Material colorCoche;
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
        //UIManager.Instance._carController = carController;
        //if (IsOwner)
        //{
        //    GetComponent<PlayerInput>().enabled = true;
        //}
        networkColorIdx.OnValueChanged += OnSetColor;
        //print("actualPlayerCopia: " + GameManager.Instance.actualPlayer.name);
        //carsReadyToRace_ntw.OnValueChanged += OnCarsReadyChanged;

        //carController.OnSpeedChangeEvent += updatePlayerSpeed;

    }

    //private void updatePlayerSpeed(float newVal)
    //{
    //    GameManager.Instance.actualPlayerInfo.playerSpeed = newVal; //para el servidor
    //    //GameManager.Instance.ntGameInfo.updateSpeedClientRpc(newVal); //para los clientes
    //}


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

            GameManager.Instance.currentRace.AddPlayer(this);
            
            GameManager.Instance.actualPlayer = this;

            Name = GameManager.Instance.actualPlayerInfo.playerName;
            print(Name);
            GameManager.Instance.nombrePlayer = Name;
            SetupCamera();
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

    public void EnablePlayerInput()
    {
        if (IsOwner)
        {
            GetComponent<PlayerInput>().enabled = true;
        }
    }

    void OnSetColor(int previous, int newM)
    {
        //Material body = car.transform.GetChild(0).GetComponent<MeshRenderer>().materials[1];
        MeshRenderer body = car.transform.GetChild(0).GetComponent<MeshRenderer>();
        Material[] mat = body.materials;
        print("material: " + newM);
        mat[1] = GameManager.Instance.coloresMaterial[newM];
        body.materials = mat;
        /*Se iguala el material otra vez, porque por defecto body.materials devuelve una copia del valor,
         por lo que hay que reflejar los cambios*/
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
        carController.InputAcceleration = input.y;
        carController.InputSteering = input.x;
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