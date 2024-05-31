using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.Netcode;
using UnityEngine;

public class MetaController : MonoBehaviour
{
    public int maxLaps = 3; 


    private void OnTriggerEnter(Collider other)
    {
        //si el coche local colisiona con la meta 
        if (other == GameManager.Instance.actualPlayer.car.GetComponent<BoxCollider>())  
        {
            print("Entro al trigger");
            //por cada vuelta que da el coche se hace el if
            if (GameManager.Instance.actualPlayer.CurrentLap < maxLaps - 1)
            {
                //print("lapCount: " + GameManager.Instance.actualPlayer.CurrentLap + " < maxLaps");
                //print("count " + GameManager.Instance.actualPlayer.count);
                //print("len: " + GameManager.Instance.ntGameInfo.checkpoints.Length);

                //si en la vuelta el coche ha pasado por todos los checkpoints, entra en el if
                if (GameManager.Instance.actualPlayer.count == GameManager.Instance.ntGameInfo.checkpoints.Length)
                {
                    GameManager.Instance._chronometer.UpdateLapTime(GameManager.Instance.actualPlayer.CurrentLap); 
                        //imprime en la UI el momento de tiempo justo en el que termina la vuelta al pasar por la meta
                    resetArray();// resetea los chekpoints visitados y los pone a 0
                    GameManager.Instance.actualPlayer.count = 0; //resetea la variable de los checkpoints visitados para una nueva vuelta
                    GameManager.Instance.actualPlayer.CurrentLap++; //suma uno a las vueltas que ha dado el jugador
                }
            }
            else//cuando haya hecho todas las vueltas
            { 
                UIManager.Instance._semaphore.UpdateToRed(); //pone el semaforo a rojo para restaurar el color del material
                GameManager.Instance._chronometer.UpdateLapTime(maxLaps - 1); //muestra el tiempo de la ultima vuelta
                GameManager.Instance._chronometer.StopChronometer(); //para el cronometro

                UIManager.Instance.OpenRanking();//muestra el ranking 
                //actualiza la UI del ranking y los jugadores que han terminado en el servidor
                GameManager.Instance.ntGameInfo.RaceFinishServerRpc(GameManager.Instance.actualPlayer.Name, UIManager.Instance._chronometer.GetComponent<Chronometer>()._stringLapTimes[2].ToString());
            }
        }
    }
    void resetArray() //resetea el aray de checkpoints visitados 
    {
        for(int i = 0; i < GameManager.Instance.ntGameInfo.checkpoints.Length; i++)
        {
            GameManager.Instance.actualPlayer.visitedCheckpoint[i] = false;
        }
    }
}
