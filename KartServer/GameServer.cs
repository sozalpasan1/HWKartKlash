using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace KartServer
{
    public class GameServer
    {
        private TcpListener tcpListener;
        private bool isRunning = false;
        private int port;
        private Thread serverThread;
        private Dictionary<string, GameRoom> gameRooms = new Dictionary<string, GameRoom>();
        private int maxPlayersPerRoom = 4;

        public GameServer(int port)
        {
            this.port = port;
        }

        public void Start()
        {
            if (!isRunning)
            {
                isRunning = true;
                serverThread = new Thread(new ThreadStart(ServerLoop));
                serverThread.Start();
            }
        }

        public void Stop()
        {
            if (isRunning)
            {
                isRunning = false;
                
                // Close all rooms and clients
                foreach (var room in gameRooms.Values)
                {
                    room.CloseRoom();
                }
                
                tcpListener?.Stop();
                serverThread?.Join(1000);
            }
        }

        private void ServerLoop()
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Any, port);
                tcpListener.Start();
                Console.WriteLine($"Server listening on port {port}");

                while (isRunning)
                {
                    if (tcpListener.Pending())
                    {
                        TcpClient client = tcpListener.AcceptTcpClient();
                        Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientConnection));
                        clientThread.Start(client);
                    }

                    // Cleanup empty rooms periodically
                    CleanupEmptyRooms();
                    
                    Thread.Sleep(10); // Prevent CPU overuse
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Server error: {e.Message}");
            }
        }

        private void HandleClientConnection(object clientObj)
        {
            TcpClient tcpClient = (TcpClient)clientObj;
            NetworkStream stream = tcpClient.GetStream();
            byte[] buffer = new byte[1024];
            string clientId = Guid.NewGuid().ToString();

            try
            {
                int bytesRead;
                
                // Read initial command from client
                bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"Received: {message}");
                    
                    // Process initial command
                    if (message.StartsWith("JOIN:"))
                    {
                        string roomCode = message.Substring(5);
                        ProcessRoomJoinRequest(roomCode, tcpClient, clientId);
                    }
                    else if (message.StartsWith("CREATE"))
                    {
                        string roomCode = CreateRoom();
                        
                        // Send room code back to client
                        byte[] roomCodeBytes = Encoding.UTF8.GetBytes("ROOM:" + roomCode);
                        stream.Write(roomCodeBytes, 0, roomCodeBytes.Length);
                        
                        // Join the room after creating it
                        ProcessRoomJoinRequest(roomCode, tcpClient, clientId);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Client connection error: {e.Message}");
                RemoveClientFromRoom(clientId);
                tcpClient.Close();
            }
        }

        private void ProcessRoomJoinRequest(string roomCode, TcpClient client, string clientId)
        {
            NetworkStream stream = client.GetStream();
            
            Console.WriteLine($"Processing join request for room: {roomCode}");
            
            if (string.IsNullOrEmpty(roomCode) || !gameRooms.ContainsKey(roomCode))
            {
                // Room doesn't exist
                byte[] errorMsg = Encoding.UTF8.GetBytes("ERROR:Room not found");
                stream.Write(errorMsg, 0, errorMsg.Length);
                client.Close();
                return;
            }

            GameRoom room = gameRooms[roomCode];
            
            if (room.GetPlayerCount() >= maxPlayersPerRoom)
            {
                // Room is full
                byte[] errorMsg = Encoding.UTF8.GetBytes("ERROR:Room is full");
                stream.Write(errorMsg, 0, errorMsg.Length);
                client.Close();
                return;
            }

            // Add player to room
            KartPlayer player = new KartPlayer(clientId, client);
            room.AddPlayer(player);
            
            // Send success message
            byte[] successMsg = Encoding.UTF8.GetBytes("SUCCESS:Joined room " + roomCode);
            stream.Write(successMsg, 0, successMsg.Length);
            Console.WriteLine($"Player {clientId} joined room {roomCode}");
            
            // Start handling player input in the room
            Thread playerThread = new Thread(() => HandlePlayerInput(player, room));
            playerThread.Start();
        }

        private void HandlePlayerInput(KartPlayer player, GameRoom room)
        {
            TcpClient client = player.TcpClient;
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];
            
            try
            {
                int bytesRead;
                while (isRunning && client.Connected && (bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    
                    // Process input message (e.g., "INPUT:W:1" for forward)
                    if (message.StartsWith("INPUT:"))
                    {
                        string[] parts = message.Substring(6).Split(':');
                        if (parts.Length == 2)
                        {
                            string key = parts[0];
                            int value = int.Parse(parts[1]);
                            player.UpdateInput(key, value);
                            
                            // Update player state based on input
                            room.UpdatePlayerState(player);
                            
                            // Broadcast new state to all players in room
                            room.BroadcastState();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Player input error: {e.Message}");
            }
            finally
            {
                RemoveClientFromRoom(player.Id);
                client.Close();
            }
        }

        private string CreateRoom()
        {
            string roomCode;
            do
            {
                // Generate a 6-character room code
                roomCode = GenerateRoomCode(6);
            } while (gameRooms.ContainsKey(roomCode));

            GameRoom newRoom = new GameRoom(roomCode);
            gameRooms.Add(roomCode, newRoom);
            Console.WriteLine($"Created room: {roomCode}");
            
            return roomCode;
        }

        private string GenerateRoomCode(int length)
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789"; // Omitting similar-looking characters
            char[] code = new char[length];
            Random random = new Random();
            
            for (int i = 0; i < length; i++)
            {
                code[i] = chars[random.Next(chars.Length)];
            }
            
            return new string(code);
        }

        private void RemoveClientFromRoom(string clientId)
        {
            foreach (var room in gameRooms.Values)
            {
                if (room.RemovePlayer(clientId))
                {
                    Console.WriteLine($"Player {clientId} left the room");
                    break;
                }
            }
        }

        private void CleanupEmptyRooms()
        {
            List<string> emptyRooms = new List<string>();
            
            foreach (var kvp in gameRooms)
            {
                if (kvp.Value.GetPlayerCount() == 0)
                {
                    emptyRooms.Add(kvp.Key);
                }
            }
            
            foreach (string roomCode in emptyRooms)
            {
                gameRooms.Remove(roomCode);
                Console.WriteLine($"Removed empty room: {roomCode}");
            }
        }
    }
}