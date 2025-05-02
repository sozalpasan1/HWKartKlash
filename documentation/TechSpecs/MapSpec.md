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

The race track follows a circuit around the Harvard-Westlake campus:
![Race Track](./HWKKMapDraft1.png)

- **Starting Line**: Begins at the stop light on Coldwater heading southbound; once the light changes the players turn left into the parking lot
- **First Section**: Leads into the parking lot with spikes and a challenging dip
- **Second Section**: Navigates through a narrow passage going towards the Hamilton parking lot and then through to the Senior Lot; there are cars blokcing the loading zone so you must go around then toward the security kiosk
- **Third Section**: Curves back up toward the security kiosk
- **Fourth Section**: Traverse up toward the steep fire road leading up to St. Saviour's Chapel
- **Fifth Section**: Behind the chapel is another steep descent (making an artificial ramp) into the teacher parking lot. Alternatively, there is a route up near the admissions buliding where there is a ramp that launches you up into the air and you land onto the exit of the faculty lot. 
- **Sixth Section**: Route continues toward Weiler Hall and to around the track with obstacles
- **Seventh Section**: Drives up the bleachers leading to lower quad, then to upper quad then to the cafeteria 
- **Eighth Section**: Exit the cafeteria and goes through the main pathway to and through Taper Gym
- **Final Section**: Exits Taper and goes up the handicap ramp and restarts the lap at the entry of the parking lot.

Track 2:
- **Starting Line**: Begins at the pool
- **First Section**: Leads into the track and loops around it
- **Second Section**: Exits near Taper Gym stairs
- **Third Section**: Passes through Munger up to the sophomore quad
- **Fourth Section**:  Heads down to the main quad, then right into the back entrance of Chalmers
- **Fifth Section**: Travels through Chalmers and down to the cafeteria
- **Sixth Section**: Goes down stairs to North entrance
- **Seventh Section**: Leaves out North entrance and re-enters campus main entrance by pool
- **Final Section**: Through Taper to the pool

Need map when you start the race and unload when the race is finished.

**Map Manager**
Methods:
LoadMap(): Loads the map and calls InitializeTrack() to set it up. (P1)
UnloadMap(): Unloads the map and clears any active data. (P1)

**Lap Tracker (Finish Line) (P1)**
Variables:
int requiredLaps: Number of laps required to finish the race. (P1)
Transform finishLine: Transform for the finish line. (P1)
Methods:
IncrementLap(player): Increases the player's lap count upon completing a lap. (P1)
HasFinishedRace(player): Determines if the player has completed the required number of laps. (P1)

**CheckpointManager (P1)**
Variables:
int currentCheckpoint: Index of the checkpoint in the sequence. (P1)
Transform respawnPoint: Where to respawn the player if needed. (P1)
Methods:
void ActivateCheckpoint(): Marks the checkpoint as active when a player passes through. (P1)
bool IsNextCheckpoint(Player player): Validates if this checkpoint is the next in sequence for the player. (P1)
void TriggerRespawn(Player player): Initiates the respawn process for the player. (P1)
Checkpoint GetLastCheckpoint(Player player): Retrieves the last checkpoint the player passed for accurate respawn positioning. (P1)

**Collision (P1)**
Variables:
LayerMask trackLayer: Layer mask for identifying track boundaries and off-track zones. (P1)
Methods:
void OnCollision(playerb): Responds to collision events. (P1)

**Track (P1)**
Variables:
List<Borders> borders: List of borders in the track. (P1)
List<Transform> boostPads: List of boost pads scattered across the track. (P2)
List<Transform> ramps: List of ramps scattered across the track. (P2)
List<Transform> shortcuts: List of shortcuts available on the track. (P2)
List<Transform> offRoadAreas: Areas where players can go off-road. (P2)
Material roadTexture: The material for the track's road surface. (P2)
Methods:
void InitializeTrack(): Sets up the track layout and components. (P1)
