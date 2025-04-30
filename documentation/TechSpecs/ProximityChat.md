# Proximity Chat System - Tech Spec

## Overview
This document outlines the proximity voice chat system for our kart racing game.

## Audio Input

### Microphone Capture
System will access the player's microphone to capture voice input. Will include basic options for microphone selection and input volume adjustment.

## Networking

### Audio
Voice data will be compressed and packaged for efficient network transmission. The system will integrate with our multiplayer framework (implementation details pending). This will then be returned alongside player positions

## Audio Output

### Distance Calculation
Client-side processing will calculate the distance between the local player and each voice source.

### Volume Modulation
The system will dynamically adjust voice volume based on:
- Distance between players (inverse square falloff)
- (LATE DEVELOPMENT FEATURE) directional audio based on relative positions

### Audio Mixing
All incoming voice streams will be mixed into the game's audio output with appropriate volume levels.

### Customization
Basic user options will include master voice chat volume and proximity distance thresholds.