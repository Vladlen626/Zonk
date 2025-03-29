using Mirror;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HybridLobbyManager : MonoBehaviour
{
    [Header("Transport Settings")]
    [SerializeField] private Transport steamTransport;
    [SerializeField] private Transport localTransport; // Например, Telepathy

    [Header("UI Elements")]
    [SerializeField] private Button hostButton;
    [SerializeField] private Button joinButton;
    [SerializeField] private TMP_InputField lobbyCodeInput;
    [SerializeField] private Text connectionStatusText;

    private NetworkManager networkManager;
    private bool useSteamworks = false;

    private void Awake()
    {
        networkManager = GetComponent<NetworkManager>();
        ConfigureTransport();

        hostButton.onClick.AddListener(StartHost);
        joinButton.onClick.AddListener(StartClient);
    }

    private void ConfigureTransport()
    {
        #if UNITY_EDITOR
        // В редакторе используем локальный транспорт
        useSteamworks = false;
        networkManager.transport = localTransport;
        connectionStatusText.text = "Режим: Локальный транспорт (Editor)";
        #else
        // В билде используем Steamworks
        useSteamworks = true;
        networkManager.transport = steamTransport;
        connectionStatusText.text = "Режим: Steam Transport";
        #endif
    }

    private void StartHost()
    {
        if (useSteamworks)
        {
            // Steamworks логика
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, 4);
        }
        else
        {
            // Локальная логика
            networkManager.StartHost();
            connectionStatusText.text = "Хост запущен. Адрес: localhost";
        }
    }

    private void StartClient()
    {
        if (useSteamworks)
        {
            if (ulong.TryParse(lobbyCodeInput.text, out ulong lobbyID))
            {
                SteamMatchmaking.JoinLobby(new CSteamID(lobbyID));
            }
        }
        else
        {
            networkManager.networkAddress = string.IsNullOrEmpty(lobbyCodeInput.text) ? 
                "localhost" : lobbyCodeInput.text;
            networkManager.StartClient();
            connectionStatusText.text = $"Подключение к {networkManager.networkAddress}...";
        }
    }

    // Для работы с Steam Callbacks
    private void OnEnable()
    {
        if (useSteamworks)
        {
            Callback<LobbyCreated_t>.Create(OnLobbyCreated);
            Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        }
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult == EResult.k_EResultOK)
        {
            networkManager.StartHost();
            SteamMatchmaking.SetLobbyData(
                new CSteamID(callback.m_ulSteamIDLobby), 
                "HostAddress", 
                SteamUser.GetSteamID().ToString()
            );
            connectionStatusText.text = $"Steam лобби создано. ID: {callback.m_ulSteamIDLobby}";
        }
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        if (!NetworkServer.active)
        {
            string hostAddress = SteamMatchmaking.GetLobbyData(
                new CSteamID(callback.m_ulSteamIDLobby), 
                "HostAddress"
            );
            networkManager.networkAddress = hostAddress;
            networkManager.StartClient();
        }
    }
}
