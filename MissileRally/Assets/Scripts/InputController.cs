using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class InputController : MonoBehaviour
{
    private CarController car;

    private void Start()
    {
        car = GetComponent<Player>().car.GetComponent<CarController>();
        //_playerTransform = car.transform;

        //_nPlayerPosition.OnValueChanged += OnPositionChange;
        //_nPlayerRotation.OnValueChanged += OnRotationChange;
    }


    public void OnMove(InputAction.CallbackContext context)
    {
        print("me muevoooo");
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
        print("onmoveServerrpc");
        car.InputAcceleration = -input.y;
        car.InputSteering = input.x;
        print("input aceleration: " + car.InputAcceleration);
    }

    public void OnBrakeServerRpc(float input)
    {
        car.InputBrake = input;
    }
}