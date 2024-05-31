using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.Netcode;
using UnityEngine;

public class MetaController : MonoBehaviour
{
    //public int lapCount;
    public int maxLaps = 3; //ELEFANTE cambiar a 3


    private void OnTriggerEnter(Collider other)
    {
        //si el coche colisiona con la meta 
        if (other == GameManager.Instance.actualPlayer.car.GetComponent<BoxCollider>())  
        {
            //GameManager.Instance.ntGameInfo.restorePositionServerRpc(GameManager.Instance.actualPlayer.ID);
            print("Entro al trigger");
            //por cada vuelta que da el coche se hace el if
            if (GameManager.Instance.actualPlayer.CurrentLap < maxLaps - 1) //ELEFANTE: tiene qeu ser -1 jeje, empieza en 0
            {
                print("lapCount: " + GameManager.Instance.actualPlayer.CurrentLap + " < maxLaps");
                print("count " + GameManager.Instance.actualPlayer.count);
                print("len: " + GameManager.Instance.ntGameInfo.checkpoints.Length);
                //si en la vuelta el coche ha pasado por todos los checkpoints, entra en el if
                if (GameManager.Instance.actualPlayer.count == GameManager.Instance.ntGameInfo.checkpoints.Length)
                {
                   
                    GameManager.Instance._chronometer.UpdateLapTime(GameManager.Instance.actualPlayer.CurrentLap); //actualiza el tiempo dado en esa vuelta
                    resetArray();// resetea los chekpoints visitados y los pone a 0
                    GameManager.Instance.actualPlayer.count = 0; //porque cuando pasas por la meta acaba de pasar por un checkpoint
                    //GameManager.Instance.actualPlayer.count = 1; //porque cuando pasas por la meta acaba de pasar por un checkpoint
                    GameManager.Instance.actualPlayer.CurrentLap++;
                }
            }
            else//cuando haya hecho todas las vueltas
            {
             
                //ELEFANTE:
                /*
                 - en la misma escena: se bloquea el player input de los jugadores y se pone encima la UI
                 - en otra escena: se llama a un serverRPC que cambie la escena (no se si habría que quitar el input igualmente)
                 */
                UIManager.Instance._semaphore.UpdateToRed();//pone el semaforo a rojo
                GameManager.Instance._chronometer.UpdateLapTime(maxLaps - 1);//actualiza el tiempo de la ultima vuelta
                GameManager.Instance._chronometer.StopChronometer();// para el cronometro

                UIManager.Instance.OpenRanking();//aparece en la pantalla el ranking 
                //actualiza la UI del ranking y los jugadores que han terminado 
                GameManager.Instance.ntGameInfo.RaceFinishServerRpc(GameManager.Instance.actualPlayer.Name, UIManager.Instance._chronometer.GetComponent<Chronometer>()._stringLapTimes[2].ToString());
            }
        }
    }
    void resetArray()//resetea el aray de checkpoints visitados 
    {
        for(int i = 0; i < GameManager.Instance.ntGameInfo.checkpoints.Length; i++)
        {
            GameManager.Instance.actualPlayer.visitedCheckpoint[i] = false;
        }
    }
}
