# Neighborhood Go-Kart Game - Map Class Technical Specification

## Overview
This document outlines the technical specifications for the `Map` class in our Unity-based go-kart racing game. The `Map` class is responsible for managing the neighborhood race track environment, including importing OpenStreetMap data, handling game elements, and managing level state.

## Class Dependencies
- `UnityEngine`
- `OSMImporter` (or alternative mapping solution)
- `KartPhysics`
- `PowerUpSystem`
- `ObstacleSystem`

## Class Structure

### Properties

```csharp
// Core Map Properties
public string MapName { get; private set; }
public float MapScale { get; private set; }
public Vector3 MapCenter { get; private set; }
public Bounds MapBounds { get; private set; }
public bool IsLoaded { get; private set; }

// Game Elements
public List<PowerUpItem> PowerUps { get; private set; }
public List<Obstacle> Obstacles { get; private set; }
public List<Checkpoint> Checkpoints { get; private set; }
public Transform StartingGrid { get; private set; }
public List<SpawnPoint> PlayerSpawnPoints { get; private set; }

// Environment
public TerrainData TerrainData { get; private set; }
public List<BuildingData> Buildings { get; private set; }
public List<RoadSegment> Roads { get; private set; }
public List<LandmarkData> Landmarks { get; private set; }
```

### Methods

```csharp
// Initialization
public void Initialize(string osmFilePath, MapSettings settings)
public async Task LoadMapAsync(bool showLoadingScreen = true)
public void UnloadMap()

// OSM Data Management
private void ImportOSMData(string osmFilePath)
private void ProcessRoadData(OSMData osmData)
private void ProcessBuildingData(OSMData osmData)
private void ProcessLandmarkData(OSMData osmData)
private void ApplyElevationData(HeightmapData heightmapData)

// Game Elements Management
public void PlacePowerUps(PowerUpPlacementStrategy strategy)
public void PlaceObstacles(ObstaclePlacementStrategy strategy)
public void GenerateCheckpoints(int numberOfCheckpoints)
public void SetupStartingGrid(int maxPlayers)

// Runtime Methods
public Vector3 GetRandomSpawnPoint()
public PowerUpItem GetNearestPowerUp(Vector3 position, float maxDistance)
public void ResetAllPowerUps()
public void RespawnPlayer(PlayerController player)
public bool IsWithinBounds(Vector3 position)

// Event Handlers
public event Action<Map> OnMapLoaded;
public event Action<Map> OnMapUnloaded;
public event Action<PowerUpItem> OnPowerUpCollected;
public event Action<Checkpoint> OnCheckpointPassed;
```

## Detailed Functionality

### Map Loading Workflow

1. **Initialization Phase**
   - Initialize map properties based on settings
   - Prepare loading resources

2. **OSM Data Import**
   - Import OSM data through the OSM Importer
   - Parse road, building, and landmark data
   - Generate appropriate Unity GameObjects

3. **Stylization**
   - Apply Mario Kart-style materials and effects
   - Scale and position objects according to map settings
   - Apply level-specific themes and decorations

4. **Game Element Placement**
   - Generate checkpoints along the main route
   - Place power-ups based on strategy (e.g., curves, straightaways)
   - Place obstacles to create challenging sections

5. **Finalization**
   - Initialize physics systems
   - Set up spawn points
   - Trigger OnMapLoaded event

### Power-Up System

The `PowerUps` array contains all power-up instances in the map. Each power-up has:
- Position/rotation data
- Type (speed boost, shield, projectile, etc.)
- Cooldown timer for respawning
- Visual effects

```csharp
// Power-up placement strategy
public void PlacePowerUps(PowerUpPlacementStrategy strategy)
{
    PowerUps = new List<PowerUpItem>();
    
    switch (strategy)
    {
        case PowerUpPlacementStrategy.Even:
            PlaceEvenlyOnTrack();
            break;
        case PowerUpPlacementStrategy.Clustered:
            PlaceClustered();
            break;
        case PowerUpPlacementStrategy.Custom:
            PlaceFromCustomData();
            break;
    }
}
```

### Obstacle System

The `Obstacles` array manages all race obstacles. Obstacles include:
- Static objects (cones, barriers)
- Dynamic obstacles (moving vehicles, pedestrians)
- Environmental hazards (oil slicks, puddles)

```csharp
// Example obstacle management
public void UpdateObstacles(float deltaTime)
{
    foreach (var obstacle in Obstacles)
    {
        if (obstacle.IsMoving)
        {
            obstacle.UpdatePosition(deltaTime);
        }
        
        if (obstacle.HasLifetime && obstacle.RemainingLifetime <= 0)
        {
            RemoveObstacle(obstacle);
        }
    }
}
```

### Map Unloading

When the player exits the game or selects a different map:

```csharp
public void UnloadMap()
{
    if (!IsLoaded) return;
    
    // Dispose of game elements
    PowerUps.Clear();
    Obstacles.Clear();
    Checkpoints.Clear();
    
    // Clean up environment
    foreach (var building in Buildings)
    {
        Destroy(building.GameObject);
    }
    Buildings.Clear();
    
    // Similar cleanup for roads and landmarks
    
    // Reset state
    IsLoaded = false;
    
    // Trigger event
    OnMapUnloaded?.Invoke(this);
}
```

## Integration with OpenStreetMap

The Map class will use one of these methods to import OSM data:

1. **OSM Importer for Unity**
   ```csharp
   private void ImportUsingOSMImporter(string osmFilePath)
   {
       var importer = new OSMImporter();
       importer.Import(osmFilePath, MapCenter, MapScale);
       
       Roads = importer.GetRoads();
       Buildings = importer.GetBuildings();
       Landmarks = importer.GetLandmarks();
   }
   ```

2. **Mapbox SDK Integration**
   ```csharp
   private void ImportUsingMapbox(Vector2d coordinates, float zoom)
   {
       var mapInstance = MapboxMap.Instance;
       mapInstance.Initialize(coordinates, zoom);
       mapInstance.OnMapLoaded += OnMapboxDataLoaded;
       mapInstance.LoadMap();
   }
   ```

## Performance Considerations

- Level of detail (LOD) system for distant buildings
- Object pooling for power-ups and obstacles
- Asynchronous loading of map sections
- Optimized colliders for roads and obstacles
- Quad-tree spatial partitioning for large maps

## Future Enhancements

- Dynamic time-of-day system
- Weather effects
- Traffic system for non-player vehicles
- Pedestrian AI
- Destructible environment elements

## Usage Example

```csharp
// Example usage in GameManager
public class GameManager : MonoBehaviour
{
    public Map neighborhoodMap;
    public string osmFilePath = "Assets/Maps/MyNeighborhood.osm";
    
    async void StartGame()
    {
        MapSettings settings = new MapSettings
        {
            MapName = "My Neighborhood",
            MapScale = 1.0f,
            MapCenter = Vector3.zero,
            StyleSettings = MapStyle.MarioKart
        };
        
        neighborhoodMap.Initialize(osmFilePath, settings);
        await neighborhoodMap.LoadMapAsync(true);
        
        neighborhoodMap.PlacePowerUps(PowerUpPlacementStrategy.Even);
        neighborhoodMap.PlaceObstacles(ObstaclePlacementStrategy.Medium);
        neighborhoodMap.GenerateCheckpoints(12);
        neighborhoodMap.SetupStartingGrid(4);
        
        // Start race
        RaceController.StartCountdown();
    }
}
```
