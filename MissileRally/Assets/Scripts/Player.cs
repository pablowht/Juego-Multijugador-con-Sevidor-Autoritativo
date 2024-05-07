using Cinemachine;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : NetworkBehaviour
{
    // Player Info
    public string Name { get; set; }
    public int ID { get; set; }

    // Race Info
    public GameObject car;
    public TextMeshProUGUI namePlayer;
    public int CurrentPosition { get; set; }
    public int CurrentLap { get; set; }

    private InputAction Move = new InputAction();
    private InputAction Brake = new InputAction();
    private InputAction Attack = new InputAction();


    public NetworkVariable<Vector3> CarPosition = new NetworkVariable<Vector3>();

    CinemachineVirtualCamera _vCamera;

    public override string ToString()
    {
        return Name;
    }

    private void Start()
    {

    }

    private void Update()
    {
        //if (IsServer)
        //{
        //    // Sé que es mejor suscribirse y tal pero es por probar
        //    // En el host entra, pero en el cliente no porque no es server...
        //    // Pero si se saca de aquí da error porque solo el server tiene derecho a escribir en las network variables
        //    // Utilizando un ServerRpc no me ha funcionado muy bien
        //    CarPosition.Value = car.transform.position;
        //}
    }
    private void FixedUpdate()
    {
        
    }


    public override void OnNetworkSpawn()
    {
        SetupPlayer();
    }

    void SetupPlayer()
    {
        if (IsOwner)
        {
            GameManager.Instance.currentRace.AddPlayer(this);

            SetupCamera();

            namePlayer.SetText(OwnerClientId.ToString());

            SetupInput();
            SetupPosition();

            //_nPlayerPosition.OnValueChanged += OnPositionChange;
            //_nPlayerRotation.OnValueChanged += OnRotationChange;
            

            //_playerTransform = transform;
            //_nPlayerPosition.OnValueChanged += OnPositionChange;
            //_nPlayerRotation.OnValueChanged += OnRotationChange;
        }
    }

    void SetupCamera()
    {
        _vCamera = GameManager.Instance._virtualCamera;

        _vCamera.Follow = car.GetComponent<Transform>();
        _vCamera.LookAt = car.GetComponent<Transform>();
    }

    void SetupPosition()
    {
        print("Coche " + OwnerClientId.ToString());
        car.transform.position = GameManager.Instance.currentCircuit._playersPositions[(int)OwnerClientId].position;
        print("Posicion Coche" + car.transform.position);
        print("Posicion Debería " + GameManager.Instance.currentCircuit._playersPositions[(int)OwnerClientId].position);
        print("Existe? " + GameManager.Instance.currentCircuit._playersPositions[(int)OwnerClientId]);
    }

    void SetupInput()
    {
        GetComponent<PlayerInput>().enabled = true;

        InputController input = GetComponent<InputController>();

        Move.performed += input.OnMove;
        Move.Enable();

        Brake.performed += input.OnBrake;
        Brake.Enable();

        Attack.performed += input.OnBrake;
        Attack.Enable();
    }
    


    //private void OnPositionChange(Vector3 previousValue, Vector3 newValue)
    //{
    //    if (IsOwner)
    //    {
    //        _nPlayerPosition.Value = _playerTransform.position;
    //    }
    //    else
    //    {
    //        _playerTransform.position = newValue;
    //    }
        
    //}  
    //private void OnRotationChange(Quaternion previousValue, Quaternion newValue)
    //{
    //    if (IsOwner)
    //    {
    //        _nPlayerRotation.Value = _playerTransform.rotation;
    //    }
    //    else
    //    {
    //        _playerTransform.rotation = newValue;
    //    }
    //}
}
