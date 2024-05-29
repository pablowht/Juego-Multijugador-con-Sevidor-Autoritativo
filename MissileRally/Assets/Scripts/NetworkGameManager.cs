using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkGameManager : NetworkBehaviour
{
    //public NetworkVariable<int> carsReadyToRace_ntw = new NetworkVariable<int>(0);
    //// Start is called before the first frame update
    //void Start()
    //{
    //    carsReadyToRace_ntw.OnValueChanged += OnCarsReadyChanged;
    //}

    //public void OnCarsReadyChanged(int previousValue, int newValue)
    //{
    //    GameManager.Instance.carsReadyToRace = newValue;
    //    Debug.Log($"Cars Ready Changed: {previousValue} -> {newValue}");
    //    print("cars ready: " + GameManager.Instance.carsReadyToRace);
    //    UIManager.Instance._numberCarReadyUI.SetText(newValue.ToString());
    //    if (GameManager.Instance.carsReadyToRace >= 2)
    //    {
    //        GameManager.Instance.BeginRace();
    //    }
    //}

    //public void IncrementCarReady()
    //{
    //    if (IsServer)
    //    {
    //        print("soy owner");
    //        Debug.Log($"Before Increment: {carsReadyToRace_ntw.Value}");
    //        carsReadyToRace_ntw.Value += 1;
    //        Debug.Log($"After Increment: {carsReadyToRace_ntw.Value}");
    //        OnCarsReadyChanged(0, carsReadyToRace_ntw.Value);
    //    }
    //    else
    //    {
    //        print("else");
    //        OnCarsReadyChanged(0, carsReadyToRace_ntw.Value);
    //    }
    //}

    [ServerRpc(RequireOwnership = false)]
    public void IncrementCarReadyServerRpc()
    {
        Debug.Log($"Before Increment: {GameManager.Instance.carsReadyToRace}");
        GameManager.Instance.carsReadyToRace += 1;
        Debug.Log($"After Increment: {GameManager.Instance.carsReadyToRace}");

        UIManager.Instance._numberCarReadyUI.SetText(GameManager.Instance.carsReadyToRace.ToString());
        if (GameManager.Instance.carsReadyToRace >= 2)
        {
            GameManager.Instance.BeginRace();
        }
    }
}
