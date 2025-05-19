using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class LobbySync
{
    private static readonly string LobbyFilePath = Path.Combine(Application.dataPath, "../SharedLobbies.json");
    
    [Serializable]
    private class LobbyListData
    {
        public List<LobbyInfo> Lobbies = new List<LobbyInfo>();
    }
    
    public static void SaveLobbies()
    {
        try
        {
            LobbyListData data = new LobbyListData();
            data.Lobbies = new List<LobbyInfo>(LobbyManager.ActiveLobbies.Values);
            
            string json = JsonUtility.ToJson(data);
            File.WriteAllText(LobbyFilePath, json);
            Debug.Log($"Saved {LobbyManager.ActiveLobbies.Count} lobbies to disk");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error saving lobbies: {e.Message}");
        }
    }
    
    public static void LoadLobbies()
    {
        try
        {
            if (File.Exists(LobbyFilePath))
            {
                string json = File.ReadAllText(LobbyFilePath);
                LobbyListData data = JsonUtility.FromJson<LobbyListData>(json);
                
                LobbyManager.ActiveLobbies.Clear();
                foreach (var lobby in data.Lobbies)
                {
                    LobbyManager.ActiveLobbies[lobby.lobbyId] = lobby;
                }
                
                Debug.Log($"Loaded {LobbyManager.ActiveLobbies.Count} lobbies from disk");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error loading lobbies: {e.Message}");
        }
    }
}