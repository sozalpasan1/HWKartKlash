using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject createRoomPanel;
    [SerializeField] private GameObject joinRoomPanel;
    [SerializeField] private GameObject waitingRoomPanel;
    
    [Header("Create Room")]
    [SerializeField] private Button createRoomButton;
    
    [Header("Join Room")]
    [SerializeField] private Button joinRoomButton;
    [SerializeField] private TMP_InputField roomCodeInput;
    
    [Header("Waiting Room")]
    [SerializeField] private TextMeshProUGUI roomCodeText;
    [SerializeField] private TextMeshProUGUI playerCountText;
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button leaveRoomButton;
    
    [Header("Game Scene")]
    [SerializeField] private string gameSceneName = "GameScene";
    
    private string currentRoomCode = "";
    private List<string> connectedPlayers = new List<string>();
    
    private void Awake()
    {
        // Show the main panel initially
        ShowPanel(mainPanel);
    }
    
    private void Start()
    {
        // Set up button listeners
        createRoomButton.onClick.AddListener(OnCreateRoomClicked);
        joinRoomButton.onClick.AddListener(OnJoinRoomClicked);
        startGameButton.onClick.AddListener(OnStartGameClicked);
        leaveRoomButton.onClick.AddListener(OnLeaveRoomClicked);
        
        // Set up network event listeners
        NetworkManager.Instance.OnRoomJoined += OnRoomJoined;
        NetworkManager.Instance.OnConnectionError += OnConnectionError;
        NetworkManager.Instance.OnPlayerJoined += OnPlayerJoined;
        NetworkManager.Instance.OnPlayerLeft += OnPlayerLeft;
    }
    
    private void OnDestroy()
    {
        // Clean up event listeners
        if (NetworkManager.Instance != null)
        {
            NetworkManager.Instance.OnRoomJoined -= OnRoomJoined;
            NetworkManager.Instance.OnConnectionError -= OnConnectionError;
            NetworkManager.Instance.OnPlayerJoined -= OnPlayerJoined;
            NetworkManager.Instance.OnPlayerLeft -= OnPlayerLeft;
        }
    }
    
    private void OnCreateRoomClicked()
    {
        // Create a new room
        NetworkManager.Instance.CreateRoom();
        
        // Show waiting panel
        ShowPanel(waitingRoomPanel);
    }
    
    private void OnJoinRoomClicked()
    {
        // Get room code from input field
        string roomCode = roomCodeInput.text.Trim().ToUpper();
        
        if (string.IsNullOrEmpty(roomCode))
        {
            Debug.LogWarning("Room code cannot be empty");
            return;
        }
        
        // Join the room
        NetworkManager.Instance.JoinRoom(roomCode);
        
        // Show waiting panel
        ShowPanel(waitingRoomPanel);
    }
    
    private void OnStartGameClicked()
    {
        // Load the game scene
        SceneManager.LoadScene(gameSceneName);
    }
    
    private void OnLeaveRoomClicked()
    {
        // Leave the room
        NetworkManager.Instance.CloseConnection();
        
        // Reset UI
        currentRoomCode = "";
        connectedPlayers.Clear();
        
        // Show main panel
        ShowPanel(mainPanel);
    }
    
    private void OnRoomJoined(string roomCode)
    {
        // Room joined successfully
        currentRoomCode = roomCode;
        
        // Update UI
        roomCodeText.text = "Room Code: " + roomCode;
        UpdatePlayerCountText();
        
        // Show waiting room panel
        ShowPanel(waitingRoomPanel);
    }
    
    private void OnConnectionError(string errorMessage)
    {
        Debug.LogError("Connection error: " + errorMessage);
        
        // Show main panel
        ShowPanel(mainPanel);
    }
    
    private void OnPlayerJoined(string playerId)
    {
        // Add player to list
        if (!connectedPlayers.Contains(playerId))
        {
            connectedPlayers.Add(playerId);
        }
        
        // Update UI
        UpdatePlayerCountText();
    }
    
    private void OnPlayerLeft(string playerId)
    {
        // Remove player from list
        connectedPlayers.Remove(playerId);
        
        // Update UI
        UpdatePlayerCountText();
    }
    
    private void UpdatePlayerCountText()
    {
        // +1 for local player
        playerCountText.text = "Players: " + (connectedPlayers.Count + 1);
    }
    
    private void ShowPanel(GameObject panel)
    {
        // Hide all panels
        mainPanel.SetActive(false);
        createRoomPanel.SetActive(false);
        joinRoomPanel.SetActive(false);
        waitingRoomPanel.SetActive(false);
        
        // Show the requested panel
        panel.SetActive(true);
    }
}