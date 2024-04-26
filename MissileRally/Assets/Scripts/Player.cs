using System;
using System.Collections;
using System.Collections.Generic;
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
    public GameObject _camera;
    public int CurrentPosition { get; set; }
    public int CurrentLap { get; set; }

    private InputAction Move;
    private InputAction Brake;
    private InputAction Attack;
    

    //private NetworkVariable<Vector3> _nPlayerPosition = NetworkVariable<Vector3>()
    //private readonly NetworkVariable<Vector3> _nPlayerPosition = new(writePerm: NetworkVariableWritePermission.Owner);
    //private readonly NetworkVariable<Quaternion> _nPlayerRotation = new(writePerm: NetworkVariableWritePermission.Owner);
    //private Transform _playerTransform;


    public override string ToString()
    {
        return Name;
    }

    private void Start()
    {
        GameManager.Instance.currentRace.AddPlayer(this);
        if (IsHost)
        {
            Camera.main.enabled = false;
        }
    }

    public override void OnNetworkSpawn()
    {
        SetupPlayer();
    }

    void SetupPlayer()
    {
        if (IsOwner)
        {
            _camera.SetActive(true);
            //_nPlayerPosition.OnValueChanged += OnPositionChange;
            //_nPlayerRotation.OnValueChanged += OnRotationChange;
            print(NetworkObject.OwnerClientId);
            GetComponent<PlayerInput>().enabled = true;
            InputController input = GetComponent<InputController>();

            Move.performed += input.OnMove;
            Move.Enable();

            Brake.performed += input.OnBrake;
            Brake.Enable();

            Attack.performed += input.OnBrake;
            Attack.Enable();

            //_playerTransform = transform;
            //_nPlayerPosition.OnValueChanged += OnPositionChange;
            //_nPlayerRotation.OnValueChanged += OnRotationChange;
        }
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