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
            GameManager.Instance.actualPlayer.visitedCheckpoints++;
            print("Checkpoints visitados: " + GameManager.Instance.actualPlayer.visitedCheckpoints);
       }
    }
}
