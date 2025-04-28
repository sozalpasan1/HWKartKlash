# HW KartKlash - MultiplayerSpec.md

## Overview

This document outlines the technical specifications for implementing multiplayer functionality in the HW KartKlash racing game. The game is a multiplayer racing experience set on the Harvard-Westlake campus, featuring custom karts, teacher characters with unique abilities, and various power-ups.

## Technology Stack

### Game Engine
- **Unity 2022.3 LTS** or newer

#### Primary Solution: Photon PUN 2
- **Package**: Photon Unity Networking 2 (PUN 2)
- **Version**: 2.43 or newer
- **Purpose**: Main networking framework for player synchronization, room management, and matchmaking

#### Secondary Solution
- **Unity Netcode for GameObjects**
  - **Version**: 1.5.1 or newer
  - **Purpose**: For specific Unity-optimized networking features

#### Voice Communication
- **Photon Voice 2**
  - **Version**: 2.50 or newer
  - **Purpose**: Implementation of proximity voice chat

## Architecture Overview

```
┌─────────────────────────────────────┐
│           Client Application         │
├─────────────┬──────────┬─────────────┤
│  Game Logic │ UI Layer │ Input Layer │
├─────────────┴──────────┴─────────────┤
│         Networking Manager           │
├───────────────────┬─────────────────┤
│   Photon Client   │   Voice Client  │
└───────────────────┴─────────────────┘
            ▲                ▲
            │                │
            ▼                ▼
┌───────────────────┬─────────────────┐
│   Photon Cloud    │  Voice Servers  │
└───────────────────┴─────────────────┘
```

## Feature Specifications

### 1. Network Management System
**Related Design Elements**: "Multiplayer racing game", "Game Features"

#### NetworkManager
* **Variables Expected**:
  * `instance`: Reference to the NetworkManager
  * `maxPlayersPerRoom`: Maximum number of players allowed in a race (8)
  * `gameVersion`: Current version of the game for compatibility checking
  * `roomName`: Default room name format for created rooms
  * `roomProperties`: Dictionary for storing custom room settings
  * `connectionState`: Current connection status to Photon servers
  * `autoReconnect`: Flag for automatic reconnection attempts

* **Methods Expected**:
  * `Initialize()`: Setup the Photon client configuration
  * `Connect()`: Connect to Photon network servers
  * `Disconnect()`: Properly disconnect from Photon services
  * `CreateRoom()`: Create a new room with specified settings
  * `JoinRoom()`: Join an existing room by name
  * `JoinRandomRoom()`: Join any available room
  * `LeaveRoom()`: Exit the current room
  * `OnConnectionStatusChanged()`: Event handler for connection state changes
  * `OnRoomPropertiesUpdated()`: Handle room property changes
  * `GetPing()`: Retrieve current connection latency

* **Related Objects**:
  * LobbyManager: For room listings and matchmaking
  * GameStateManager: For synchronizing game state across network
  * PlayerManager: For player registration and management

### 2. Player Synchronization System
**Related Design Elements**: "All racing karts have identical performance specifications", "Racers can collect various powerups throughout the track"

#### PlayerNetworking
* **Variables Expected**:
  * `playerID`: Unique identifier for each player
  * `playerName`: Display name of the player
  * `selectedCharacter`: Currently selected teacher character
  * `position`: Current position in world space
  * `rotation`: Current rotation in world space
  * `velocity`: Current velocity vector
  * `currentLap`: Player's current lap number
  * `currentPowerup`: Currently held powerup ID
  * `positionInterpolationSpeed`: Speed to smooth position updates
  * `rotationInterpolationSpeed`: Speed to smooth rotation updates
  * `isLocal`: Whether this is the local player instance

* **Methods Expected**:
  * `Initialize()`: Setup player networking components
  * `SyncPosition()`: Update position data across the network
  * `SyncRotation()`: Update rotation data across the network
  * `SyncCheckpoint()`: Synchronize checkpoint crossing events
  * `ApplyPowerup()`: Activate and synchronize powerup effects
  * `UseAbility()`: Trigger character-specific abilities
  * `UpdateRacePosition()`: Update player's position in the race
  * `OnPlayerDisconnected()`: Handle player disconnection events
  * `RequestOwnership()`: Take control of this player object

* **Related Objects**:
  * KartController: For controlling kart physics and input
  * PowerupManager: For powerup activation and effects
  * TeacherAbilityManager: For character-specific abilities
  * RaceManager: For race progression tracking


### 3. Room and Matchmaking System (This is lower priority, should get 1 room working first)
**Related Design Elements**: "Multiplayer racing game"

#### LobbyManager
* **Variables Expected**:
  * `availableRooms`: List of currently active game rooms
  * `roomFilters`: Filters for room searches (map, player count, etc.)
  * `customRoomProperties`: Properties for room creation
  * `refreshInterval`: How often to refresh room listings
  * `maxVisibleRooms`: Maximum number of rooms to display in UI
  * `defaultRoomSettings`: Default settings for created rooms
  * `mapOptions`: Available maps for selection
  * `playerCountOptions`: Available player count settings
  * `lastRefreshTime`: Timestamp of last room list refresh

* **Methods Expected**:
  * `RefreshRoomList()`: Update the list of available rooms
  * `CreateRoom()`: Create a new room with specified settings
  * `JoinSelectedRoom()`: Join a room selected from the list
  * `FilterRooms()`: Apply filters to room listings
  * `UpdateRoomProperties()`: Modify settings of current room
  * `LeaveCurrentRoom()`: Exit the current room and return to lobby
  * `OnRoomListUpdated()`: Handle updated room list from server
  * `DisplayRoomDetails()`: Show detailed information for a room
  * `IsRoomFull()`: Check if a specific room has reached capacity
  * `GetRoomPing()`: Get estimated connection quality to a room

* **Related Objects**:
  * NetworkManager: For connection management
  * UIManager: For displaying room listings and details
  * GameStateManager: For transitioning between lobby and game
  * PlayerPrefs: For saving preferred room settings



