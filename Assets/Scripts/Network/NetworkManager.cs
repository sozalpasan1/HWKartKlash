using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance { get; private set; }
    
    [Header("Server Settings")]
    [SerializeField] private string serverIP = "127.0.0.1";
    [SerializeField] private int serverPort = 7777;
    
    [Header("Prefabs")]
    [SerializeField] private GameObject playerKartPrefab;
    [SerializeField] private GameObject otherPlayerKartPrefab;
    
    // Network connection
    private TcpClient tcpClient;
    private NetworkStream stream;
    private Thread receiveThread;
    private bool isConnected = false;
    
    // Game state
    private string playerId;
    private string currentRoomCode = "";
    private GameObject localPlayerKart;
    private Dictionary<string, GameObject> otherPlayerKarts = new Dictionary<string, GameObject>();
    
    // Events
    public event Action<string> OnRoomJoined;
    public event Action<string> OnConnectionError;
    public event Action<string> OnPlayerJoined;
    public event Action<string> OnPlayerLeft;
    
    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    private void OnDestroy()
    {
        CloseConnection();
    }
    
    public void CreateRoom()
    {
        CloseConnection();
        StartCoroutine(ConnectToServer("CREATE"));
    }
    
    public void JoinRoom(string roomCode)
    {
        if (string.IsNullOrEmpty(roomCode))
        {
            OnConnectionError?.Invoke("Room code cannot be empty");
            return;
        }
        
        CloseConnection();
        StartCoroutine(ConnectToServer("JOIN:" + roomCode));
    }
    
    private IEnumerator ConnectToServer(string initialCommand)
    {
        try
        {
            // Connect to server
            tcpClient = new TcpClient();
            tcpClient.Connect(serverIP, serverPort);
            
            if (tcpClient.Connected)
            {
                isConnected = true;
                stream = tcpClient.GetStream();
                
                // Send initial command (CREATE or JOIN:roomCode)
                byte[] commandBytes = Encoding.UTF8.GetBytes(initialCommand);
                stream.Write(commandBytes, 0, commandBytes.Length);
                
                // Start receiving thread
                receiveThread = new Thread(new ThreadStart(ReceiveData));
                receiveThread.IsBackground = true;
                receiveThread.Start();
                
                Debug.Log("Connected to server");
            }
            else
            {
                OnConnectionError?.Invoke("Failed to connect to server");
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Connection error: " + e.Message);
            OnConnectionError?.Invoke("Connection error: " + e.Message);
            CloseConnection();
        }
        
        yield return null;
    }
    
    private void ReceiveData()
    {
        byte[] buffer = new byte[1024];
        
        try
        {
            while (isConnected && tcpClient != null && tcpClient.Connected)
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    ProcessMessage(message);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Receive error: " + e.Message);
            
            // Notify UI on main thread
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                OnConnectionError?.Invoke("Connection lost: " + e.Message);
            });
            
            CloseConnection();
        }
    }
    
    private void ProcessMessage(string message)
    {
        // Process various message types
        if (message.StartsWith("ROOM:"))
        {
            // Room created successfully
            currentRoomCode = message.Substring(5);
            
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                OnRoomJoined?.Invoke(currentRoomCode);
            });
        }
        else if (message.StartsWith("SUCCESS:"))
        {
            // Joined room successfully
            string successMsg = message.Substring(8);
            
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                Debug.Log("Join success: " + successMsg);
                
                // If we joined a room, spawn the local player
                SpawnLocalPlayer();
                
                OnRoomJoined?.Invoke(currentRoomCode);
            });
        }
        else if (message.StartsWith("ERROR:"))
        {
            // Error occurred
            string errorMsg = message.Substring(6);
            
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                OnConnectionError?.Invoke(errorMsg);
                CloseConnection();
            });
        }
        else if (message.StartsWith("JOIN:"))
        {
            // Another player joined
            string[] parts = message.Substring(5).Split(':');
            if (parts.Length >= 5)
            {
                string newPlayerId = parts[0];
                float posX = float.Parse(parts[1]);
                float posY = float.Parse(parts[2]);
                float posZ = float.Parse(parts[3]);
                float rotY = float.Parse(parts[4]);
                
                UnityMainThreadDispatcher.Instance().Enqueue(() => {
                    SpawnOtherPlayer(newPlayerId, new Vector3(posX, posY, posZ), rotY);
                    OnPlayerJoined?.Invoke(newPlayerId);
                });
            }
        }
        else if (message.StartsWith("LEAVE:"))
        {
            // Player left
            string leftPlayerId = message.Substring(6);
            
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                RemoveOtherPlayer(leftPlayerId);
                OnPlayerLeft?.Invoke(leftPlayerId);
            });
        }
        else if (message.StartsWith("FULLSTATE:"))
        {
            // Full state update with all players
            string[] playerDataArray = message.Substring(10).Split('|');
            
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                UpdateAllPlayers(playerDataArray);
            });
        }
    }
    
    private void SpawnLocalPlayer()
    {
        // Spawn local player kart if not already spawned
        if (localPlayerKart == null)
        {
            localPlayerKart = Instantiate(playerKartPrefab, Vector3.zero, Quaternion.identity);
            
            // Configure local player
            KartController controller = localPlayerKart.GetComponent<KartController>();
            if (controller != null)
            {
                controller.IsLocalPlayer = true;
            }
        }
    }
    
    private void SpawnOtherPlayer(string id, Vector3 position, float rotation)
    {
        // Don't spawn if this is us or if player already exists
        if (id == playerId || otherPlayerKarts.ContainsKey(id))
        {
            return;
        }
        
        // Spawn other player kart
        GameObject otherKart = Instantiate(otherPlayerKartPrefab, position, Quaternion.Euler(0, rotation, 0));
        
        // Configure remote player
        KartController controller = otherKart.GetComponent<KartController>();
        if (controller != null)
        {
            controller.IsLocalPlayer = false;
            controller.PlayerId = id;
        }
        
        otherPlayerKarts[id] = otherKart;
    }
    
    private void RemoveOtherPlayer(string id)
    {
        if (otherPlayerKarts.TryGetValue(id, out GameObject kart))
        {
            Destroy(kart);
            otherPlayerKarts.Remove(id);
        }
    }
    
    private void UpdateAllPlayers(string[] playerDataArray)
    {
        foreach (string playerData in playerDataArray)
        {
            string[] parts = playerData.Split(':');
            if (parts.Length >= 6)
            {
                string id = parts[0];
                float posX = float.Parse(parts[1]);
                float posY = float.Parse(parts[2]);
                float posZ = float.Parse(parts[3]);
                float rotY = float.Parse(parts[4]);
                float speed = float.Parse(parts[5]);
                
                Vector3 position = new Vector3(posX, posY, posZ);
                
                // If this is our first state update, set our ID
                if (string.IsNullOrEmpty(playerId))
                {
                    // The first player in the list with matching position to our local kart is us
                    if (localPlayerKart != null && 
                        Vector3.Distance(localPlayerKart.transform.position, position) < 0.1f)
                    {
                        playerId = id;
                        
                        // Set player ID on controller
                        KartController controller = localPlayerKart.GetComponent<KartController>();
                        if (controller != null)
                        {
                            controller.PlayerId = playerId;
                        }
                    }
                }
                
                // Update other player karts
                if (id != playerId)
                {
                    // Spawn if doesn't exist
                    if (!otherPlayerKarts.ContainsKey(id))
                    {
                        SpawnOtherPlayer(id, position, rotY);
                    }
                    else
                    {
                        // Update existing kart
                        GameObject kart = otherPlayerKarts[id];
                        
                        // Use interpolation for smoother movement
                        KartController controller = kart.GetComponent<KartController>();
                        if (controller != null)
                        {
                            controller.SetTargetTransform(position, rotY, speed);
                        }
                    }
                }
            }
        }
    }
    
    public void SendInput(string key, bool isPressed)
    {
        if (!isConnected || stream == null)
        {
            return;
        }
        
        try
        {
            // Format: INPUT:key:value (1 for pressed, 0 for released)
            string inputMessage = $"INPUT:{key}:{(isPressed ? 1 : 0)}";
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputMessage);
            stream.Write(inputBytes, 0, inputBytes.Length);
        }
        catch (Exception e)
        {
            Debug.LogError("Send input error: " + e.Message);
        }
    }
    
    public void CloseConnection()
    {
        isConnected = false;
        
        if (receiveThread != null && receiveThread.IsAlive)
        {
            receiveThread.Abort();
            receiveThread = null;
        }
        
        if (stream != null)
        {
            stream.Close();
            stream = null;
        }
        
        if (tcpClient != null)
        {
            tcpClient.Close();
            tcpClient = null;
        }
        
        currentRoomCode = "";
    }
}