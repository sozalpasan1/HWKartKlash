### Initialization
| Method | Returns | Notes |
|--------|---------|-------|
| `load3D()` | `void` | Loads the 3-D map file **and** every feature list, wiring them into the live scene graph. |
| `unload()` | `void` | Gracefully unloads the current map, freeing meshes, textures, and feature tables. |

---

### MapFeatures
<sub>`checkpointList : Position[]` • `powerupList : PowerUp[]` • `finishPosition : Position` • `startPosition : Position` • `killBoundary : double[8]`</sub>

> *Data-only container for P1; helper getters will land in P2.*

---

### RespawnService
| Method | Returns | Notes |
|--------|---------|-------|
| `Respawn()` | `void` | Teleports the player to the nearest respawn location. |
| `determineRespawnPosition()` | `Position` | Computes the point linked to the most recent checkpoint passed. |

<sub>`fruitLocation : (int,int,int)` • `layoutType : static enum { pellet, wall, clear, powerup }`</sub>

---

### PowerUp
| Method | Returns | Notes |
|--------|---------|-------|
| `GetLocation()` | `Position` | Exposes world-space coordinates for spawn or collision checks. |

<sub>`location : Position`</sub>

---

### CheckPoint
| Method | Returns | Notes |
|--------|---------|-------|
| `hasBeenActivated(playerID)` | `bool` | Indicates whether this player has already triggered the checkpoint during the current lap. |

<sub>`position : Position` • `respawnLocation : Position` • `checkpointNumber : int` • `playersPassed : int[]`</sub>

---

### Supporting struct
| Struct | Fields |
|--------|--------|
| `Position` | `double x`, `double y` |

---