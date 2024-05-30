using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class MetaController : MonoBehaviour
{
    public int lapCount = 0;
    public int maxLaps = 3;

    private void OnTriggerEnter(Collider other)
    {
        if (other == GameManager.Instance.actualPlayer.car.GetComponent<BoxCollider>())
        {
            print("Entro al trigger");
            if (lapCount < maxLaps-1)
            {
                print("lapCount: " + lapCount + " < maxLaps");
                if (GameManager.Instance.actualPlayer.visitedCheckpoints-1 == GameManager.Instance.ntGameInfo.checkpoints.Length)
                {
                    print("Checkpoints visitados - 1: " + (GameManager.Instance.actualPlayer.visitedCheckpoints - 1) + " = " + GameManager.Instance.ntGameInfo.checkpoints.Length);
                    GameManager.Instance._chronometer.UpdateLapTime(lapCount);
                    GameManager.Instance.actualPlayer.visitedCheckpoints = 1; //porque cuando pasas por la meta acaba de pasar por un checkpoint
                    lapCount++;
                }
            }
            else { GameManager.Instance._chronometer.StopChronometer(); }
        }
    }
}
