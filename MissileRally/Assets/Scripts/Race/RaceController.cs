using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class RaceController : NetworkBehaviour
{
    public int numPlayers;

    public List<Player> _players = new(6);
    private CircuitController _circuitController;
    //private GameObject[] _debuggingSpheres;

    private void Start()
    {
        if (_circuitController == null) _circuitController = GetComponent<CircuitController>();

        //_debuggingSpheres = new GameObject[GameManager.Instance.numPlayers];
        //for (int i = 0; i < GameManager.Instance.numPlayers; ++i)
        //{
        //    _debuggingSpheres[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //    _debuggingSpheres[i].GetComponent<SphereCollider>().enabled = false;
        //}
    }

    private void Update()
    {
        if (_players.Count == 0)
            return;
        print("Count: " + _players.Count);
        
        if(IsServer){
            UpdateRaceProgress();
        }
    }

    public void AddPlayer(Player player)
    {
        _players.Add(player);
    }

    private class PlayerInfoComparer : Comparer<Player>
    {
        readonly float[] _arcLengths;

        public PlayerInfoComparer(float[] arcLengths)
        {
            _arcLengths = arcLengths;
        }

        public override int Compare(Player x, Player y)
        {
            //print("x ID: "+x.ID);
            //print("y ID: "+y.ID);

            if (x.distancia < y.distancia)
                return 1;
            else return -1;
        }
    }

    public void UpdateRaceProgress()
    {
        // Update car arc-lengths
        float[] arcLengths = new float[_players.Count];

        for (int i = 0; i < _players.Count; ++i)
        {
            arcLengths[i] = ComputeCarArcLength(i);
            _players[i].distancia = ComputeCarArcLength(i);
            //_players[i].distancia = ComputeCarArcLength(i);

            //print(arcLengths[i]);
        }

        //Array.Sort(arcLengths);
        //_players.Sort(new PlayerInfoComparer(arcLengths)); //******
        _players.Sort(new PlayerInfoComparer(arcLengths)); //******
        

        //string myRaceOrder = "";
        int raceCount = 0;
        foreach (var player in _players)
        {
            raceCount++;
            player.orderRaceClientRpc(raceCount);
            // myRaceOrder += player.ID + " ";
        }
        // Debug.Log("Race order: " + myRaceOrder);
    }

    float ComputeCarArcLength(int id)
    {
        // Compute the projection of the car position to the closest circuit 
        // path segment and accumulate the arc-length along of the car along
        // the circuit.
        Vector3 carPos = this._players[id].car.transform.position;


        float minArcL =
            this._circuitController.ComputeClosestPointArcLength(carPos, out _, out var carProj, out _);

        //this._debuggingSpheres[id].transform.position = carProj;

        if (this._players[id].CurrentLap == 0)
        {
            minArcL -= _circuitController.CircuitLength;
            
        }
        else
        {
            minArcL += _circuitController.CircuitLength *
                       (_players[id].CurrentLap);
            print("current lap else: "+_players[id].CurrentLap);
        }

        return minArcL;
    }

    
}