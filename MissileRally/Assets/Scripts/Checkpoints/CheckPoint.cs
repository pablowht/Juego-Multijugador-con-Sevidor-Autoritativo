using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public Transform position;
    public int cpName;

    //para cuando el coche pase por un checkpoint
    private void OnTriggerEnter(Collider other)
    {
       if(other == GameManager.Instance.actualPlayer.car.GetComponent<BoxCollider>())
       {
            //actualiza el numero de checkpoints que ha pasado
            GameManager.Instance.ntGameInfo.updateCheckpointServerRpc(GameManager.Instance.actualPlayer.ID, cpName);
            if (GameManager.Instance.actualPlayer.visitedCheckpoint[cpName] == false)
            {
                GameManager.Instance.actualPlayer.count++;
                GameManager.Instance.actualPlayer.visitedCheckpoint[cpName] = true;
            }
        
       }
    }
}
