using System.Collections.Generic;
using Unity.Netcode;

public struct CreateLobbyMessage : INetworkSerializable
{
    public string LobbyId;
    public string LobbyName;
    
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref LobbyId);
        serializer.SerializeValue(ref LobbyName);
    }
}

public struct JoinLobbyMessage : INetworkSerializable
{
    public string PlayerId;
    public string LobbyId;
    
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref PlayerId);
        serializer.SerializeValue(ref LobbyId);
    }
}

public struct LobbyListMessage : INetworkSerializable
{
    public List<LobbyInfo> Lobbies;
    
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        // For simplicity, we'll serialize the count and then each lobby separately
        if (serializer.IsReader)
        {
            int count = 0;
            serializer.SerializeValue(ref count);
            Lobbies = new List<LobbyInfo>(count);
            
            for (int i = 0; i < count; i++)
            {
                string id = string.Empty;
                string name = string.Empty;
                int currentPlayers = 0;
                int maxPlayers = 0;
                string hostId = string.Empty;
                
                serializer.SerializeValue(ref id);
                serializer.SerializeValue(ref name);
                serializer.SerializeValue(ref currentPlayers);
                serializer.SerializeValue(ref maxPlayers);
                serializer.SerializeValue(ref hostId);
                
                Lobbies.Add(new LobbyInfo(id, name, hostId, maxPlayers)
                {
                    currentPlayers = currentPlayers
                });
            }
        }
        else
        {
            int count = Lobbies.Count;
            serializer.SerializeValue(ref count);
            
            foreach (var lobby in Lobbies)
            {
                string id = lobby.lobbyId;
                string name = lobby.lobbyName;
                int currentPlayers = lobby.currentPlayers;
                int maxPlayers = lobby.maxPlayers;
                string hostId = lobby.hostId;
                
                serializer.SerializeValue(ref id);
                serializer.SerializeValue(ref name);
                serializer.SerializeValue(ref currentPlayers);
                serializer.SerializeValue(ref maxPlayers);
                serializer.SerializeValue(ref hostId);
            }
        }
    }
}