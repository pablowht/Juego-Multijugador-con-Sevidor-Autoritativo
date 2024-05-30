using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public Transform position;
    public int cpName;

    private void OnTriggerEnter(Collider other)
    {
       if(other == GameManager.Instance.actualPlayer.car.GetComponent<BoxCollider>())
       {
            GameManager.Instance.ntGameInfo.updateCheckpointServerRpc(GameManager.Instance.actualPlayer.ID, cpName);
            if (GameManager.Instance.actualPlayer.visitedCheckpoint[cpName] == false)
            {
                GameManager.Instance.actualPlayer.count++;
                GameManager.Instance.actualPlayer.visitedCheckpoint[cpName] = true;
            }
            //print("Checkpoints visitados: " + GameManager.Instance.actualPlayer.count);
       }
    }
}
