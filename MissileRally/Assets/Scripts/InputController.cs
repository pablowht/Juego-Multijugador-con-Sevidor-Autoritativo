using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    private CarController car;
    //private readonly NetworkVariable<Vector3> _nPlayerPosition = new(writePerm: NetworkVariableWritePermission.Owner);
    //private readonly NetworkVariable<Quaternion> _nPlayerRotation = new(writePerm: NetworkVariableWritePermission.Owner);
    private Transform _playerTransform;
    private NetworkVariable<Quaternion> _nPlayerRotation = new NetworkVariable<Quaternion>();
    private NetworkVariable<Vector2> _nPlayerPosition = new NetworkVariable<Vector2>();
    Transform _cameraTransform;
    private void Start()
    {
        car = GetComponent<Player>().car.GetComponent<CarController>();
        _playerTransform = car.transform;

        _nPlayerPosition.OnValueChanged += OnPositionChange;
        _nPlayerRotation.OnValueChanged += OnRotationChange;
        _cameraTransform = Camera.main.transform;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        OnMoveServerRpc(context.ReadValue<Vector2>());
    }

    public void OnBrake(InputAction.CallbackContext context)
    {
        OnBrakeServerRpc(context.ReadValue<float>());
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
    }

    [ServerRpc]
    public void OnMoveServerRpc(Vector2 input)
    {
        car.InputAcceleration = -input.y;
        car.InputSteering = input.x;
    }

    public void OnBrakeServerRpc(float input)
    {
        car.InputBrake = input;
    }

    private void OnPositionChange(Vector2 previousValue, Vector2 newValue)
    {
        print("el valor posicion ha cambiado");
        _playerTransform.position = newValue;
    }
    private void OnRotationChange(Quaternion previousValue, Quaternion newValue)
    {
        print("el valor rotacion ha cambiado");
        _playerTransform.rotation = newValue;
    }
}