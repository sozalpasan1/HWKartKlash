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
    Debug.Log("[LobbyUIManager] Start method called");
    VerifyScrollViewHierarchy();
    // Verify the required UI components are assigned
    if (mainMenuPanel == null) Debug.LogError("[LobbyUIManager] mainMenuPanel is not assigned!");
    if (createLobbyPanel == null) Debug.LogError("[LobbyUIManager] createLobbyPanel is not assigned!");
    if (lobbyListPanel == null) Debug.LogError("[LobbyUIManager] lobbyListPanel is not assigned!");
    if (lobbyRoomPanel == null) Debug.LogError("[LobbyUIManager] lobbyRoomPanel is not assigned!");
    
    if (createLobbyButton == null) Debug.LogError("[LobbyUIManager] createLobbyButton is not assigned!");
    if (joinLobbyButton == null) Debug.LogError("[LobbyUIManager] joinLobbyButton is not assigned!");
    
    if (lobbyNameInput == null) Debug.LogError("[LobbyUIManager] lobbyNameInput is not assigned!");
    if (createButton == null) Debug.LogError("[LobbyUIManager] createButton is not assigned!");
    if (backFromCreateButton == null) Debug.LogError("[LobbyUIManager] backFromCreateButton is not assigned!");
    
    if (lobbyListContent == null) Debug.LogError("[LobbyUIManager] lobbyListContent is not assigned!");
    if (lobbyEntryPrefab == null) Debug.LogError("[LobbyUIManager] lobbyEntryPrefab is not assigned!");
    if (refreshButton == null) Debug.LogError("[LobbyUIManager] refreshButton is not assigned!");
    if (backFromListButton == null) Debug.LogError("[LobbyUIManager] backFromListButton is not assigned!");
    
    if (lobbyNameText == null) Debug.LogError("[LobbyUIManager] lobbyNameText is not assigned!");
    if (playerCountText == null) Debug.LogError("[LobbyUIManager] playerCountText is not assigned!");
    if (startGameButton == null) Debug.LogError("[LobbyUIManager] startGameButton is not assigned!");
    if (leaveLobbyButton == null) Debug.LogError("[LobbyUIManager] leaveLobbyButton is not assigned!");
    
    // Check if NetworkLobbyManager is available
    if (NetworkLobbyManager.Instance == null)
    {
        Debug.LogError("[LobbyUIManager] NetworkLobbyManager.Instance is null! Make sure NetworkLobbyManager is in the scene.");
    }

    // Set up button listeners
    createLobbyButton.onClick.AddListener(() => {
        Debug.Log("[LobbyUIManager] Create Lobby button clicked");
        ShowPanel(createLobbyPanel);
    });
    
    joinLobbyButton.onClick.AddListener(() => {
        Debug.Log("[LobbyUIManager] Join Lobby button clicked");
        ShowPanel(lobbyListPanel);
    });
    
    backFromCreateButton.onClick.AddListener(() => {
        Debug.Log("[LobbyUIManager] Back from Create button clicked");
        ShowPanel(mainMenuPanel);
    });
    
    backFromListButton.onClick.AddListener(() => {
        Debug.Log("[LobbyUIManager] Back from List button clicked");
        ShowPanel(mainMenuPanel);
    });
    
    createButton.onClick.AddListener(() => {
        Debug.Log("[LobbyUIManager] Create button clicked");
        CreateLobby();
    });
    
    refreshButton.onClick.AddListener(() => {
        Debug.Log("[LobbyUIManager] Refresh button clicked");
        RefreshLobbyList();
    });
    
    startGameButton.onClick.AddListener(() => {
        Debug.Log("[LobbyUIManager] Start Game button clicked");
        NetworkLobbyManager.Instance.StartGame();
    });
    
    leaveLobbyButton.onClick.AddListener(() => {
        Debug.Log("[LobbyUIManager] Leave Lobby button clicked");
        NetworkLobbyManager.Instance.LeaveLobby();
        ShowPanel(mainMenuPanel);
    });
    
    // Initially show main menu
    Debug.Log("[LobbyUIManager] Showing main menu panel");
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
    Debug.Log("[LobbyUIManager] RefreshLobbyList called");
    
    // Force load from disk to share lobbies between instances
    Debug.Log("[LobbyUIManager] Calling LobbySync.LoadLobbies()");
    LobbySync.LoadLobbies();
    
    // Verify the ScrollView hierarchy
    VerifyScrollViewHierarchy();
    
    // Clear existing entries
    Debug.Log($"[LobbyUIManager] Clearing {lobbyEntries.Count} existing entries");
    foreach (var entry in lobbyEntries)
    {
        Destroy(entry);
    }
    lobbyEntries.Clear();
    
    Debug.Log($"[LobbyUIManager] Adding {LobbyManager.ActiveLobbies.Count} lobbies to UI list");
    
    // First, find the correct Content transform
    Transform contentTransform = FindContentTransform();
    if (contentTransform == null)
    {
        Debug.LogError("[LobbyUIManager] Could not find the Content transform for ScrollView!");
        return;
    }
    
    // Add all active lobbies
    foreach (var lobbyInfo in LobbyManager.ActiveLobbies.Values)
    {
        try 
        {
            Debug.Log($"[LobbyUIManager] Creating UI entry for lobby: {lobbyInfo.lobbyName} (ID: {lobbyInfo.lobbyId})");
            
            // Instantiate as a child of the CONTENT transform, not the ScrollView directly
            GameObject entryObj = Instantiate(lobbyEntryPrefab, contentTransform);
            lobbyEntries.Add(entryObj);
            
            // Configure the entry's RectTransform
            RectTransform entryRect = entryObj.GetComponent<RectTransform>();
            if (entryRect != null)
            {
                // Make sure it takes full width of the content area
                entryRect.anchorMin = new Vector2(0, 0);
                entryRect.anchorMax = new Vector2(1, 0);
                entryRect.pivot = new Vector2(0.5f, 0);
                entryRect.anchoredPosition = new Vector2(0, 0);
                entryRect.sizeDelta = new Vector2(0, 100); // Fixed height of 100 pixels
                
                Debug.Log($"[LobbyUIManager] Entry '{lobbyInfo.lobbyName}' RectTransform configured: Width={entryRect.rect.width}, Height={entryRect.rect.height}");
            }
            
            // Set up the entry UI components
            var lobbyNameText = entryObj.transform.Find("LobbyNameText")?.GetComponent<TextMeshProUGUI>();
            var playerCountText = entryObj.transform.Find("PlayerCountText")?.GetComponent<TextMeshProUGUI>();
            var joinButton = entryObj.transform.Find("JoinButton")?.GetComponent<Button>();
            
            if (lobbyNameText != null) lobbyNameText.text = lobbyInfo.lobbyName;
            if (playerCountText != null) playerCountText.text = $"{lobbyInfo.currentPlayers}/{lobbyInfo.maxPlayers}";
            
            string lobbyId = lobbyInfo.lobbyId; // Capture for closure
            if (joinButton != null)
            {
                joinButton.onClick.AddListener(() => {
                    Debug.Log($"[LobbyUIManager] Join button clicked for lobby: {lobbyId}");
                    JoinLobby(lobbyId);
                });
            }
            
            Debug.Log($"[LobbyUIManager] Successfully created UI entry for lobby: {lobbyInfo.lobbyName}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[LobbyUIManager] Error creating lobby entry: {e.Message}\n{e.StackTrace}");
        }
    }
    
    // Update the content size to fit all entries
    UpdateContentSize(contentTransform);
    
    Debug.Log($"[LobbyUIManager] RefreshLobbyList complete - added {lobbyEntries.Count} entries to UI");
}

