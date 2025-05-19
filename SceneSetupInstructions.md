# Kart Rebellion - Scene Setup Instructions

This document explains how to set up the Unity scenes for the Kart Rebellion multiplayer game.

## Required Scenes

You need to create two scenes:
1. **LobbyScene** - For creating/joining rooms
2. **GameScene** - For the actual gameplay

## LobbyScene Setup

1. Create a new scene and save it as "LobbyScene"
2. Add a Canvas with UI elements:
   - Main Panel
     - Title Text: "Kart Rebellion"
     - Create Room Button
     - Join Room Button
   - Create Room Panel (initially hidden)
     - Create button
     - Back button
   - Join Room Panel (initially hidden)
     - Room Code Input Field
     - Join button
     - Back button
   - Waiting Room Panel (initially hidden)
     - Room Code Text
     - Player Count Text
     - Start Game Button
     - Leave Room Button
3. Add the UnityMainThreadDispatcher and NetworkManager scripts:
   - Create an empty GameObject named "Managers"
   - Add the NetworkManager.cs script
   - Add the UnityMainThreadDispatcher.cs script
4. Add the LobbyManager script to the Canvas
5. Connect all UI elements to the LobbyManager in the Inspector

## GameScene Setup

1. Create a new scene and save it as "GameScene"
2. Create a simple track:
   - Add a Plane or Terrain for the ground
   - Add obstacles, walls, or other track elements
   - You can use basic primitives or import models
3. Set up the player kart prefab:
   - Create a new empty GameObject named "PlayerKart"
   - Add basic kart components (cube for body, cylinders for wheels)
   - Add a Rigidbody component
   - Add a Collider component (Box Collider or Mesh Collider)
   - Add the KartController.cs script
   - Set IsLocalPlayer to true
   - Save as a prefab in the Prefabs folder
4. Set up the other player kart prefab:
   - Duplicate the PlayerKart prefab
   - Rename it to "OtherPlayerKart"
   - Change the material color to distinguish from the player kart
   - Keep IsLocalPlayer as false
   - Save as a prefab in the Prefabs folder
5. Set up the camera:
   - Add a new Camera GameObject
   - Position it behind and slightly above where the kart will spawn
   - Add the KartCameraController.cs script
6. Add managers:
   - Create an empty GameObject named "Managers"
   - Add the NetworkManager script
   - Add the UnityMainThreadDispatcher script
   - Assign the player kart and other player kart prefabs in the inspector
7. Optional: Add UI elements:
   - Room code display
   - Player count
   - Basic instructions

## Build Settings

1. Open Build Settings (File > Build Settings)
2. Add both scenes to the build
3. Set LobbyScene as the first scene

## Server Setup

1. Build the KartServer project
2. Run the server before launching the game
3. Make sure the port (default: 7777) is open if playing online

## Gameplay Flow

1. Start in LobbyScene
2. Create a room or join an existing one using a room code
3. When ready, the host clicks "Start Game"
4. Game transitions to GameScene
5. Players control karts using WASD + Space
6. All input is sent to the server, which handles physics and state synchronization