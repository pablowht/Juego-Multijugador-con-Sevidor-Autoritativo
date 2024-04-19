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
    public int CurrentPosition { get; set; }
    public int CurrentLap { get; set; }

    public InputAction Move;
    public InputAction Brake;
    public InputAction Attack;


    public override string ToString()
    {
        return Name;
    }

    private void Start()
    {
        GameManager.Instance.currentRace.AddPlayer(this);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        SetupPlayer();
    }

    void SetupPlayer()
    {
        if (IsOwner)
        {
            GetComponent<PlayerInput>().enabled = true;
            InputController input = GetComponent<InputController>();
            Move.performed += input.OnMove;
            Move.Enable();

            Brake.performed += input.OnBrake;
            Brake.Enable();

            Attack.performed += input.OnBrake;
            Attack.Enable();
            //Asignar la camara, como un hijo del player
        }
    }
}