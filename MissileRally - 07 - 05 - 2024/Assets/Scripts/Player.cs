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

    public NetworkVariable<Vector3> CarPosition = new NetworkVariable<Vector3>();

    CinemachineVirtualCamera _vCamera;

    public override string ToString()
    {
        return Name;
    }

    private void Start()
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
        //print("Coche " + OwnerClientId.ToString());
        car.transform.position = GameManager.Instance.currentCircuit._playersPositions[(int)OwnerClientId].position;
        //print("Posicion Coche" + car.transform.position);
        //print("Posicion Debería " + GameManager.Instance.currentCircuit._playersPositions[(int)OwnerClientId].position);
        //print("Existe? " + GameManager.Instance.currentCircuit._playersPositions[(int)OwnerClientId]);
    }

    void SetupInput()
    {
        GetComponent<PlayerInput>().enabled = true;
    }
}
