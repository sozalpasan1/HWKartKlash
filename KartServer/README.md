# Kart Racing Dedicated Server

This folder contains a simple standalone server implementation for the Kart Racing game. 
The server creates a single persistent room where all players join automatically.

## How to Use

### Running the Server

1. Build the server application:
   ```
   cd KartServer
   dotnet build
   ```

2. Run the server:
   ```
   dotnet run
   ```

   Optionally, specify a custom port:
   ```
   dotnet run -- 8080
   ```

3. The server will start and create a single room for all players. You'll see logs in the console.

4. To stop the server, press Ctrl+C.

## Integrating with Unity Client

### Easy Integration Steps

The Unity client is already configured to automatically join the server. Here's what happens:

1. The `KartGameManager` in the Unity project automatically connects to the server.
2. When a player joins, the server assigns them a spawn position and instantiates a kart for them.
3. All player karts are networked and synchronized automatically.

### Configuration

To configure the server connection:

1. In Unity, find the `KartGameManager` component in your scene.
2. Set the `Server IP` field to the server's IP address (default: 127.0.0.1)
3. Set the `Server Port` field to match the server's port (default: 7777)
4. Make sure `Auto Start Client` is checked to automatically connect

### Server-Only Build (Optional)

For dedicated server deployments:

1. Create a separate Unity build with just the server scene.
2. Add the `DedicatedServerScript` component to an object in the scene.
3. This build will automatically start in server-only mode.

## How It Works

- The server creates a single room that everyone joins
- Players are automatically spawned with their own drift kart
- Physics and movement are synchronized over the network
- The server handles all player connections/disconnections automatically

No UI needs to be shown - players are dropped directly into the game when they connect!