# Kart Physics People

- **Dylan**: Acceleration/traction  
- **Aviv**: Finished drifting script, working on fixed camera position  
- **Jacob and Milo**: Working on kart and kart physics  

# Map People

- **Sean**: Moving file into Blender  
- **Olivia**: Adding checkpoints and fence along map  
- **Matthew**: Spawn location  

# Next Steps

- Work on map we have right now  
- Work on Blender map  

# Multiplayer

- Nothing for now  

# UI

## Account/Login

### Start

- **Host** → Private/Public → Start Game (list of players)  
  **OR**  
- **Join** → Enter Room Codes → Waiting for Host to Start Game  

## Game

### Endgame

- If play again, loop back to **Start**

```mermaid
flowchart TD
    A[Start] --> B{Choose Option}
    B -->|Host| C[Select Private/Public]
    C --> D[List of Players]
    D --> E[Start Game]
    B -->|Join| F[Enter Room Code]
    F --> G[Waiting for Host to Start Game]
    G --> E
    E --> H[Game Running]
    H --> I[Endgame]
    I --> J{Play Again?}
    J -->|Yes| A
    J -->|No| K[Exit]