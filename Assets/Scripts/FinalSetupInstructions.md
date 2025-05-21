# Simple Kart Setup Instructions

Follow these steps to set up the kart racing multiplayer system without any UI.

## Option 1: Quick Setup with SimpleKartSetup (Recommended)

1. Create an empty GameObject in your scene named "NetworkManager"
2. Add these components to it:
   - NetworkManager (Unity.Netcode)
   - Unity Transport (Unity.Netcode.Transports.UTP)
   - SimpleKartSetup

3. Configure SimpleKartSetup:
   - Assign your Player/Kart prefab
   - Check "Start As Host" for the server machine
   - Or leave it unchecked and enable "Auto Connect As Client" for clients

That's it! When you start the game, the server and clients will connect automatically.

## Option 2: Using Separate Components

If you prefer more control:

1. Create a "NetworkManager" GameObject with:
   - NetworkManager (Unity.Netcode)
   - Unity Transport (Unity.Netcode.Transports.UTP)

2. Create a "PlayerManager" GameObject with:
   - SimpleNetworkManager (assign player prefab)

3. Create a "Connector" GameObject with one of:
   - AutoConnector (has hardcoded CONNECTION_MODE)
   - CommandLineArgs (for command-line control)

## Setting Up Your Player Prefab

Your Player prefab must have:
1. A NetworkObject component
2. The NetworkKartMovement component

## Connection Options

### Option A: Using SimpleKartSetup
- Toggle "Start As Host" in the Inspector

### Option B: Using AutoConnector
- Edit the CONNECTION_MODE constant (0=client, 1=host)

### Option C: Using CommandLine
Start your game with command line arguments:
- `-host`: Start as the host
- `-client`: Start as a client
- `-ip <address>`: Connect to this IP
- `-port <number>`: Use this port

## Files You Need

- **SimpleKartSetup.cs** - All-in-one setup (recommended)
- **NetworkKartMovement.cs** - Drift physics for karts
- **SimpleNetworkManager.cs** - Player spawning (if using Option 2)
- **AutoConnector.cs** - Hardcoded connection (if using Option 2)
- **CommandLineArgs.cs** - Command-line connection (optional)