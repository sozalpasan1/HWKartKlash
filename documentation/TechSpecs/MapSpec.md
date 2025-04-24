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


**Lap Tracking (Finish Line) (P1)**
IncrementLap(player): Increases the player's lap count upon completing a lap.
HasFinishedRace(player): Determines if the player has completed the required number of laps.

**Checkpoints (P1)**
ActivateCheckpoint(): Marks the checkpoint as active when a player passes through.
IsNextCheckpoint(player): Validates if this checkpoint is the next in sequence for the player.

**Respawn/Relocation (P1)**
- Respawn Trigger / Fall Detection (P1)
- Respawn Behavior (P1)
TriggerRespawn(player): Initiates the respawn process for the player.
GetLastCheckpoint(player): Retrieves the last checkpoint the player passed for accurate respawn positioning.

**Collision (P1)**
OnCollisionEnter(collider): Responds to collision events.
IsOffTrack(collider): Determines if the collision indicates the player is off the track.

**Track (P1)**
- Borders/Barriers (P1)
- Shortcuts (P2)
- Off-Road Areas (P2)
- Road Textures (P2)
- Boost Pads / Ramps (P2)
InitializeTrack(): Sets up the track layout and components.
GetCheckpointList(): Retrieves the list of checkpoints in order.

**Player Position Tracking (P1)**
UpdatePositions(): Recalculates player positions based on progress.
GetPlayerPosition(player): Retrieves the current position of a specific player.