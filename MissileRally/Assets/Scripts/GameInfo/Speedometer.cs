using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speedometer : MonoBehaviour
{
    void FixedUpdate()
    {
        UIManager.Instance.updateSpeedometer();
    }
}
