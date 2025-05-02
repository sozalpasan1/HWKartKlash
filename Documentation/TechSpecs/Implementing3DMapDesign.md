# 3D Neighborhood Go-Kart Game - Map Class Technical Specification

## Overview
This document outlines the technical specifications for the `Map3D` class in our Unity-based go-kart racing game. The `Map3D` class is responsible for managing a 3D photorealistic neighborhood race track environment using Google Maps 3D Tiles and Cesium for Unity integration.

## Class Dependencies
- `UnityEngine`
- `Cesium for Unity` (3D tileset integration)
- `Google Maps Platform` (Map Tiles API)
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

// Cesium and Google Maps Integration
public CesiumIonRasterOverlay CesiumOverlay { get; private set; }
public Cesium3DTileset TileSet { get; private set; }
public CesiumGeoreference Georeference { get; private set; }
public CesiumSubScene NeighborhoodSubScene { get; private set; }

// Game Elements
public List<PowerUpItem> PowerUps { get; private set; }
public List<Obstacle> Obstacles { get; private set; }
public List<Checkpoint> Checkpoints { get; private set; }
public Transform StartingGrid { get; private set; }
public List<SpawnPoint> PlayerSpawnPoints { get; private set; }

// Environment Properties
public float ElevationScale { get; private set; }
public float TilesetLoadDistance { get; private set; }
public float ScreenSpaceError { get; set; }
```

### Methods

```csharp
// Initialization
public void Initialize(double latitude, double longitude, float radius, Map3DSettings settings)
public async Task LoadMapAsync(bool showLoadingScreen = true)
public void UnloadMap()

// Cesium and Google Maps Integration
private void ConfigureGeoreference(double latitude, double longitude, float height)
private void Create3DTileset(string apiKey)
private void CreateSubScene(string name, double latitude, double longitude, double height, double radius)
private void ConfigureTilesetOptions(float screenSpaceError, int maximumScreenSpaceError)

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

// Performance Optimization
public void OptimizeTileset(PerformancePreset preset)
public void SetLODSettings(float distanceMultiplier)
public void EnableOcclusionCulling(bool enable)

// Event Handlers
public event Action<Map3D> OnMapLoaded;
public event Action<Map3D> OnMapUnloaded;
public event Action<PowerUpItem> OnPowerUpCollected;
public event Action<Checkpoint> OnCheckpointPassed;
```

## Detailed Functionality

### 3D Tileset Configuration

```csharp
private void Create3DTileset(string googleMapsApiKey)
{
    // Create Cesium 3D Tileset
    GameObject tilesetGameObject = new GameObject("3DTileset");
    TileSet = tilesetGameObject.AddComponent<Cesium3DTileset>();
    
    // Configure tileset with Google Maps URL
    TileSet.url = $"https://tile.googleapis.com/v1/3dtiles/root.json?key={googleMapsApiKey}";
    TileSet.maximumScreenSpaceError = 16; // Balance between quality and performance
    TileSet.preloadAncestors = true;
    TileSet.preloadSiblings = false;
    TileSet.forbidHoles = true;
    TileSet.showCreditsOnScreen = true; // Required by Google's terms of service
    
    // Create and configure raster overlay if needed
    CesiumIonRasterOverlay rasterOverlay = tilesetGameObject.AddComponent<CesiumIonRasterOverlay>();
    rasterOverlay.ionAssetID = YOUR_CESIUM_ION_ASSET_ID; // Replace with your satellite imagery asset
    rasterOverlay.maximumTextureSize = 2048;
    rasterOverlay.maximumScreenSpaceError = 2.0;
    
    CesiumOverlay = rasterOverlay;
}
```

### Sub-Scene Implementation

```csharp
public void CreateNeighborhoodSubScene(string name, double latitude, double longitude, double height, double radius)
{
    // Create sub-scene game object
    GameObject subSceneObject = new GameObject(name);
    NeighborhoodSubScene = subSceneObject.AddComponent<CesiumSubScene>();
    
    // Configure sub-scene
    NeighborhoodSubScene.latitude = latitude;
    NeighborhoodSubScene.longitude = longitude;
    NeighborhoodSubScene.height = height;
    NeighborhoodSubScene.extentInMeters = radius;
    
    // Set up georeference
    if (Georeference == null)
    {
        GameObject georefObject = new GameObject("Georeference");
        Georeference = georefObject.AddComponent<CesiumGeoreference>();
        Georeference.latitude = latitude;
        Georeference.longitude = longitude;
        Georeference.height = height;
    }
    
    // Link sub-scene to georeference
    NeighborhoodSubScene.georeference = Georeference;
    
    // Create sub-scene
    NeighborhoodSubScene.CreateSubScene();
    
    // Set map center and bounds
    MapCenter = NeighborhoodSubScene.transform.position;
    MapBounds = new Bounds(MapCenter, new Vector3((float)radius * 2, 1000, (float)radius * 2));
}
```

### Collision Management

```csharp
public void SetupCollisionForTileset()
{
    // Wait for tileset to load a bit before adding colliders
    StartCoroutine(AddCollidersAfterLoading());
}

