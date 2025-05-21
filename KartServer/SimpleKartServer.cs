using System;
using System.Threading.Tasks;
using System.Collections.Generic;

/// <summary>
/// A simplified standalone server that creates a single room for all players.
/// </summary>
public class SimpleKartServer
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("====================================");
        Console.WriteLine("=== Kart Racing Dedicated Server ===");
        Console.WriteLine("====================================");
        
        // Default port, can be overridden via command-line args
        ushort port = 7777;
        
        // Parse command-line arguments
        if (args.Length > 0)
        {
            if (ushort.TryParse(args[0], out ushort customPort))
            {
                port = customPort;
            }
        }
        
        Console.WriteLine($"Creating single persistent room for all players");
        Console.WriteLine($"Server listening on port: {port}");
        Console.WriteLine("Press Ctrl+C to shut down the server");
        
        // Create a simple server
        var server = new SingleRoomKartServer(port);
        await server.Start();
        
        // Keep the server running until manually terminated
        var tcs = new TaskCompletionSource<bool>();
        Console.CancelKeyPress += (s, e) => 
        {
            e.Cancel = true;
            tcs.SetResult(true);
        };
        
        await tcs.Task;
        
        // Cleanup
        await server.Stop();
        Console.WriteLine("Server shut down successfully");
    }
}

class SingleRoomKartServer
{
    private readonly ushort _port;
    private readonly Dictionary<string, PlayerData> _connectedPlayers = new Dictionary<string, PlayerData>();
    private readonly object _playersLock = new object();
    
    // Simulation variables
    private bool _running = false;
    private Task? _simulationTask;
    
    public SingleRoomKartServer(ushort port)
    {
        _port = port;
    }
    
    public async Task Start()
    {
        // In a real implementation, this would initialize network listeners
        await Task.Delay(1000);
        Console.WriteLine("Server started and ready for connections");
        
        // Start a background simulation
        _running = true;
        _simulationTask = Task.Run(RunSimulationLoop);
        
        Console.WriteLine("Created a single persistent room");
    }
    
    public async Task Stop()
    {
        // Stop simulation
        _running = false;
        if (_simulationTask != null)
        {
            await _simulationTask;
        }
        
        // In a real implementation, this would clean up network resources
        await Task.Delay(500);
        
        lock (_playersLock)
        {
            _connectedPlayers.Clear();
        }
    }
    
    private async Task RunSimulationLoop()
    {
        // A simple simulation that updates every 100ms
        while (_running)
        {
            // This could update game state, handle collisions, etc.
            lock (_playersLock)
            {
                if (_connectedPlayers.Count > 0)
                {
                    Console.WriteLine($"Room status: {_connectedPlayers.Count} players connected");
                }
            }
            
            // Wait before next update
            await Task.Delay(5000);
        }
    }
    
    // Methods that would be called by network event handlers
    public void AddPlayer(string playerId)
    {
        var player = new PlayerData { Id = playerId };
        
        lock (_playersLock)
        {
            _connectedPlayers[playerId] = player;
        }
        
        Console.WriteLine($"Player {playerId} connected. Total players: {_connectedPlayers.Count}");
    }
    
    public void RemovePlayer(string playerId)
    {
        lock (_playersLock)
        {
            _connectedPlayers.Remove(playerId);
        }
        
        Console.WriteLine($"Player {playerId} disconnected. Total players: {_connectedPlayers.Count}");
    }
}

// Using PlayerData instead of KartPlayer to avoid naming conflict
class PlayerData
{
    public string Id { get; set; } = string.Empty;
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
    public float RotationY { get; set; }
}