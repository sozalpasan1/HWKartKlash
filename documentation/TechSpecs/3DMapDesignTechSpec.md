# FROM THIS VIDEO https://www.youtube.com/watch?v=lLw5hCqSv5Y&t=1s

## THIS IS DIRECTLY 3D

## 1. Overview

This document outlines the technical approach for implementing Google Maps 3D Tiles into a Unity-based go-kart racing game set in a real-world neighborhood. This approach allows for photorealistic 3D map rendering using the Cesium for Unity integration with Google Maps Platform.

## 2. Prerequisites

| Requirement | Details |
|-------------|---------|
| Unity | Latest version with Universal Render Pipeline (URP) |
| Google Maps Platform | Active account with Map Tiles API enabled |
| Cesium ION | Active account for 3D tileset services |
| Unity Knowledge | Basic understanding of Unity development |

## 3. Implementation Process

### 3.1 Unity Project Setup

1. Create a new 3D Unity project with Universal Render Pipeline
2. Install the Cesium package:
   - Open Package Manager (Edit > Project Settings > Package Manager)
   - Add scope registry:
     - Name: `Cesium`
     - URL: `https://unity.pkg.cesium.com`
     - Scope: `com.cesium.unity`
   - Install "Cesium for Unity" from the My Registries tab
   - Restart Unity

### 3.2 API Configuration

1. **Google Maps Setup**:
   - Create project in Google Maps Platform
   - Enable Map Tiles API
   - Generate and copy API key
2. **Cesium Configuration**:
   - Open Cesium window (Window > Cesium)
   - Log in to Cesium ION account
   - Generate access token

### 3.3 3D Tileset Creation

1. In the Hierarchy panel, create a blank 3D TileSet
2. Click "+" to add necessary game objects
3. Configure the 3D TileSet:
   - Check "Show Credits" (required for Google's terms of service)
   - In "From URL" field enter:
     ```
     https://tile.googleapis.com/v1/3dtiles/root.json?key=YOUR_API_KEY
     ```
     (Replace YOUR_API_KEY with actual Google Maps API key)

### 3.4 Neighborhood Sub-Scene Implementation

**Purpose**: Create independently renderable map sections to prevent physics drift

1. Find coordinates of neighborhood center in Google Maps
2. In Cesium window:
   - Enter latitude/longitude/height of neighborhood
   - Click "Create Sub-scene"
   - Name the sub-scene (e.g., "NeighborhoodTrack")
   - Set radius to encompass desired race area
3. Place all go-kart assets within sub-scene's global transformation object

### 3.5 Go-Kart Integration

1. Import 3D go-kart assets (from Asset Store or custom models)
2. Position prefabs within the neighborhood sub-scene
3. Configure player character camera to follow the kart

### 3.6 Collision Management

**Challenge**: Initial map loading may cause physics issues

**Solutions**:
- Elevate player starting position above ground level
- Implement loading screen until tiles are fully loaded
- Increase screen space error value to improve loading performance

## 4. Race Track Design Considerations

| Component | Implementation Approach |
|-----------|-------------------------|
| Track Boundaries | Utilize 3D tile data for outer edges |
| Road Surface | Create custom road mesh overlaying 3D tiles |
| Obstacles | Add custom colliders to existing building meshes |
| Decorations | Enhance visual appeal with additional game objects |

## 5. Performance Optimization

1. **Texture Resolution Management**:
   - Balance visual fidelity with performance
   - Implement multiple LOD levels

2. **Rendering Optimization**:
   - Use occlusion culling
   - Implement distance-based object activation

3. **Data Extraction**:
   - Extract building geometries for simplified collision meshes
   - Create low-poly alternatives for distant objects

## 6. Implementation Timeline

| Phase | Duration | Key Deliverables |
|-------|----------|------------------|
| Setup | 1 week | Project creation, package installation, API configuration |
| Map Integration | 1 week | Functioning 3D tileset of neighborhood |
| Sub-scene Creation | 1 week | Optimized neighborhood map section |
| Go-kart Implementation | 2 weeks | Functioning kart with controls and physics |
| Optimization | 1 week | Performance tuning and bug fixing |

## 7. Technical Constraints

- Map size limited by Cesium tileset loading performance
- Initial loading time proportional to map complexity
- Collision precision limited by Google's 3D tile accuracy
- Usage subject to Google Maps Platform pricing and quotas

## 8. References

- [Google Maps Platform Documentation](https://developers.google.com/maps/documentation)
- [Cesium for Unity Documentation](https://cesium.com/docs/cesiumjs-ref-doc/)
- [Unity Universal Render Pipeline](https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@latest)