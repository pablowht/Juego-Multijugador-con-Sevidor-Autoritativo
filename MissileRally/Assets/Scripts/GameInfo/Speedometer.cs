using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speedometer : MonoBehaviour
{
    void FixedUpdate()
    {
        UIManager.Instance.updateSpeedometer(); //para que el speedometer se updatee
        /*
         Para que solo se actualice cuando esté activo el speedmeter y no gastar recursos innecesarios en el UIManager
         */
    }
}
