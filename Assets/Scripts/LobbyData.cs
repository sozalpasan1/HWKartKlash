using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[Serializable]
public class LobbyInfo
{
    public string lobbyId;
    public string lobbyName;
    public int currentPlayers;
    public int maxPlayers;
    public string hostId;

    public LobbyInfo(string id, string name, string host, int maxPlayers = 8)
    {
        lobbyId = id;
        lobbyName = name;
        hostId = host;
        currentPlayers = 1; // Host is the first player
        this.maxPlayers = maxPlayers;
    }
}

public static class LobbyManager
{
    public static Dictionary<string, LobbyInfo> ActiveLobbies = new Dictionary<string, LobbyInfo>();
    
    static LobbyManager()
    {
        Debug.Log("[LobbyManager] Static constructor called - initializing manager");
    }
    
    public static string CreateLobby(string lobbyName, string hostId)
    {
        Debug.Log($"[LobbyManager] CreateLobby called for '{lobbyName}' by host '{hostId}'");
        
        // Load first to get any lobbies from other instances
        Debug.Log("[LobbyManager] Loading lobbies from disk before creation");
        LobbySync.LoadLobbies();
        
        string lobbyId = Guid.NewGuid().ToString();
        Debug.Log($"[LobbyManager] Generated new lobby ID: {lobbyId}");
        
        LobbyInfo newLobby = new LobbyInfo(lobbyId, lobbyName, hostId);
        Debug.Log($"[LobbyManager] Created new lobby object: {lobbyName}, ID: {lobbyId}");
        
        int existingCount = ActiveLobbies.Count;
        ActiveLobbies.Add(lobbyId, newLobby);
        Debug.Log($"[LobbyManager] Added lobby to dictionary. Previous count: {existingCount}, New count: {ActiveLobbies.Count}");
        
        // Save to share with other instances
        Debug.Log("[LobbyManager] Saving lobbies to disk after creation");
        LobbySync.SaveLobbies();
        
        return lobbyId;
    }
    
    public static bool JoinLobby(string lobbyId, string playerId)
    {
        Debug.Log($"[LobbyManager] JoinLobby called for lobby '{lobbyId}' by player '{playerId}'");
        
        // Load first to get updated lobby state
        Debug.Log("[LobbyManager] Loading lobbies from disk before join");
        LobbySync.LoadLobbies();
        
        if (ActiveLobbies.TryGetValue(lobbyId, out LobbyInfo lobby))
        {
            Debug.Log($"[LobbyManager] Found lobby '{lobbyId}', current players: {lobby.currentPlayers}/{lobby.maxPlayers}");
            
            if (lobby.currentPlayers < lobby.maxPlayers)
            {
                lobby.currentPlayers++;
                Debug.Log($"[LobbyManager] Increased player count to {lobby.currentPlayers}/{lobby.maxPlayers}");
                
                Debug.Log("[LobbyManager] Saving lobbies to disk after join");
                LobbySync.SaveLobbies();
                return true;
            }
            else
            {
                Debug.LogWarning($"[LobbyManager] Cannot join - lobby is full: {lobby.currentPlayers}/{lobby.maxPlayers}");
                return false;
            }
        }
        else
        {
            Debug.LogError($"[LobbyManager] Lobby not found: '{lobbyId}'. Available lobbies: {string.Join(", ", ActiveLobbies.Keys)}");
            return false;
        }
    }
    
    public static void LeaveLobby(string lobbyId, string playerId, bool isHost)
    {
        Debug.Log($"[LobbyManager] LeaveLobby called for lobby '{lobbyId}' by player '{playerId}', isHost: {isHost}");
        
        // Load first to get updated lobby state
        Debug.Log("[LobbyManager] Loading lobbies from disk before leave");
        LobbySync.LoadLobbies();
        
        if (ActiveLobbies.TryGetValue(lobbyId, out LobbyInfo lobby))
        {
            Debug.Log($"[LobbyManager] Found lobby '{lobbyId}', current players: {lobby.currentPlayers}/{lobby.maxPlayers}");
            
            lobby.currentPlayers--;
            Debug.Log($"[LobbyManager] Decreased player count to {lobby.currentPlayers}/{lobby.maxPlayers}");
            
            // If the host leaves, remove the lobby
            if (isHost || lobby.currentPlayers <= 0)
            {
                Debug.Log($"[LobbyManager] Removing lobby '{lobbyId}' because host left or no players remain");
                ActiveLobbies.Remove(lobbyId);
            }
            
            Debug.Log("[LobbyManager] Saving lobbies to disk after leave");
            LobbySync.SaveLobbies();
        }
        else
        {
            Debug.LogError($"[LobbyManager] Cannot leave - lobby not found: '{lobbyId}'. Available lobbies: {string.Join(", ", ActiveLobbies.Keys)}");
        }
    }
    
    // Add a debug helper
    public static void LogLobbyState()
    {
        Debug.Log($"[LobbyManager] Current Lobby State - {ActiveLobbies.Count} lobbies:");
        foreach (var kvp in ActiveLobbies)
        {
            Debug.Log($"[LobbyManager] - Lobby: ID={kvp.Key}, Name={kvp.Value.lobbyName}, " +
                     $"Players={kvp.Value.currentPlayers}/{kvp.Value.maxPlayers}, Host={kvp.Value.hostId}");
        }
    }
}