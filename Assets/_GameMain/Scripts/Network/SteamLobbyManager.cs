using Mirror;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SteamLobbyManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button hostButton;
    [SerializeField] private Button joinButton;
    [SerializeField] private TMP_InputField lobbyCodeInput;
    [SerializeField] private GameObject networkUI;
    [SerializeField] private TMP_Text lobbyCodeText;
    [SerializeField] private Button copyCodeButton;

    [Header("Debug Settings")]
    [SerializeField] private bool enableDebugMode = true;
    [SerializeField] private string debugHostAddress = "localhost";

    private NetworkManager networkManager;
    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> lobbyEntered;

    private const string HostAddressKey = "HostAddress";
    private bool useSteamworks = true;

    private void Start()
    {
        networkManager = GetComponent<NetworkManager>();
        ConfigureTransportMode();

        hostButton.onClick.AddListener(HostLobby);
        joinButton.onClick.AddListener(JoinLobby);
        copyCodeButton.onClick.AddListener(CopyLobbyCode);

        if (useSteamworks && !SteamManager.Initialized) 
        {
            Debug.LogError("Steam не инициализирован!");
            return;
        }

        InitializeSteamCallbacks();
    }

    private void ConfigureTransportMode()
    {
        #if UNITY_EDITOR
        useSteamworks = !enableDebugMode;
        networkUI.SetActive(!enableDebugMode || !useSteamworks);
        #else
        useSteamworks = true;
        networkUI.SetActive(true);
        #endif
    }

    private void InitializeSteamCallbacks()
    {
        if (!useSteamworks) return;

        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
    }

    private void HostLobby()
    {
        if (useSteamworks)
        {
            hostButton.gameObject.SetActive(false);
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, networkManager.maxConnections);
        }
        else
        {
            networkManager.StartHost();
            lobbyCodeText.text = "Локальный хост запущен";
            lobbyCodeText.gameObject.SetActive(true);
            copyCodeButton.gameObject.SetActive(false);
            networkUI.SetActive(false);
            Debug.Log("Локальный хост запущен");
        }
    }

    private void JoinLobby()
    {
        if (useSteamworks)
        {
            if (ulong.TryParse(lobbyCodeInput.text, out ulong lobbyID))
            {
                SteamMatchmaking.JoinLobby(new CSteamID(lobbyID));
            }
            else
            {
                Debug.LogError("Неверный формат кода лобби.");
            }
        }
        else
        {
            networkManager.networkAddress = string.IsNullOrEmpty(lobbyCodeInput.text) ? 
                debugHostAddress : lobbyCodeInput.text;
            networkManager.StartClient();
            networkUI.SetActive(false);
            Debug.Log($"Подключение к {networkManager.networkAddress}");
        }
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK)
        {
            hostButton.gameObject.SetActive(true);
            return;
        }

        CSteamID lobbyID = new CSteamID(callback.m_ulSteamIDLobby);
        lobbyCodeText.text = "Код лобби: " + lobbyID.m_SteamID;
        lobbyCodeText.gameObject.SetActive(true);
        copyCodeButton.gameObject.SetActive(true);

        networkManager.StartHost();
        SteamMatchmaking.SetLobbyData(lobbyID, HostAddressKey, SteamUser.GetSteamID().ToString());
        networkUI.SetActive(false);
    }

    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        if (NetworkServer.active) return;

        string hostAddress = SteamMatchmaking.GetLobbyData(
            new CSteamID(callback.m_ulSteamIDLobby), 
            HostAddressKey
        );

        networkManager.networkAddress = hostAddress;
        networkManager.StartClient();
        networkUI.SetActive(false);
    }

    private void CopyLobbyCode()
    {
        GUIUtility.systemCopyBuffer = lobbyCodeText.text.Replace("Код лобби: ", "");
        Debug.Log("Код лобби скопирован в буфер обмена.");
    }

    // Для тестирования в редакторе
    public void ToggleDebugMode(bool debugEnabled)
    {
        enableDebugMode = debugEnabled;
        ConfigureTransportMode();
    }
}