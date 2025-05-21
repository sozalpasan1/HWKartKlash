# Simple Drift Kart Server Setup

This document explains how to quickly set up the drift kart multiplayer system where everyone joins the same room automatically.

## Quick Setup (One-Click Solution)

1. Add the `NetworkManager` prefab to your scene
2. Add the `SimpleKartSetup` component to any GameObject in your scene
3. Assign your Player/Kart prefab to the `Player Prefab` field
4. Make sure your Player prefab has:
   - A `NetworkObject` component
   - The `NetworkKartMovement` script

That's it! When you run the game:
- The first player to join will automatically be the host
- Other players will automatically connect
- Each player gets their own drift kart

## Option 1: Use the included Unity Host (Easiest)

For quick local testing or LAN play:

1. Open the game as normal
2. Set `Start As Host` to true on the SimpleKartSetup component
3. Run the game - this instance will be the server + player
4. Run additional instances of the game as clients (keep `Start As Host` unchecked)

## Option 2: Use the Standalone Server

For dedicated server setups:

1. Navigate to the `KartServer` folder
2. Run the server:
   ```
   dotnet run
   ```
3. The server will create a single room for all players
4. Launch the Unity game with `Start As Host` unchecked to connect as clients

## Port and IP Configuration

- Default server port: 7777
- Default server IP: 127.0.0.1 (local)

To change these:
- Modify the `Server IP` and `Server Port` in the SimpleKartSetup component
- If using the standalone server, specify a port: `dotnet run -- 8080`

## Troubleshooting

- **Cannot connect:** Make sure the server IP and port are correct
- **No Kart spawns:** Ensure Player prefab has a NetworkObject component
- **No movement:** Check that NetworkKartMovement script is on the Player prefab