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
    public CarController carController; //Esta variable la he puesto pública para poder cambiar la aguja en la UI pero no se si hay mejores opciones

    public int CurrentPosition { get; set; }
    public int CurrentLap { get; set; }

    // Player Objects
    private CinemachineVirtualCamera _vPlayerCamera;

    public override string ToString()
    {
        return Name;
    }

    private void Start()
    {
        carController = car.GetComponent<CarController>();
        UIManager.Instance._carController = carController;
        //if (IsOwner)
        //{
        //    GetComponent<PlayerInput>().enabled = true;
        //}
    }


    #region Network

    public override void OnNetworkSpawn()
    {
        SetupPlayer();
    }

    void SetupPlayer()
    {
        if (IsOwner)
        {
            ID = (int)OwnerClientId;

            GameManager.Instance.currentRace.AddPlayer(this);

            SetupCamera();

            //namePlayer.SetText(OwnerClientId.ToString());
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