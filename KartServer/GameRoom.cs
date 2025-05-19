using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace KartServer
{
    public class GameRoom
    {
        public string RoomCode { get; private set; }
        private Dictionary<string, KartPlayer> players = new Dictionary<string, KartPlayer>();
        private bool isRunning = true;
        private Thread physicsThread;
        private const int PHYSICS_RATE = 30; // Updates per second
        private readonly object playersLock = new object();
        
        public GameRoom(string roomCode)
        {
            RoomCode = roomCode;
            
            // Start physics update thread
            physicsThread = new Thread(new ThreadStart(PhysicsLoop));
            physicsThread.Start();
        }
        
        public void AddPlayer(KartPlayer player)
        {
            lock (playersLock)
            {
                // Give player a random starting position
                Random rand = new Random();
                player.PositionX = (float)(rand.NextDouble() * 10 - 5);
                player.PositionZ = (float)(rand.NextDouble() * 10 - 5);
                player.RotationY = (float)(rand.NextDouble() * 360);
                
                players.Add(player.Id, player);
                
                // Notify all players about the new player
                BroadcastPlayerJoined(player);
            }
        }
        
        public bool RemovePlayer(string playerId)
        {
            lock (playersLock)
            {
                if (players.ContainsKey(playerId))
                {
                    // Get player before removing (for notification)
                    KartPlayer player = players[playerId];
                    
                    // Remove player
                    players.Remove(playerId);
                    
                    // Notify other players
                    BroadcastPlayerLeft(player);
                    
                    return true;
                }
                return false;
            }
        }
        
        public void CloseRoom()
        {
            isRunning = false;
            
            lock (playersLock)
            {
                // Close all client connections
                foreach (var player in players.Values)
                {
                    try
                    {
                        player.TcpClient.Close();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Error closing client: {e.Message}");
                    }
                }
                
                players.Clear();
            }
            
            physicsThread?.Join(1000);
        }
        
        public int GetPlayerCount()
        {
            lock (playersLock)
            {
                return players.Count;
            }
        }
        
        public void UpdatePlayerState(KartPlayer player)
        {
            // No need to do anything here, physics loop will update all players
        }
        
        public void BroadcastState()
        {
            lock (playersLock)
            {
                if (players.Count == 0) return;
                
                // Prepare state message with all players
                StringBuilder stateBuilder = new StringBuilder("FULLSTATE:");
                
                foreach (var player in players.Values)
                {
                    stateBuilder.Append($"{player.Id}:{player.PositionX}:{player.PositionY}:{player.PositionZ}:{player.RotationY}:{player.Speed}|");
                }
                
                // Remove last pipe
                if (stateBuilder.Length > 0 && stateBuilder[stateBuilder.Length - 1] == '|')
                {
                    stateBuilder.Length--;
                }
                
                string stateMessage = stateBuilder.ToString();
                byte[] stateBytes = Encoding.UTF8.GetBytes(stateMessage);
                
                // Send to all players
                foreach (var player in players.Values)
                {
                    try
                    {
                        NetworkStream stream = player.TcpClient.GetStream();
                        stream.Write(stateBytes, 0, stateBytes.Length);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Error sending state to player {player.Id}: {e.Message}");
                    }
                }
            }
        }
        
        private void BroadcastPlayerJoined(KartPlayer newPlayer)
        {
            string joinMessage = $"JOIN:{newPlayer.Id}:{newPlayer.PositionX}:{newPlayer.PositionY}:{newPlayer.PositionZ}:{newPlayer.RotationY}";
            byte[] joinBytes = Encoding.UTF8.GetBytes(joinMessage);
            
            foreach (var player in players.Values)
            {
                if (player.Id != newPlayer.Id) // Don't send to the new player
                {
                    try
                    {
                        NetworkStream stream = player.TcpClient.GetStream();
                        stream.Write(joinBytes, 0, joinBytes.Length);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Error sending join notification: {e.Message}");
                    }
                }
            }
        }
        
        private void BroadcastPlayerLeft(KartPlayer leftPlayer)
        {
            string leftMessage = $"LEAVE:{leftPlayer.Id}";
            byte[] leftBytes = Encoding.UTF8.GetBytes(leftMessage);
            
            foreach (var player in players.Values)
            {
                try
                {
                    NetworkStream stream = player.TcpClient.GetStream();
                    stream.Write(leftBytes, 0, leftBytes.Length);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error sending leave notification: {e.Message}");
                }
            }
        }
        
        private void PhysicsLoop()
        {
            float deltaTime = 1.0f / PHYSICS_RATE;
            long lastUpdateTime = DateTime.Now.Ticks;
            
            while (isRunning)
            {
                // Calculate actual delta time
                long currentTime = DateTime.Now.Ticks;
                float actualDelta = (currentTime - lastUpdateTime) / 10000000f; // Convert to seconds
                lastUpdateTime = currentTime;
                
                // Update all players
                lock (playersLock)
                {
                    foreach (var player in players.Values)
                    {
                        player.UpdatePhysics(actualDelta);
                    }
                }
                
                // Broadcast state to all clients
                BroadcastState();
                
                // Sleep to maintain update rate
                Thread.Sleep(1000 / PHYSICS_RATE);
            }
        }
    }
}