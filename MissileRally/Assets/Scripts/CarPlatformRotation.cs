using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarPlatformRotation : MonoBehaviour
{
    void FixedUpdate()
    {
        // Rotaci�n de la plataforma en 0.5 por segundo
        transform.Rotate(0, 0.5f, 0);
    }
}