// Helper method to update the Content transform's size
private void UpdateContentSize(Transform contentTransform)
{
    if (contentTransform == null) return;
    
    // Get the RectTransform of the content
    RectTransform contentRect = contentTransform.GetComponent<RectTransform>();
    if (contentRect == null) return;
    
    // Get vertical layout group if available
    VerticalLayoutGroup layoutGroup = contentTransform.GetComponent<VerticalLayoutGroup>();
    if (layoutGroup != null)
    {
        // Force rebuild layout
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentRect);
    }
    else
    {
        // Manually calculate height if no layout group
        float totalHeight = 0;
        foreach (var entry in lobbyEntries)
        {
            RectTransform entryRect = entry.GetComponent<RectTransform>();
            if (entryRect != null)
            {
                totalHeight += entryRect.rect.height;
            }
        }
        
        // Add some padding
        totalHeight += 20 * lobbyEntries.Count;
        
        // Set content height
        contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, totalHeight);
    }
    
    Debug.Log($"[LobbyUIManager] Content size updated: {contentRect.sizeDelta}");
}

// Helper method to find the Content transform in the ScrollView hierarchy
private Transform FindContentTransform()
{
    // If lobbyListContent is directly assigned, use it
    if (lobbyListContent != null)
    {
        Debug.Log($"[LobbyUIManager] Using directly assigned lobbyListContent: {lobbyListContent.name}");
        return lobbyListContent;
    }
    
    // Otherwise, try to find Content in the ScrollView hierarchy
    if (lobbyListPanel == null)
    {
        Debug.LogError("[LobbyUIManager] lobbyListPanel is null!");
        return null;
    }
    
    // Find ScrollView in the lobby list panel
    ScrollRect scrollRect = lobbyListPanel.GetComponentInChildren<ScrollRect>();
    if (scrollRect == null)
    {
        Debug.LogError("[LobbyUIManager] No ScrollRect found in lobbyListPanel!");
        return null;
    }
    
    // Get the content from the ScrollRect
    if (scrollRect.content != null)
    {
        Debug.Log($"[LobbyUIManager] Found Content via ScrollRect: {scrollRect.content.name}");
        return scrollRect.content;
    }
    
    // If ScrollRect.content is not set, try to find it by name
    Transform viewport = scrollRect.transform.Find("Viewport");
    if (viewport != null)
    {
        Transform content = viewport.Find("Content");
        if (content != null)
        {
            Debug.Log($"[LobbyUIManager] Found Content by name in hierarchy: {content.name}");
            return content;
        }
    }
    
    Debug.LogError("[LobbyUIManager] Could not find Content transform in ScrollView hierarchy!");
    return null;
}

