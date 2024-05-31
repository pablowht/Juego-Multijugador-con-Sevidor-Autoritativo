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
    public CarController carController;
    public int CurrentLap { get; set; }

    // Player Objects
    private CinemachineVirtualCamera _vPlayerCamera; //la cámara del jugador
    public bool[] visitedCheckpoint = new bool[20]; //lista de checkpoints que ha visitado
    /*FUNCIONAMIENTO CHECKPOINTS:
        el jugador tiene que atravesar TODOS los checkpoints para contar una vuelta, si llega a la meta saltándose uno,
        la vuelta no se contará y tendrá que repetirla
    */
    public int count = 0; //´variable que cuenta el número de checkpoints visitados
    public int rankPosition = 0; //posición en el ranking
    public float distancia = 0; //para saber cuanto lleva recorrido

    // información color
    NetworkVariable<int> networkColorIdx = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    //esta variable SOLO puede ser escrita por el propio owner del player, el servidor NO puede escribirla, solo leerla
    //para actualizar la información al resto de clientes
    int colorCocheIdx; //variable local del color

    // información nombre para que se actualice al resto de jugadores
    public TextMeshProUGUI _playerNameUI; //variable local del nombre
    NetworkVariable<NetworkString> networkPlayerName = new NetworkVariable<NetworkString>("Pepona", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    //al igual que la NetworkVariable del color, solo puede ser escrita por el owner del player
    public override string ToString()
    {
        return Name;
    }

    private void Start()
    {
        //asignamos scripts y subscribimos métodos
        carController = car.GetComponent<CarController>();
        networkColorIdx.OnValueChanged += OnSetColor;
        networkPlayerName.OnValueChanged += OnSetName;
    }
    public void DisablePlayerInput() //desactiva el input del jugador
    {
        if (IsOwner) //solo ÉL mismo puede desactivarse su input
        {
            GetComponent<PlayerInput>().enabled = false;
        }
    }

    #region Network

    public override void OnNetworkSpawn() //se ejecuta cuando un cliente es spawneado
    {
        SetupPlayer(); //iniciación del server
    }
    void SetupPlayer()
    {
        SetupColor(); //inicializar el color
        SetupName(); //inicializar el nombre
        if (IsOwner)
        {
            ntID = OwnerClientId; //id original
            ID = (int)ntID; //id casteado a int
            
            GameManager.Instance.actualPlayer = this; //asignamos el actual player en LOCAL
            SetupCamera(); //inicialización de la cámara
            UIManager.Instance.botonCarReady.SetActive(true); //activación del botón ready para dar comienzo a la carrera
        }

        if (IsServer) //el servidor modifica variables UNICAS del server
        {
            //print("jugador añadido");
            GameManager.Instance.ntGameInfo.currentPlayerInstance.Add(this); //añade una referencia al player que se acaba de instanciar
            GameManager.Instance.currentRace.AddPlayer(this); //añade un player a la carrera actual
            GameManager.Instance.ntGameInfo.sendCodeClientRpc(GameManager.Instance.ntGameInfo.code); //actualiza y envía el código de acceso a los clientes
        }
    }

    private void SetupName()
    {
        if (IsOwner) 
        {
            //actualiza la NetworkVariable y la variable local del player del nombre
            //print("entro a actualizar Nombre");
            Name = GameManager.Instance.actualPlayerInfo.playerName;
            GameManager.Instance.nombrePlayer = Name;
            networkPlayerName.Value = Name;
            OnSetName(networkPlayerName.Value, networkPlayerName.Value);
        }
        else //para que la información se sincronice tanto en host como en cliente
        {
            OnSetName(networkPlayerName.Value, networkPlayerName.Value);
        }
    }

    void SetupColor() //mismo funcionamiento que el anterior pero con el color
    {
        if (IsOwner)
        {
            //print("entro a actualizar color");
            colorCocheIdx = GameManager.Instance.actualPlayerInfo.playerCar;
            networkColorIdx.Value = colorCocheIdx;
            OnSetColor(0, networkColorIdx.Value);
        }
        else
        {
            OnSetColor(0, networkColorIdx.Value);
        }
    }


    void OnSetColor(int previous, int newM) //cuando la NetworkVariable del color modifica su valor
    {
        MeshRenderer body = car.transform.GetChild(0).GetComponent<MeshRenderer>();
        Material[] mat = body.materials;
        //print("material: " + newM);
        mat[1] = GameManager.Instance.coloresMaterial[newM];
        body.materials = mat;
        /*Se iguala el material otra vez, porque por defecto body.materials devuelve una copia del valor,
         por lo que hay que reflejar los cambios*/
    }

    private void OnSetName(NetworkString previousValue, NetworkString newValue) //cuando la NetworkVariable del nombre modifica su valor
    {
        _playerNameUI.SetText(newValue.ToString());
    }

    public void EnablePlayerInput() //activa el player input
    {
        if (IsOwner) //solo el owner puede activarselo a el mismo
        {
            GetComponent<PlayerInput>().enabled = true;
        }
    }

    void SetupCamera() //inicialización de la cámara
    {
        _vPlayerCamera = GameManager.Instance.virtualCamera;

        _vPlayerCamera.Follow = car.GetComponent<Transform>();
        _vPlayerCamera.LookAt = car.GetComponent<Transform>();
    }
    #endregion

    [ClientRpc]
    public void orderRaceClientRpc(int orden) //clientRpc para actualizar el ranking actual en el que estoy (puesto)
    {
        //print("clientRPc");
        if (IsOwner) //solo el owner puede hacerse cambios a si mismo
        {
            UIManager.Instance.UpdateCarOrderNumberUI(orden);
            //print("mi puesto es: " + orden);
            //print(ID);
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