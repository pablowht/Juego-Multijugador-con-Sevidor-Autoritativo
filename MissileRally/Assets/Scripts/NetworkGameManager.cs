using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Windows;

public class NetworkGameManager : NetworkBehaviour //CONTROLADA UNICAMENTE POR EL SERVIDOR (con alg�n clienteRpc)
{
    private int serverCountCar = 0; //variable que cuenta el n�mero de coches listos
    private int[] carsCheckpoints = new int[6]; //array que guarda el �ltimo checkpoint de cada jugador seg�n su ID
    public CheckPoint[] checkpoints; //lista de checkpoints: depende de cada circuito
    public List<Player> currentPlayerInstance = new List<Player>(); //lista de los actuales players instanciados, guarda una referencia a ellos
        //no es la soluci�n m�s eficiente sobre todo en cuanto a buscar instancias de elementos (acceso secuencial)
    public string code; //c�digo de la partida

    [ServerRpc(RequireOwnership = false)] 
    public void IncrementCarReadyServerRpc() //el servidor gestiona el n�mero de players que est�n listos para comenzar
    {
        //Debug.Log($"Before Increment: {serverCountCar}");
        serverCountCar += 1;
        //Debug.Log($"After Increment: {serverCountCar}");

        UpdateUIClientRpc(serverCountCar); //actualiza la UI de los clientes (y �l mismo)

        if (serverCountCar >= 2) //la carrera empieza cuando hay al menos dos jugadores listos
        {
            GameManager.Instance.BeginRace(); //comienza la carrera para el server
            BeginRaceClientRpc(); //comienza la carrera para todos los jugadores
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void setJoinCodeServerRpc(string c) //guarda en el servidor el c�digo de la sala
    {
        //print("rpc");
        code = c;
        //print(code);
    }

    private string[] rankingList = new string[6]; //array que guarda los tiempos, nombre y orden de cada jugador

    [ServerRpc(RequireOwnership = false)]
    public void RaceFinishServerRpc(string playerName, string tiempoCarrera) //cuando un jugador cruza la meta en la ultima vuelta
    {
        //Escribe el jugador en la UI de ranking 
        rankingList[GameManager.Instance.finishedPlayers] = UIManager.Instance.RankingTextInRanking(GameManager.Instance.finishedPlayers, playerName, tiempoCarrera); 
            //guarda los datos del jugador que acaba de terminar la carrera
        GameManager.Instance.finishedPlayers++; // aumenta el numero de jugadores que han terminado 
        int i = 0;
        foreach (string rankingPos in rankingList)
        {
            RaceFinishClientRpc(i, rankingPos); //se envia a los clientes la informaci�n (solo se actualiza si el cliente ha acabado) del ranking para actualizarla
            i++;
        }
        
    }

    [ClientRpc]
    public void RaceFinishClientRpc(int i, string rankingPos) //actualiza la UI de los clientes
    {
        UIManager.Instance.WriteRankingUI(i, rankingPos);
    }

    [ClientRpc]
    public void sendCodeClientRpc(string r) //envia el c�digo de sala a los clientes para que se actualice en su UI y en su variable local
    {
        UIManager.Instance.joinCode = r;
        UIManager.Instance._raceCodeUI.SetText(r);
    }

    [ClientRpc]
    public void BeginRaceClientRpc() //empieza la carrera en los clientes que est�n listos
    {
        GameManager.Instance.BeginRace();
    }

    [ClientRpc]
    public void UpdateUIClientRpc(int c) //actualiza la UI en el cliente del n�mero de clientes listos
    {
        GameManager.Instance.carsReadyToRace = c;
        UIManager.Instance._numberCarReadyUI.SetText(GameManager.Instance.carsReadyToRace.ToString());
    }

    [ServerRpc]
    public void removeCarServerRpc() //disminuye en uno (siempre que sea mayor que cero) la variable de los coches listos
    {
        if (serverCountCar > 0)
        {
            serverCountCar--;
        }

        UpdateUIClientRpc(serverCountCar); //actualiza la UI del cliente
    }
    #region checkpoints

    [ServerRpc(RequireOwnership = false)]
    public void updateCheckpointServerRpc(int clientID, int last) //guarda el ultimo chechpoint por el que ha pasado el cliente (dentro del server)
    {
        //print("ultimo" + last);
        //print("id" + clientID);
        if (last != carsCheckpoints[clientID] - 1)
        //para evitar que haga en sentido contrario la carrera, solo se actualiza el �ltimo checkpoint siempre que este no sea uno menos que el acutal
        //ej: last: 2, carsCheckpoints[clientID] = 3 -> no entraria en la condici�n
        {
            carsCheckpoints[clientID] = last;
        }
        else //si vuelve para atr�s se respawnea
        {
            restorePositionServerRpc(clientID);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void restorePositionServerRpc(int idx) //restaura la posici�n del cliente en la posici�n asignada de su ultimo checkpoint
    {
        //print("moviendo...");
        //print("id: " + idx);
        currentPlayerInstance[idx].car.transform.position = checkpoints[carsCheckpoints[idx]].position.position; //le asigna la posicion de la posici�n del ultimo checkpoint
        currentPlayerInstance[idx].car.transform.rotation = checkpoints[carsCheckpoints[idx]].position.rotation; //le asigna la rotacion de la posici�n del ultimo checkpoint
        currentPlayerInstance[idx].carController._rigidbody.velocity = Vector3.zero; //pone a cero la velocidad del rigidbody para que no se instancien con velocidad/aceleraci�n
        currentPlayerInstance[idx].carController._rigidbody.angularVelocity = Vector3.zero; //pone a cero la velocidad del rigidbody para que no se instancien con velocidad/aceleraci�n
    }    
    #endregion
}