private IEnumerator AddCollidersAfterLoading()
{
    // Wait for initial tileset loading
    yield return new WaitForSeconds(5.0f);
    
    // Find all tile objects
    Transform tilesetTransform = TileSet.transform;
    List<MeshFilter> meshFilters = new List<MeshFilter>();
    
    // Collect all meshes in the tileset
    CollectAllMeshFilters(tilesetTransform, meshFilters);
    
    // Add colliders to ground and buildings
    foreach (MeshFilter meshFilter in meshFilters)
    {
        // Skip very complex meshes
        if (meshFilter.sharedMesh.vertexCount > 10000)
            continue;
            
        // Check if this is likely ground or a building
        bool isGround = IsLikelyGround(meshFilter.sharedMesh);
        
        if (isGround)
        {
            // Add mesh collider for ground
            MeshCollider collider = meshFilter.gameObject.AddComponent<MeshCollider>();
            collider.sharedMesh = meshFilter.sharedMesh;
            collider.convex = false;
        }
        else
        {
            // For buildings, use simplified box colliders
            BoxCollider collider = meshFilter.gameObject.AddComponent<BoxCollider>();
            // Adjust collider size based on mesh bounds
            collider.size = meshFilter.sharedMesh.bounds.size * 0.9f;
            collider.center = meshFilter.sharedMesh.bounds.center;
        }
    }
    
    Debug.Log("Collision setup complete for tileset");
}

private void CollectAllMeshFilters(Transform parent, List<MeshFilter> meshFilters)
{
    // Get mesh filter on current object
    MeshFilter filter = parent.GetComponent<MeshFilter>();
    if (filter != null && filter.sharedMesh != null)
    {
        meshFilters.Add(filter);
    }
    
    // Process all children recursively
    foreach (Transform child in parent)
    {
        CollectAllMeshFilters(child, meshFilters);
    }
}

private bool IsLikelyGround(Mesh mesh)
{
    // Simple heuristic: If most faces are oriented upward, it's likely ground
    Vector3[] normals = mesh.normals;
    int upwardFacingCount = 0;
    
    foreach (Vector3 normal in normals)
    {
        if (normal.y > 0.8f)
        {
            upwardFacingCount++;
        }
    }
    
    return (float)upwardFacingCount / normals.Length > 0.6f;
}
```

### Performance Optimization

```csharp
public void OptimizeTileset(PerformancePreset preset)
{
    switch (preset)
    {
        case PerformancePreset.Low:
            TileSet.maximumScreenSpaceError = 32;
            TileSet.preloadAncestors = false;
            TileSet.preloadSiblings = false;
            TileSet.forbidHoles = false;
            TilesetLoadDistance = 1000;
            break;
            
        case PerformancePreset.Medium:
            TileSet.maximumScreenSpaceError = 16;
            TileSet.preloadAncestors = true;
            TileSet.preloadSiblings = false;
            TileSet.forbidHoles = true;
            TilesetLoadDistance = 2000;
            break;
            
        case PerformancePreset.High:
            TileSet.maximumScreenSpaceError = 8;
            TileSet.preloadAncestors = true;
            TileSet.preloadSiblings = true;
            TileSet.forbidHoles = true;
            TilesetLoadDistance = 3000;
            break;
    }
    
    // Apply distance settings
    CesiumTilesetPlacer placer = TileSet.GetComponent<CesiumTilesetPlacer>();
    if (placer != null)
    {
        placer.distanceToCamera = TilesetLoadDistance;
    }
    
    // Apply overlay settings
    if (CesiumOverlay != null)
    {
        CesiumOverlay.maximumTextureSize = preset == PerformancePreset.Low ? 1024 : 2048;
        CesiumOverlay.maximumScreenSpaceError = preset == PerformancePreset.High ? 1.0f : 2.0f;
    }
}
```

### Level of Detail (LOD) System

```csharp
public void EnableLODSystem()
{
    // Find all renderers in the tileset
    Renderer[] renderers = TileSet.GetComponentsInChildren<Renderer>();
    
    foreach (Renderer renderer in renderers)
    {
        // Add LOD group if not present
        LODGroup lodGroup = renderer.gameObject.GetComponent<LODGroup>();
        if (lodGroup == null)
        {
            lodGroup = renderer.gameObject.AddComponent<LODGroup>();
            
            // Create LOD levels
            LOD[] lods = new LOD[3];
            
            // High detail
            lods[0] = new LOD(0.6f, new Renderer[] { renderer });
            
            // Medium detail - create simplified mesh
            Mesh originalMesh = renderer.GetComponent<MeshFilter>().sharedMesh;
            Mesh simplifiedMesh = SimplifyMesh(originalMesh, 0.5f);
            
            GameObject mediumLOD = CreateLODObject(renderer.gameObject, simplifiedMesh, "MediumLOD");
            lods[1] = new LOD(0.3f, new Renderer[] { mediumLOD.GetComponent<Renderer>() });
            
            // Low detail
            Mesh lowMesh = SimplifyMesh(originalMesh, 0.2f);
            GameObject lowLOD = CreateLODObject(renderer.gameObject, lowMesh, "LowLOD");
            lods[2] = new LOD(0.1f, new Renderer[] { lowLOD.GetComponent<Renderer>() });
            
            // Set LODs
            lodGroup.SetLODs(lods);
            lodGroup.RecalculateBounds();
        }
    }
}

