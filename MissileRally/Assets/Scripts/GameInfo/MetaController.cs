using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.Netcode;
using UnityEngine;

public class MetaController : MonoBehaviour
{
    public int lapCount = 0;
    public int maxLaps = 3;

    private void OnTriggerEnter(Collider other)
    {
        //if(collision.gameObject.layer == 8)         metaCrossedClientRpc();
        //metaCrossedClientRpc();
        print(other == GameManager.Instance.actualPlayer.car.GetComponent<BoxCollider>());
        print(GameManager.Instance.actualPlayer.ID);
        if (other == GameManager.Instance.actualPlayer.car.GetComponent<BoxCollider>())    //ELEFANTE: si se llama al del network game manager NO FUNCIONA, si se hace aqui si, no se
        {
            //GameManager.Instance.ntGameInfo.restorePositionServerRpc(GameManager.Instance.actualPlayer.ID);
            print("Entro al trigger");
            if (lapCount < maxLaps - 1) //ELEFANTE: tiene qeu ser -1 jeje, empieza en 0
            {
                print("lapCount: " + lapCount + " < maxLaps");
                print("count " + GameManager.Instance.actualPlayer.count);
                print("len: " + GameManager.Instance.ntGameInfo.checkpoints.Length);
                if (GameManager.Instance.actualPlayer.count == GameManager.Instance.ntGameInfo.checkpoints.Length)
                {
                    //print("Checkpoints visitados - 1: " + (GameManager.Instance.actualPlayer.count - 1) + " = " + GameManager.Instance.ntGameInfo.checkpoints.Length);
                    GameManager.Instance._chronometer.UpdateLapTime(lapCount);
                    resetArray();
                    GameManager.Instance.actualPlayer.count = 0; //porque cuando pasas por la meta acaba de pasar por un checkpoint
                    //GameManager.Instance.actualPlayer.count = 1; //porque cuando pasas por la meta acaba de pasar por un checkpoint
                    lapCount++;
                }
            }
            else
            {
                print("acabó");
                //ELEFANTE:
                /*
                 - en la misma escena: se bloquea el player input de los jugadores y se pone encima la UI
                 - en otra escena: se llama a un serverRPC que cambie la escena (no se si habría que quitar el input igualmente)
                 */
                UIManager.Instance._semaphore.UpdateToRed();
                GameManager.Instance._chronometer.UpdateLapTime(maxLaps - 1);
                GameManager.Instance._chronometer.StopChronometer();
            }
        }
    }


    //[ClientRpc]
    //public void metaCrossedClientRpc()
    //{
    //    if (IsOwner)// & other == GameManager.Instance.actualPlayer.car.GetComponent<BoxCollider>())    //ELEFANTE: si se llama al del network game manager NO FUNCIONA, si se hace aqui si, no se
    //    {
    //        //GameManager.Instance.ntGameInfo.restorePositionServerRpc(GameManager.Instance.actualPlayer.ID);
    //        print("Entro al trigger");
    //        if (lapCount < maxLaps - 1) //ELEFANTE: tiene qeu ser -1 jeje, empieza en 0
    //        {
    //            print("lapCount: " + lapCount + " < maxLaps");
    //            print("count " + GameManager.Instance.actualPlayer.count);
    //            print("len: " + GameManager.Instance.ntGameInfo.checkpoints.Length);
    //            if (GameManager.Instance.actualPlayer.count == GameManager.Instance.ntGameInfo.checkpoints.Length)
    //            {
    //                //print("Checkpoints visitados - 1: " + (GameManager.Instance.actualPlayer.count - 1) + " = " + GameManager.Instance.ntGameInfo.checkpoints.Length);
    //                GameManager.Instance._chronometer.UpdateLapTime(lapCount);
    //                resetArray();
    //                GameManager.Instance.actualPlayer.count = 0; //porque cuando pasas por la meta acaba de pasar por un checkpoint
    //                //GameManager.Instance.actualPlayer.count = 1; //porque cuando pasas por la meta acaba de pasar por un checkpoint
    //                lapCount++;
    //            }
    //        }
    //        else
    //        {
    //            print("acabó");
    //            //ELEFANTE:
    //            /*
    //             - en la misma escena: se bloquea el player input de los jugadores y se pone encima la UI
    //             - en otra escena: se llama a un serverRPC que cambie la escena (no se si habría que quitar el input igualmente)
    //             */
    //            UIManager.Instance._semaphore.UpdateToRed();
    //            GameManager.Instance._chronometer.UpdateLapTime(maxLaps - 1);
    //            GameManager.Instance._chronometer.StopChronometer();
    //        }
    //    }
    //}

    void resetArray()
    {
        for(int i = 0; i < GameManager.Instance.ntGameInfo.checkpoints.Length; i++)
        {
            GameManager.Instance.actualPlayer.visitedCheckpoint[i] = false;
            print("seteo");
        }
    }
}
