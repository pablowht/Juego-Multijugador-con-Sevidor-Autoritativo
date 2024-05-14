using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private GameObject _ui_Lobby;
    private GameObject _ui_GameInfo;
    //private TMP_InputField _playerNameInput;
    public TextMeshProUGUI _raceCodeUI;
    private TMP_InputField _raceCodeInput;
    private TextMeshProUGUI _userData;
    private TextMeshProUGUI _playerGameInfo;

    public static UIManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        _ui_Lobby = GameObject.FindGameObjectWithTag("UI_Lobby");
        foreach (Transform child in _ui_Lobby.transform)
        {
            if (child.tag == "RaceCode") _raceCodeInput = child.GetComponent<TMP_InputField>();
        }

        _ui_GameInfo = GameObject.FindGameObjectWithTag("UI_GameInfo");
        foreach (Transform child in _ui_GameInfo.transform)
        {
            if (child.tag == "RaceCode") _raceCodeUI = child.GetComponent<TextMeshProUGUI>();
            if (child.tag == "UserData") _userData = child.GetComponent<TextMeshProUGUI>();
        }

        _ui_Lobby.SetActive(true);
        _ui_GameInfo.SetActive(false);
    }

    void OnGUI()
    {
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            LobbyControlUI();
        }
        else
        {
            StatusLabels();
        }
    }

    private void LobbyControlUI()
    {
        _ui_Lobby.SetActive(true);
        //RelayManager.Instance.joinCode = _raceCodeInput.ToString();
        //_raceCodeUI.SetText(RelayManager.Instance.joinCode);
    }

    private void StatusLabels()
    {
        _ui_Lobby.SetActive(false);
        _ui_GameInfo.SetActive(true);

        var mode = NetworkManager.Singleton.IsHost ?
            "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";

        _userData.SetText(mode + " : PlayerID");
    }

    public void StartHostButton()
    {
        RelayManager.Instance.StartHost();
        //_raceCodeUI.SetText(RelayManager.Instance.joinCode);
    }

    public void StartClientButton()
    {
        print(_raceCodeInput.text);
        RelayManager.Instance.StartClient(_raceCodeInput.text);
    }
}