private GameObject CreateLODObject(GameObject original, Mesh mesh, string suffix)
{
    // Create simplified LOD object
    GameObject lodObject = new GameObject(original.name + "_" + suffix);
    lodObject.transform.SetParent(original.transform.parent);
    lodObject.transform.localPosition = original.transform.localPosition;
    lodObject.transform.localRotation = original.transform.localRotation;
    lodObject.transform.localScale = original.transform.localScale;
    
    // Add mesh components
    MeshFilter meshFilter = lodObject.AddComponent<MeshFilter>();
    meshFilter.sharedMesh = mesh;
    
    // Copy renderer properties
    MeshRenderer originalRenderer = original.GetComponent<MeshRenderer>();
    MeshRenderer newRenderer = lodObject.AddComponent<MeshRenderer>();
    newRenderer.sharedMaterials = originalRenderer.sharedMaterials;
    
    return lodObject;
}
```

### Map Unloading

```csharp
public void UnloadMap()
{
    if (!IsLoaded) return;
    
    // Dispose of game elements
    PowerUps.Clear();
    Obstacles.Clear();
    Checkpoints.Clear();
    
    // Destroy tileset
    if (TileSet != null)
    {
        Destroy(TileSet.gameObject);
        TileSet = null;
    }
    
    // Destroy sub-scene
    if (NeighborhoodSubScene != null)
    {
        Destroy(NeighborhoodSubScene.gameObject);
        NeighborhoodSubScene = null;
    }
    
    // Reset state
    IsLoaded = false;
    
    // Trigger event
    OnMapUnloaded?.Invoke(this);
}
```

## Usage Example

```csharp
// Example usage in GameManager
public class GameManager : MonoBehaviour
{
    public Map3D neighborhoodMap;
    public string googleMapsApiKey = "YOUR_GOOGLE_MAPS_API_KEY";
    
    // Coordinates for the neighborhood center
    public double latitude = 37.7749;
    public double longitude = -122.4194;
    public double height = 0.0;
    public double radius = 500.0; // 500 meters radius
    
    async void StartGame()
    {
        Map3DSettings settings = new Map3DSettings
        {
            MapName = "My Neighborhood",
            MapScale = 1.0f,
            PerformancePreset = PerformancePreset.Medium,
            StyleSettings = MapStyle.MarioKart
        };
        
        // Initialize map with coordinates
        neighborhoodMap.Initialize(latitude, longitude, radius, settings);
        
        // Set up the Cesium 3D tileset with Google Maps
        neighborhoodMap.Create3DTileset(googleMapsApiKey);
        
        // Create neighborhood sub-scene
        neighborhoodMap.CreateNeighborhoodSubScene("RaceTrack", latitude, longitude, height, radius);
        
        // Load the map
        await neighborhoodMap.LoadMapAsync(true);
        
        // Set up collisions
        neighborhoodMap.SetupCollisionForTileset();
        
        // Optimize performance
        neighborhoodMap.OptimizeTileset(PerformancePreset.Medium);
        
        // Place game elements
        neighborhoodMap.PlacePowerUps(PowerUpPlacementStrategy.Even);
        neighborhoodMap.PlaceObstacles(ObstaclePlacementStrategy.Medium);
        neighborhoodMap.GenerateCheckpoints(12);
        neighborhoodMap.SetupStartingGrid(4);
        
        // Start race
        RaceController.StartCountdown();
    }
    
    // Handle cleanup
    void OnDestroy()
    {
        if (neighborhoodMap != null && neighborhoodMap.IsLoaded)
        {
            neighborhoodMap.UnloadMap();
        }
    }
}
```

## Performance Considerations

- Screen space error settings need careful balancing for performance vs. quality
- Async tileset loading to prevent frame drops during initial load
- Level of detail (LOD) system for distant buildings
- Simplified collision meshes for complex building structures
- Object pooling for power-ups and obstacles
- Adaptive quality settings based on device capabilities

## Future Enhancements

- Procedural track path generation along real roads
- Custom shader for Mario Kart-style visual enhancement
- Automatic placement of game elements based on real-world POIs
- Dynamic time-of-day system
- Weather effects
- Traffic system using real-world traffic patterns
- Custom material overrides for tileset objects
