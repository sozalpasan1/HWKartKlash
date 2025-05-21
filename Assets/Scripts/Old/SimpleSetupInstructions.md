# Simple Setup Instructions

Here's how to set up the kart multiplayer system:

## Step 1: Add NetworkManager to your scene

1. Right-click in your Hierarchy window
2. Select "Create Empty" and name it "NetworkManager"
3. Add these components to it:
   - NetworkManager (Unity.Netcode)
   - Unity Transport (Unity.Netcode.Transports.UTP)

## Step 2: Add SimpleKartSetup to your scene

1. Right-click in your Hierarchy window
2. Select "Create Empty" and name it "KartManager"
3. Add the `SimpleKartSetup` component to it (not KartGameManager)

## Step 3: Assign your Player Prefab

1. Ensure your Player/Kart prefab has:
   - NetworkObject component
   - NetworkKartMovement component
2. Drag your Player prefab to the "Player Prefab" field in SimpleKartSetup

## Step 4: Configure Options

- **Start As Host**: Turn ON if this instance should be the host (one player must be host)
- **Auto Connect As Client**: Turn ON for clients to automatically connect to the server

## That's it!

Play the game and everything should work - no UI needed. Each player gets their own kart.

## Troubleshooting

If you get "Can't add script component because the script class cannot be found":

1. Use `SimpleKartSetup` instead of KartGameManager
2. Make sure all scripts are properly compiled (no errors in the console)
3. If issues persist, restart Unity

For other issues, refer to the full ServerSetupInstructions.md