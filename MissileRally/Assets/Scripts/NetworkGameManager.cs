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
    public string code;
    //ELEFANTE: no es la solución más eficiente sobre todo en cuanto a escalabilidad

    #region Ready Button and input activate
    //ELEFANTE ENORME, ELEFANTON
    [ServerRpc(RequireOwnership = false)]
    public void IncrementCarReadyServerRpc()
    {
        Debug.Log($"Before Increment: {serverCountCar}");
        serverCountCar += 1;
        Debug.Log($"After Increment: {serverCountCar}");

        UpdateUIClientRpc(serverCountCar);

        if (serverCountCar >= 1)
        {
            GameManager.Instance.BeginRace();
            BeginRaceClientRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void setJoinCodeServerRpc(string c)
    {
        print("rpc");
        code = c;
        print(code);
    }    
    
    [ClientRpc]
    public void sendCodeClientRpc(string r)
    {
        UIManager.Instance.joinCode = r;
        UIManager.Instance._raceCodeUI.SetText(r);
    }

    [ClientRpc]
    public void BeginRaceClientRpc()
    {
        //UIManager.Instance._waitingPlayersText.SetText("Race Starting...");
        GameManager.Instance.BeginRace();
        //GameManager.Instance.StartCoroutine(UpdateSemaphoreOrange());
        //StartCoroutine(UpdateSemaphoreOrange());
    }

    //IEnumerator UpdateSemaphoreOrange()
    //{
    //    UIManager.Instance._semaphore.UpdateToRed();
    //    yield return new WaitForSeconds(2);
    //    UIManager.Instance._semaphore.UpdateToOrange();
    //    StartCoroutine(UpdateSemaphoreGreen());
    //}
    //IEnumerator UpdateSemaphoreGreen()
    //{
    //    yield return new WaitForSeconds(2);
    //    UIManager.Instance._semaphore.UpdateToGreen();
    //    activateInputClientRpc();
    //    UIManager.Instance.DisableUIToStartRace();
    //    StartCoroutine(StopSemaphore());
    //}

    //IEnumerator StopSemaphore()
    //{
    //    yield return new WaitForSeconds(1);
    //    UIManager.Instance._semaphoreCamera.SetActive(false);
    //}

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
        if (serverCountCar > 0)
        {
            serverCountCar--;
        }
        UpdateUIClientRpc(serverCountCar);
    }
    #endregion

    #region checkpoints
    //ELEFANTE - quitar el cliente de la lista de prefabs cuando se desconecte

    [ServerRpc(RequireOwnership = false)]
    public void updateCheckpointServerRpc(int clientID, int last)
    {
        print("ultimo" + last);
        print("id" + clientID);
        //if(last != carsCheckpoints[clientID] - 1 || (last != checkpoints.Length-1 & carsCheckpoints[clientID] != 0))
        if (last != carsCheckpoints[clientID] - 1)
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
        print("id: " + idx);
        currentPlayerInstance[idx].car.transform.position = checkpoints[carsCheckpoints[idx]].position.position;
        currentPlayerInstance[idx].car.transform.rotation = checkpoints[carsCheckpoints[idx]].position.rotation;
        currentPlayerInstance[idx].carController._rigidbody.velocity = Vector3.zero;
        currentPlayerInstance[idx].carController._rigidbody.angularVelocity = Vector3.zero;
    }    
    
    [ServerRpc(RequireOwnership = false)]
    public void reduceVelocityServerRpc(int idx)
    {
        //COMENTAR
        print("reduciendo...");
        currentPlayerInstance[idx].coefRed = 0.5f;
        //si pongo aqui una corrutina no funciona
    }
    #endregion
}
