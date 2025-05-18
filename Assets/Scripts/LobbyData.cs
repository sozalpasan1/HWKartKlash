using System;
using System.Collections.Generic;
using Unity.Netcode;

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
    
    public static string CreateLobby(string lobbyName, string hostId)
    {
        string lobbyId = Guid.NewGuid().ToString();
        LobbyInfo newLobby = new LobbyInfo(lobbyId, lobbyName, hostId);
        ActiveLobbies.Add(lobbyId, newLobby);
        return lobbyId;
    }
    
    public static bool JoinLobby(string lobbyId, string playerId)
    {
        if (ActiveLobbies.TryGetValue(lobbyId, out LobbyInfo lobby))
        {
            if (lobby.currentPlayers < lobby.maxPlayers)
            {
                lobby.currentPlayers++;
                return true;
            }
        }
        return false;
    }
    
    public static void LeaveLobby(string lobbyId, string playerId, bool isHost)
    {
        if (ActiveLobbies.TryGetValue(lobbyId, out LobbyInfo lobby))
        {
            lobby.currentPlayers--;
            
            // If the host leaves, remove the lobby
            if (isHost || lobby.currentPlayers <= 0)
            {
                ActiveLobbies.Remove(lobbyId);
            }
        }
    }
}