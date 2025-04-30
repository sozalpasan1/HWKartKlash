## Technology Stack

### Game Engine
- **Unity 2022.3 LTS** or newer

#### Multiplayer Networking:
- **Unity Netcode for GameObjects**
  - **Version**: 1.5.1 or newer
  - **Purpose**: For specific Unity-optimized networking features


## Feature Specifications

### 1. Network Management System
#### NetworkManager
* **Variables Expected**:
  * `instance`: Reference to the NetworkManager
  * `maxPlayersPerRoom`: Maximum number of players allowed in a race (8)
  * `gameVersion`: Current version of the game for compatibility checking
  * `roomName`: Default room name format for created rooms
  * Lower Priority:
  * `roomProperties`: Dictionary for storing custom room settings
  * `connectionState`: Current connection status to servers
  * `autoReconnect`: Flag for automatic reconnection attempts

* **Methods Expected**:
  * `Initialize()`: Setup the client configuration
  * `Connect()`: Connect to network servers
  * `Disconnect()`: Properly disconnect from services
  * `CreateRoom()`: Create a new room with specified settings
  * `JoinRoom()`: Join an existing room by name
  * `LeaveRoom()`: Exit the current room to lobby
  * Lower Priority:
  * `JoinRandomRoom()`: Join any available room
  * `OnConnectionStatusChanged()`: Event handler for connection state changes
  * `GetPing()`: Retrieve current connection latency

* **Related Objects**:
  * LobbyManager: For room listings and matchmaking
  * GameStateManager: For synchronizing game state across network
  * PlayerManager: For player registration and management

### 2. Player Synchronization System

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


### 3. Room and Matchmaking System (All this is lower priority, should get 1 room working first)

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



