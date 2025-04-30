## Feature 1: Student Obstacles

### StudentManager

* **Variables Expected**:
  * `studentSpawnPoints`: Array of classroom doors where students can emerge
  * `studentDespawnPoints`: Array of classroom doors where students can enter
  * `maxActiveStudents`: Maximum number of students active at once (global limit)
  * `spawnInterval`: Time between student spawn waves (seconds)
  * `spawnIntervalVariance`: Random variance added to spawn timing (±seconds)
  * `studentCountPerWave`: How many students spawn in each wave
  * `currentActiveStudents`: List tracking all active student objects
  * Lower Priority:
  * `ragdollForce`: Physics force applied when hit by a kart
  * `ragdollDuration`: How long students remain in ragdoll state (seconds)
  * `despawnTime`: Time before ragdolled students despawn (seconds)
  * `respawnCooldown`: Time before a despawned student can be respawned
  * `spawnSchedule`: Predetermined spawn times for race events
  * `currentActiveStudents`: List tracking all active student objects
  * `walkAnimationSpeed`: Movement animation speed multiplier
  * `walkPathVariance`: How much students can deviate from their path
  * `isNetworkSynchronized`: Whether student spawning is synced across network
  * `studentPrefabs`: Array of different student model prefabs for visual variety

* **Methods Expected**:
  * `InitializeStudentSystem()`: Setup obstacle system parameters
  * `SpawnStudentWave()`: Create a wave of students from selected doors
  * `DespawnStudent()`: Remove a student from the scene
  * `CalculateSpawnPoints()`: Select appropriate classroom doors for spawning
  * `GenerateWalkPath()`: Create a path from spawn door to destination door
  * `HandleStudentCollision()`: Process kart collision with student
  * Lower Priority:
  * `TriggerRagdollPhysics()`: Switch student to physics-based ragdoll
  * `SyncStudentPositions()`: Keep student positions synchronized in multiplayer
  * `ScheduleSpawnEvents()`: Create timed spawn events for the race
  * `UpdateStudentPaths()`: Modify student paths if needed (obstacle avoidance)
  * `GetCurrentStudentDensity()`: Calculate current student density in an area


### Student

* **Variables Expected**:
  * `walkSpeed`: Base movement speed
  * `walkSpeedVariance`: Random variation in movement speed
  * `currentPath`: Waypoints from spawn point to destination
  * `currentPathIndex`: Current position in the path
  * `collisionLayer`: Physics layer for collision detection
  * `collisionRadius`: Size of collision detection sphere
  * Lower Priority:
  * `animator`: Reference to animation controller
  * `ragdollComponents`: References to ragdoll physics components
  * `ragdollActiveState`: Whether currently in ragdoll mode
  * `bloodEffectPrefab`: Visual effect for collision
  * `hitSoundEffects`: Array of sound effects for collision
  * `healthState`: Current state (normal, injured, ragdoll)
  * `recoveryTime`: Time after which a ragdolled student gets up
  * `canRecover`: Whether this student can recover from ragdoll
  * `studentID`: Unique identifier for tracking

* **Methods Expected**:
  * `Initialize()`: Setup student parameters and path
  * `FollowPath()`: Move student along assigned path
  * `OnCollisionEnter()`: Detect and process collisions
  * `CalculateImpactForce()`: Determine force direction and magnitude
  * `CheckRecovery()`: Determine if/when student should recover
  * `EnterClassroom()`: Animation and logic for entering destination
  * `GetDistanceToKart()`: Calculate distance to nearest player
  * `OnNetworkUpdate()`: Process network synchronization data
  * Lower Priority: 
  * `ActivateRagdoll()`: Enable physics-based ragdoll
  * `DeactivateRagdoll()`: Disable ragdoll, return to animation
  * `PlayHitAnimation()`: Trigger appropriate collision animation
  * `PlayHitSound()`: Play random sound effect from array
  * `SpawnBloodEffect()`: Instantiate blood particle effect

## Feature 2: Backpack Obstacles

### BackpackManager

* **Variables Expected**:
  * `backpackSpawnPoints`: Predefined positions for static backpacks
  * `dynamicBackpackCount`: Number of randomly placed backpacks
  * `respawnTime`: Time before picked-up backpacks respawn
  * `backpackDensity`: Number of backpacks per track section
  * `backpackCollisionRadius`: Size of collision detection
  * `speedReductionFactor`: How much backpacks slow karts
  * `speedReductionDuration`: How long slowdown effect lasts
  * Lower Priority:
  * `collisionSoundEffects`: Sounds played when driving over backpack
  * `currentBackpacks`: List of all active backpack objects
  * `backpackPrefabs`: Array of different backpack model prefabs

* **Methods Expected**:
  * `InitializeBackpacks()`: Setup backpack obstacle system
  * `SpawnStaticBackpacks()`: Place backpacks at predefined points
  * `HandleBackpackCollision()`: Process kart collision with backpack
  * `ApplySpeedReduction()`: Reduce kart speed when hitting backpack
  * `ShuffleBackpacks()`: Periodically change backpack positions
  * `SyncBackpackState()`: Synchronize backpack state in multiplayer
  * Lower Priority:
  * `PlayCollisionEffect()`: Visual and audio feedback for collision

### Backpack

* **Variables Expected**:
  * `backpackType`: Visual style and collision properties
  * `originalPosition`: Starting position for respawn
  * `originalRotation`: Starting rotation for respawn
  * `colliderComponent`: Reference to collision detection component
  * `networkID`: Unique identifier for synchronization
  * Lower Priority:
  * `soundEffect`: Sound played when collided with
  * `visualEffectPrefab`: Effect shown when collided with

* **Methods Expected**:
  * `Initialize()`: Setup backpack parameters
  * `OnCollisionEnter()`: Detect collision with kart
  * `ApplyCollisionEffect()`: Apply speed penalty to kart
  * `UpdateNetworkState()`: Sync current state across network
  * Lower Priotity:
  * `TriggerVisualEffect()`: Show collision visual effect
  * `PlayCollisionSound()`: Play appropriate sound effect



## Implementation Phases

### Phase 1: Basic Obstacle Implementation
* Static backpack placement
* Simple student pathing
* Basic collision detection
* Fundamental speed penalties

### Phase 2: Physics Enhancement
* Ragdoll physics for students
* Collision response refinement
* Visual and audio feedback improvements
* Physics optimization

### Phase 3: Behavior Sophistication
* Advanced student pathing
* Classroom exit/entry animations

### Phase 4: Multiplayer Integration
* Network synchronization
* Bandwidth optimization
* Client-side prediction
* Authority model implementation
