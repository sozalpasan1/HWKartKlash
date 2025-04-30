# Kart Racing Game - Physics and Controls

## Overview
This document outlines the core physics and control systems for our kart racing game.

[copy this lmao](https://www.youtube.com/watch?v=cqATTzJmFDY)

## Kart Physics

### State Variables
The kart will maintain basic properties like position, rotation, and velocity vectors to track its state in the game world. There'll also be variables such as kart maximum speed, weight, etc that can be altered by power ups.

### Movement System
Will handle acceleration and deceleration based on player input. Implements basic physics concepts like friction and drag to create responsive yet fun arcade-style handling.

### Steering System
Controls how the kart turns and handles. Will include drift mechanics that balance realism with gameplay fun.

### Collision System
Utilizes Unity's built-in physics but with custom parameters to collisions that aren't frustrating.

## Control System

### Input Handling
Takes in user input directly to the kart physics to change how the kart will be controlled. These inputs will also be accesible as variables for later incase we add logic to make the wheels turn with the kart turning

### Physics Update
Main loop that processes inputs, calculates physics changes, and updates the kart state each frame. (most of this is handled behind the scenes by Unity, we just need to declare an update method)

## Power-up System

### Implementation
There will be an interface for powerups to temporarily modify kart variables such as max speed, acceleration, or handling characteristics.