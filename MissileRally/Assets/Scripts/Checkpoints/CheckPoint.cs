using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public Transform position; //posición donde se spawneará el coche
    public int cpName; //el nombre del checkpoint

    //para cuando el coche pase por un checkpoint
    private void OnTriggerEnter(Collider other)
    {
       if(other == GameManager.Instance.actualPlayer.car.GetComponent<BoxCollider>()) //si ha cruzado el coche actual (el local)
       {
            //actualiza el último checkpoint por el que ha pasado
            GameManager.Instance.ntGameInfo.updateCheckpointServerRpc(GameManager.Instance.actualPlayer.ID, cpName);
            // Si el checkpoint por el que se ha pasado no está visitado
            if (GameManager.Instance.actualPlayer.visitedCheckpoint[cpName] == false) //para la condición de contar una vuelta
            {
                GameManager.Instance.actualPlayer.count++; //le suma uno a el número de checkpoints visitados
                GameManager.Instance.actualPlayer.visitedCheckpoint[cpName] = true; //pone a true el checkpoint
            }
        
       }
    }
}
