using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class LobbyUIManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject createLobbyPanel;
    [SerializeField] private GameObject lobbyListPanel;
    [SerializeField] private GameObject lobbyRoomPanel;
    
    [Header("Main Menu")]
    [SerializeField] private Button createLobbyButton;
    [SerializeField] private Button joinLobbyButton;
    
    [Header("Create Lobby")]
    [SerializeField] private TMP_InputField lobbyNameInput;
    [SerializeField] private Button createButton;
    [SerializeField] private Button backFromCreateButton;
    
    [Header("Lobby List")]
    [SerializeField] private Transform lobbyListContent;
    [SerializeField] private GameObject lobbyEntryPrefab;
    [SerializeField] private Button refreshButton;
    [SerializeField] private Button backFromListButton;
    
    [Header("Lobby Room")]
    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private TextMeshProUGUI playerCountText;
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button leaveLobbyButton;
    
    private List<GameObject> lobbyEntries = new List<GameObject>();
    
    private void Start()
    {
        // Set up button listeners
        createLobbyButton.onClick.AddListener(() => ShowPanel(createLobbyPanel));
        joinLobbyButton.onClick.AddListener(() => ShowPanel(lobbyListPanel));
        
        backFromCreateButton.onClick.AddListener(() => ShowPanel(mainMenuPanel));
        backFromListButton.onClick.AddListener(() => ShowPanel(mainMenuPanel));
        
        createButton.onClick.AddListener(CreateLobby);
        refreshButton.onClick.AddListener(RefreshLobbyList);
        
        startGameButton.onClick.AddListener(() => NetworkLobbyManager.Instance.StartGame());
        leaveLobbyButton.onClick.AddListener(() => {
            NetworkLobbyManager.Instance.LeaveLobby();
            ShowPanel(mainMenuPanel);
        });
        
        // Initially show main menu
        ShowPanel(mainMenuPanel);
    }
    
    private void ShowPanel(GameObject panel)
    {
        mainMenuPanel.SetActive(panel == mainMenuPanel);
        createLobbyPanel.SetActive(panel == createLobbyPanel);
        lobbyListPanel.SetActive(panel == lobbyListPanel);
        lobbyRoomPanel.SetActive(panel == lobbyRoomPanel);
        
        if (panel == lobbyListPanel)
        {
            RefreshLobbyList();
        }
    }
    
    private void CreateLobby()
    {
        string lobbyName = lobbyNameInput.text;
        if (string.IsNullOrEmpty(lobbyName))
        {
            lobbyName = "Lobby " + UnityEngine.Random.Range(1000, 9999);
        }
        
        NetworkLobbyManager.Instance.CreateLobby(lobbyName);
        
        // Update UI
        lobbyNameText.text = lobbyName;
        UpdatePlayerCount(1, 8);
        startGameButton.gameObject.SetActive(true); // Only host can start
        
        ShowPanel(lobbyRoomPanel);
    }
    
    public void JoinLobby(string lobbyId)
    {
        NetworkLobbyManager.Instance.JoinLobby(lobbyId);
        
        // Find the lobby info to update UI
        if (LobbyManager.ActiveLobbies.TryGetValue(lobbyId, out LobbyInfo lobbyInfo))
        {
            lobbyNameText.text = lobbyInfo.lobbyName;
            UpdatePlayerCount(lobbyInfo.currentPlayers, lobbyInfo.maxPlayers);
            startGameButton.gameObject.SetActive(false); // Only host can start
        }
        
        ShowPanel(lobbyRoomPanel);
    }

    // Add this method to your LobbyUIManager class
    public void UpdateUIForAutoConnect() {
        // Update UI to show we're auto-connecting as client
        lobbyNameText.text = "Auto-Connected Client";
        UpdatePlayerCount(1, 8);
        startGameButton.gameObject.SetActive(false); // Client can't start
        
        ShowPanel(lobbyRoomPanel);
    }
        
    
    private void UpdatePlayerCount(int current, int max)
    {
        playerCountText.text = $"Players: {current}/{max}";
    }
    
    private void RefreshLobbyList()
{
    // Force load from disk to share lobbies between instances
    LobbySync.LoadLobbies();
    
    // Clear existing entries
    foreach (var entry in lobbyEntries)
    {
        Destroy(entry);
    }
    lobbyEntries.Clear();
    
    // Add all active lobbies
    foreach (var lobbyInfo in LobbyManager.ActiveLobbies.Values)
    {
        GameObject entryObj = Instantiate(lobbyEntryPrefab, lobbyListContent);
        lobbyEntries.Add(entryObj);
        
        // Set up the entry UI
        TextMeshProUGUI lobbyNameText = entryObj.transform.Find("LobbyNameText").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI playerCountText = entryObj.transform.Find("PlayerCountText").GetComponent<TextMeshProUGUI>();
        Button joinButton = entryObj.transform.Find("JoinButton").GetComponent<Button>();
        
        lobbyNameText.text = lobbyInfo.lobbyName;
        playerCountText.text = $"{lobbyInfo.currentPlayers}/{lobbyInfo.maxPlayers}";
        
        string lobbyId = lobbyInfo.lobbyId; // Capture for closure
        joinButton.onClick.AddListener(() => JoinLobby(lobbyId));
    }
}
}