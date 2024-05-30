using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Windows;

public class NetworkGameManager : NetworkBehaviour
{
    private int serverCountCar = 0;
    private int[] carsCheckpoints = new int[4]; 
    public CheckPoint[] checkpoints;
    public List<Player> currentPlayerInstance = new List<Player>();
    //ELEFANTE: no es la solución más eficiente sobre todo en cuanto a escalabilidad

    #region Ready Button and input activate
    //ELEFANTE ENORME, ELEFANTON
    [ServerRpc(RequireOwnership = false)]
    public void IncrementCarReadyServerRpc()
    {
        //Debug.Log($"Before Increment: {GameManager.Instance.carsReadyToRace}");
        //GameManager.Instance.carsReadyToRace += 1;
        //Debug.Log($"After Increment: {GameManager.Instance.carsReadyToRace}");
        Debug.Log($"Before Increment: {serverCountCar}");
        serverCountCar += 1;
        Debug.Log($"After Increment: {serverCountCar}");

        UpdateUIClientRpc(serverCountCar);

        if (serverCountCar >= 1)
        {
            //CORRUTINA QUE ESPERE X SEGUNDOS DEL SEMÁFORO
            GameManager.Instance.BeginRace();
            //GameManager.Instance.EnablePlayerInputs();
            activateInputClientRpc();
        }
    }

    [ClientRpc]
    public void UpdateUIClientRpc(int c)
    {
        GameManager.Instance.carsReadyToRace = c;
        UIManager.Instance._numberCarReadyUI.SetText(GameManager.Instance.carsReadyToRace.ToString());
    }

    [ClientRpc]
    public void activateInputClientRpc()
    {
        GameManager.Instance.actualPlayer.EnablePlayerInput();
        UIManager.Instance.DisableUIToStartRace();
    }    
    
    [ClientRpc(RequireOwnership = false)]
    public void updateSpeedClientRpc(float newVal)
    {
        print("update");
        //GameManager.Instance.actualPlayerInfo.playerSpeed = newVal;
        //print("velo ClientRpc: " + GameManager.Instance.actualPlayerInfo.playerSpeed);
    }

    [ServerRpc]
    public void removeCarServerRpc()
    {
        if(serverCountCar > 0)
        {
            serverCountCar--;
        }
        UpdateUIClientRpc(serverCountCar);
    }
    #endregion

    #region checkpoints

    [ServerRpc(RequireOwnership = false)]
    public void updateCheckpointServerRpc(int clientID, int last)
    {
        print("ultimo"+last);
        print("id"+clientID);
        //if(last != carsCheckpoints[clientID] - 1 || (last != checkpoints.Length-1 & carsCheckpoints[clientID] != 0))
        if(last != carsCheckpoints[clientID] - 1)
        {
            carsCheckpoints[clientID] = last;
        }
        else
        {
            restorePositionServerRpc(clientID);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void restorePositionServerRpc(int idx)
    {
        //ELEFANTE: hay que ver el tema de si dos coches se estampan a la vez, ambos se respawnean en misma posición -> collisionan
        //bucle de instanciarse y xd

        print("moviendo...");
        print("id: "+idx);
        GameManager.Instance.ntGameInfo.currentPlayerInstance[idx].car.transform.position = checkpoints[carsCheckpoints[idx]].position.position;
        GameManager.Instance.ntGameInfo.currentPlayerInstance[idx].car.transform.rotation = checkpoints[carsCheckpoints[idx]].position.rotation;
        GameManager.Instance.ntGameInfo.currentPlayerInstance[idx].carController._rigidbody.velocity = Vector3.zero;
        GameManager.Instance.ntGameInfo.currentPlayerInstance[idx].carController._rigidbody.angularVelocity = Vector3.zero;
        //moveClientRpc(idx, checkpoints[carsCheckpoints[idx]].position.rotation, checkpoints[carsCheckpoints[idx]].position.position);
    }

    [ClientRpc]
    public void collisionOccurredClientRpc()
    {
        if (IsOwner)
        {
            print(GameManager.Instance.actualPlayer.ID);
            //restorePositionServerRpc(GameManager.Instance.actualPlayer.ID);
        }
    }
    #endregion
}
