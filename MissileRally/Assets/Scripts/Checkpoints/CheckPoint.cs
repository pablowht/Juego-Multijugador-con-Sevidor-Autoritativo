using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public Transform position; //posici�n donde se spawnear� el coche
    public int cpName; //el nombre del checkpoint

    //para cuando el coche pase por un checkpoint
    private void OnTriggerEnter(Collider other)
    {
       if(other == GameManager.Instance.actualPlayer.car.GetComponent<BoxCollider>()) //si ha cruzado el coche actual (el local)
       {
            //actualiza el �ltimo checkpoint por el que ha pasado
            GameManager.Instance.ntGameInfo.updateCheckpointServerRpc(GameManager.Instance.actualPlayer.ID, cpName);
            // Si el checkpoint por el que se ha pasado no est� visitado
            if (GameManager.Instance.actualPlayer.visitedCheckpoint[cpName] == false) //para la condici�n de contar una vuelta
            {
                GameManager.Instance.actualPlayer.count++; //le suma uno a el n�mero de checkpoints visitados
                GameManager.Instance.actualPlayer.visitedCheckpoint[cpName] = true; //pone a true el checkpoint
            }
        
       }
    }
}
