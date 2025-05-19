using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class LobbySync
{
    // Use a consistent absolute path in the user's home directory
    private static readonly string LobbyFilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
        "UnityMultiplayerLobbies.json");
    
    [Serializable]
    private class LobbyListData
    {
        public List<LobbyInfo> Lobbies = new List<LobbyInfo>();
    }
    
    public static void SaveLobbies()
    {
        try
        {
            Debug.Log($"[LobbySync] Attempting to save lobbies to absolute path: {LobbyFilePath}");
            
            LobbyListData data = new LobbyListData();
            data.Lobbies = new List<LobbyInfo>(LobbyManager.ActiveLobbies.Values);
            
            Debug.Log($"[LobbySync] Preparing to save {data.Lobbies.Count} lobbies");
            foreach (var lobby in data.Lobbies)
            {
                Debug.Log($"[LobbySync] - Lobby: ID={lobby.lobbyId}, Name={lobby.lobbyName}, Players={lobby.currentPlayers}/{lobby.maxPlayers}");
            }
            
            string json = JsonUtility.ToJson(data);
            Debug.Log($"[LobbySync] JSON content to save ({json.Length} chars): {json}");
            
            // Save to the absolute path
            File.WriteAllText(LobbyFilePath, json);
            Debug.Log($"[LobbySync] Successfully saved {LobbyManager.ActiveLobbies.Count} lobbies to absolute path: {LobbyFilePath}");
            
            // Verify the file was written
            if (File.Exists(LobbyFilePath))
            {
                FileInfo fileInfo = new FileInfo(LobbyFilePath);
                Debug.Log($"[LobbySync] File saved: {fileInfo.FullName}, Size: {fileInfo.Length} bytes, Last Write: {fileInfo.LastWriteTime}");
            }
            else
            {
                Debug.LogError("[LobbySync] File doesn't exist after save attempt!");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[LobbySync] Error saving lobbies: {e.GetType().Name}: {e.Message}\nStack Trace: {e.StackTrace}");
        }
    }
    
    public static void LoadLobbies()
    {
        try
        {
            Debug.Log($"[LobbySync] Attempting to load lobbies from absolute path: {LobbyFilePath}");
            
            if (File.Exists(LobbyFilePath))
            {
                FileInfo fileInfo = new FileInfo(LobbyFilePath);
                Debug.Log($"[LobbySync] Found file: {fileInfo.FullName}, Size: {fileInfo.Length} bytes, Last Write: {fileInfo.LastWriteTime}");
                
                string json = File.ReadAllText(LobbyFilePath);
                Debug.Log($"[LobbySync] Read JSON content ({json.Length} chars): {json}");
                
                LobbyListData data = JsonUtility.FromJson<LobbyListData>(json);
                
                if (data != null && data.Lobbies != null)
                {
                    int previousCount = LobbyManager.ActiveLobbies.Count;
                    LobbyManager.ActiveLobbies.Clear();
                    
                    foreach (var lobby in data.Lobbies)
                    {
                        LobbyManager.ActiveLobbies[lobby.lobbyId] = lobby;
                        Debug.Log($"[LobbySync] - Loaded Lobby: ID={lobby.lobbyId}, Name={lobby.lobbyName}, Players={lobby.currentPlayers}/{lobby.maxPlayers}");
                    }
                    
                    Debug.Log($"[LobbySync] Successfully loaded {LobbyManager.ActiveLobbies.Count} lobbies (previous count: {previousCount})");
                }
                else
                {
                    Debug.LogWarning("[LobbySync] Parsed data was null or had null Lobbies list");
                }
            }
            else
            {
                Debug.Log($"[LobbySync] No lobby file exists at {LobbyFilePath} - starting with empty lobbies list");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[LobbySync] Error loading lobbies: {e.GetType().Name}: {e.Message}\nStack Trace: {e.StackTrace}");
        }
        
        // Log the current lobby state regardless of load success
        Debug.Log($"[LobbySync] Current lobby count: {LobbyManager.ActiveLobbies.Count}");
        foreach (var kvp in LobbyManager.ActiveLobbies)
        {
            Debug.Log($"[LobbySync] Active Lobby: ID={kvp.Key}, Name={kvp.Value.lobbyName}, Players={kvp.Value.currentPlayers}/{kvp.Value.maxPlayers}");
        }
    }
    
    // Add this helper method to check file accessibility
    public static void VerifyFileAccess()
    {
        Debug.Log($"[LobbySync] Verifying file access for absolute path: {LobbyFilePath}");
        try
        {
            // Test write access
            File.WriteAllText(LobbyFilePath, "test");
            Debug.Log("[LobbySync] Successfully wrote test file");
            
            // Test read access
            string content = File.ReadAllText(LobbyFilePath);
            Debug.Log($"[LobbySync] Successfully read test file. Content: {content}");
            
            // Clean up
            File.Delete(LobbyFilePath);
            Debug.Log("[LobbySync] Successfully deleted test file");
        }
        catch (Exception e)
        {
            Debug.LogError($"[LobbySync] File access verification failed: {e.GetType().Name}: {e.Message}");
        }
    }
}