// Helper method to verify and fix ScrollView hierarchy
private void VerifyScrollViewHierarchy()
{
    Debug.Log("[LobbyUIManager] Verifying ScrollView hierarchy");
    
    if (lobbyListPanel == null)
    {
        Debug.LogError("[LobbyUIManager] lobbyListPanel is null!");
        return;
    }
    
    // Find ScrollView in the lobby list panel
    ScrollRect scrollRect = lobbyListPanel.GetComponentInChildren<ScrollRect>();
    if (scrollRect == null)
    {
        Debug.LogError("[LobbyUIManager] No ScrollRect found in lobbyListPanel!");
        return;
    }
    
    Debug.Log($"[LobbyUIManager] Found ScrollRect: {scrollRect.name}");
    
    // Check Viewport
    RectTransform viewport = scrollRect.viewport;
    if (viewport == null)
    {
        // Try to find/create viewport
        Transform viewportTransform = scrollRect.transform.Find("Viewport");
        if (viewportTransform == null)
        {
            Debug.LogError("[LobbyUIManager] No Viewport found in ScrollRect!");
            return;
        }
        viewport = viewportTransform as RectTransform;
        scrollRect.viewport = viewport;
    }
    
    Debug.Log($"[LobbyUIManager] Found Viewport: {viewport.name}");
    
    // Check if viewport has mask
    if (viewport.GetComponent<Mask>() == null && viewport.GetComponent<RectMask2D>() == null)
    {
        Debug.LogWarning("[LobbyUIManager] Viewport has no mask component! Adding RectMask2D.");
        viewport.gameObject.AddComponent<RectMask2D>();
    }
    
    // Check Content
    RectTransform content = scrollRect.content;
    Transform contentTransform = viewport.Find("Content");
    
    if (content == null && contentTransform == null)
    {
        Debug.LogError("[LobbyUIManager] No Content found in Viewport!");
        return;
    }
    else if (content == null && contentTransform != null)
    {
        content = contentTransform as RectTransform;
        scrollRect.content = content;
    }
    
    Debug.Log($"[LobbyUIManager] Found Content: {content.name}");
    
    // Check if Content has VerticalLayoutGroup
    VerticalLayoutGroup layoutGroup = content.GetComponent<VerticalLayoutGroup>();
    if (layoutGroup == null)
    {
        Debug.Log("[LobbyUIManager] Adding VerticalLayoutGroup to Content");
        layoutGroup = content.gameObject.AddComponent<VerticalLayoutGroup>();
        layoutGroup.childAlignment = TextAnchor.UpperCenter;
        layoutGroup.childControlHeight = true;
        layoutGroup.childControlWidth = true;
        layoutGroup.childForceExpandHeight = false;
        layoutGroup.childForceExpandWidth = true;
        layoutGroup.spacing = 10;
        layoutGroup.padding = new RectOffset(10, 10, 10, 10);
    }
    
    // Check if Content has ContentSizeFitter
    ContentSizeFitter sizeFitter = content.GetComponent<ContentSizeFitter>();
    if (sizeFitter == null)
    {
        Debug.Log("[LobbyUIManager] Adding ContentSizeFitter to Content");
        sizeFitter = content.gameObject.AddComponent<ContentSizeFitter>();
        sizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    }
    
    // Update lobbyListContent reference if needed
    if (lobbyListContent == null || lobbyListContent != content)
    {
        Debug.Log("[LobbyUIManager] Updating lobbyListContent reference");
        lobbyListContent = content;
    }
    
    Debug.Log("[LobbyUIManager] ScrollView hierarchy verification complete");
}
}