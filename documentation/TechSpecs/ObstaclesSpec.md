# HW KartKlash - ObstacleSpec.md

## Overview

This document outlines the technical specifications for implementing dynamic obstacle systems in the HW KartKlash racing game. The focus is on creating believable, interactive obstacles that enhance gameplay by providing challenges while maintaining the fun, arcade-style racing experience.

## Feature 1: Student Obstacles

**Related Design Elements**: 
> "Students are roaming in certain areas on the track. The 1-3 students are placed randomly on the track walking back and forth in a line at a steady pace. If you hit them, your car will lose a lot of speed, they will turn into ragdolls and bleed, and Mr. Preciado will fly down on a cloud and scold you."

### StudentManager

* **Variables Expected**:
  * `studentPrefabs`: Array of different student model prefabs for visual variety
  * `studentSpawnPoints`: Array of classroom doors where students can emerge
  * `studentDespawnPoints`: Array of classroom doors where students can enter
  * `maxActiveStudents`: Maximum number of students active at once (global limit)
  * `spawnInterval`: Time between student spawn waves (seconds)
  * `spawnIntervalVariance`: Random variance added to spawn timing (±seconds)
  * `studentCountPerWave`: How many students spawn in each wave
  * `ragdollForce`: Physics force applied when hit by a kart
  * `ragdollDuration`: How long students remain in ragdoll state (seconds)
  * `despawnTime`: Time before ragdolled students despawn (seconds)
  * `respawnCooldown`: Time before a despawned student can be respawned
  * `spawnSchedule`: Predetermined spawn times for race events
  * `currentActiveStudents`: List tracking all active student objects
  * `walkAnimationSpeed`: Movement animation speed multiplier
  * `walkPathVariance`: How much students can deviate from their path
  * `isNetworkSynchronized`: Whether student spawning is synced across network

* **Methods Expected**:
  * `InitializeStudentSystem()`: Setup obstacle system parameters
  * `SpawnStudentWave()`: Create a wave of students from selected doors
  * `DespawnStudent()`: Remove a student from the scene
  * `CalculateSpawnPoints()`: Select appropriate classroom doors for spawning
  * `GenerateWalkPath()`: Create a path from spawn door to destination door
  * `HandleStudentCollision()`: Process kart collision with student
  * `TriggerRagdollPhysics()`: Switch student to physics-based ragdoll
  * `SyncStudentPositions()`: Keep student positions synchronized in multiplayer
  * `ScheduleSpawnEvents()`: Create timed spawn events for the race
  * `UpdateStudentPaths()`: Modify student paths if needed (obstacle avoidance)
  * `GetCurrentStudentDensity()`: Calculate current student density in an area

* **Related Objects**:
  * Student: Individual student behavior and physics
  * KartController: For interaction with player karts
  * RaceManager: For syncing with race progression
  * PreciadoManager: For triggering the Preciado scold effect
  * NetworkManager: For multiplayer synchronization
  * AnimationManager: For student walk/ragdoll animations

### Student

* **Variables Expected**:
  * `walkSpeed`: Base movement speed
  * `walkSpeedVariance`: Random variation in movement speed
  * `currentPath`: Waypoints from spawn point to destination
  * `currentPathIndex`: Current position in the path
  * `animator`: Reference to animation controller
  * `ragdollComponents`: References to ragdoll physics components
  * `collisionLayer`: Physics layer for collision detection
  * `collisionRadius`: Size of collision detection sphere
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
  * `ActivateRagdoll()`: Enable physics-based ragdoll
  * `DeactivateRagdoll()`: Disable ragdoll, return to animation
  * `PlayHitAnimation()`: Trigger appropriate collision animation
  * `PlayHitSound()`: Play random sound effect from array
  * `SpawnBloodEffect()`: Instantiate blood particle effect
  * `OnCollisionEnter()`: Detect and process collisions
  * `CalculateImpactForce()`: Determine force direction and magnitude
  * `CheckRecovery()`: Determine if/when student should recover
  * `EnterClassroom()`: Animation and logic for entering destination
  * `GetDistanceToKart()`: Calculate distance to nearest player
  * `OnNetworkUpdate()`: Process network synchronization data

## Feature 2: Backpack Obstacles

**Related Design Elements**: 
> "Other obstacles - backpacks, tables cause you to lose speed to a lesser extent."

### BackpackManager

* **Variables Expected**:
  * `backpackPrefabs`: Array of different backpack model prefabs
  * `backpackSpawnPoints`: Predefined positions for static backpacks
  * `dynamicBackpackCount`: Number of randomly placed backpacks
  * `respawnTime`: Time before picked-up backpacks respawn
  * `backpackDensity`: Number of backpacks per track section
  * `backpackCollisionRadius`: Size of collision detection
  * `speedReductionFactor`: How much backpacks slow karts
  * `speedReductionDuration`: How long slowdown effect lasts
  * `collisionSoundEffects`: Sounds played when driving over backpack
  * `currentBackpacks`: List of all active backpack objects

* **Methods Expected**:
  * `InitializeBackpacks()`: Setup backpack obstacle system
  * `SpawnStaticBackpacks()`: Place backpacks at predefined points
  * `HandleBackpackCollision()`: Process kart collision with backpack
  * `ApplySpeedReduction()`: Reduce kart speed when hitting backpack
  * `ShuffleBackpacks()`: Periodically change backpack positions
  * `SyncBackpackState()`: Synchronize backpack state in multiplayer
  * `PlayCollisionEffect()`: Visual and audio feedback for collision

### Backpack

* **Variables Expected**:
  * `backpackType`: Visual style and collision properties
  * `originalPosition`: Starting position for respawn
  * `originalRotation`: Starting rotation for respawn
  * `colliderComponent`: Reference to collision detection component
  * `soundEffect`: Sound played when collided with
  * `visualEffectPrefab`: Effect shown when collided with
  * `networkID`: Unique identifier for synchronization

* **Methods Expected**:
  * `Initialize()`: Setup backpack parameters
  * `OnCollisionEnter()`: Detect collision with kart
  * `ApplyCollisionEffect()`: Apply speed penalty to kart
  * `TriggerVisualEffect()`: Show collision visual effect
  * `PlayCollisionSound()`: Play appropriate sound effect
  * `UpdateNetworkState()`: Sync current state across network



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
