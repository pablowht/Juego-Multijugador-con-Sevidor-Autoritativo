using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Windows;

public class NetworkGameManager : NetworkBehaviour
{
    private int serverCountCar = 0;
    private int[] carsCheckpoints = new int[4]; 
    public CheckPoint[] checkpoints = new CheckPoint[20]; 
    //ELEFANTE: no es la soluci�n m�s eficiente sobre todo en cuanto a escalabilidad

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
            //CORRUTINA QUE ESPERE X SEGUNDOS DEL SEM�FORO
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
        carsCheckpoints[clientID] = last;
        print(last);
    }

    [ServerRpc(RequireOwnership = false)]
    public void restorePositionServerRpc(int idx)
    {
        //print("antes: " + GameManager.Instance.actualPlayer.car.transform.position);
        //GameManager.Instance.actualPlayer.car.transform.position = checkpoints[carsCheckpoints[idx]].position.position;
        //print("pos: " + GameManager.Instance.actualPlayer.car.transform.position);
        //GameManager.Instance.actualPlayer.carController.InputAcceleration = 0;
        //GameManager.Instance.actualPlayer.carController.InputSteering = 0;
        //ELEFANTE: hay que ver el tema de si dos coches se estampan a la vez, ambos se respawnean en misma posici�n -> collisionan
        //bucle de instanciarse y xd
        print(idx);
        print(GameManager.Instance.actualPlayer.ID);
        moveClientRpc(idx, checkpoints[carsCheckpoints[idx]].position.position);
    }

    [ClientRpc]
    public void moveClientRpc(int idx, Vector3 position)
    {
        if (idx == GameManager.Instance.actualPlayer.ID)
        {
            print(idx);
            print("moviendo al checkpoint...");
            GameManager.Instance.actualPlayer.car.transform.position = position;
            GameManager.Instance.actualPlayer.carController.InputAcceleration = 0;
            GameManager.Instance.actualPlayer.carController.InputSteering = 0;
            print(GameManager.Instance.actualPlayer.carController.InputAcceleration);
            print(GameManager.Instance.actualPlayer.carController.InputSteering);
        }
    }
    #endregion
}
