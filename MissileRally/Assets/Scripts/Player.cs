using Cinemachine;
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

     private InputAction Move = new InputAction();
     private InputAction Brake = new InputAction();
     private InputAction Attack = new InputAction();


    //private NetworkVariable<Vector3> _nPlayerPosition = NetworkVariable<Vector3>()
    //private readonly NetworkVariable<Vector3> _nPlayerPosition = new(writePerm: NetworkVariableWritePermission.Owner);
    //private readonly NetworkVariable<Quaternion> _nPlayerRotation = new(writePerm: NetworkVariableWritePermission.Owner);
    //private Transform _playerTransform;
    CinemachineVirtualCamera _vCamera;



    public override string ToString()
    {
        return Name;
    }

    private void Start()
    {
        GameManager.Instance.currentRace.AddPlayer(this);
        //Move = GetComponent<PlayerInput>().actions.FindAction("Move");
        //Brake = GetComponent<PlayerInput>().actions.FindAction("Brake");
        //Attack = GetComponent<PlayerInput>().actions.FindAction("Attack");
        //_vCamera = GameManager.Instance._virtualCamera;
    }

    public override void OnNetworkSpawn()
    {
        SetupPlayer();
    }

    void SetupPlayer()
    {
        if (IsOwner)
        {
            _vCamera = GameManager.Instance._virtualCamera;
            print(_vCamera);
            _vCamera.Follow = car.GetComponent<Transform>();
            _vCamera.LookAt = car.GetComponent<Transform>();
            //_vCamera.gameObject.transform.rotation = new Quaternion(30.964f, 180, 0,0);
            //_vCamera.LookAt = new Transform(30.964f, 180, 0, 0);
            
            //_nPlayerPosition.OnValueChanged += OnPositionChange;
            //_nPlayerRotation.OnValueChanged += OnRotationChange;
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