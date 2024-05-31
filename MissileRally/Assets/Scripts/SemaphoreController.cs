using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SemaphoreController : MonoBehaviour
{
    //Clase para cambiar el color del material del semáforo, para empezar la carrera
    Color _red = new Color(1.0f, 0.0f, 0.13f);
    Color _orange = new Color(1.0f, 0.34f, 0.0f);
    Color _green = new Color(0.39f, 1.0f, 0.0f);
    [SerializeField] private Material _semaphoreMaterial;

    public void UpdateToOrange()
    {
        _semaphoreMaterial.SetColor("_Color", _orange);
    }
    public void UpdateToGreen()
    {
        _semaphoreMaterial.SetColor("_Color", _green);
    }
    public void UpdateToRed()
    {
        _semaphoreMaterial.SetColor("_Color", _red);
    }
}
