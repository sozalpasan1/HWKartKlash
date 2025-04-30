# HW KartKlash Movement, Physics, and Controls - Technical Specification

## Player Movement System

### Movement States
1. **Grounded State**: Normal driving mechanics when all wheels are in contact with the ground
2. **Airborne State**: Physics-based trajectory calculations when kart is in the air (from jumps/ramps)
4. **Recovery State**: Auto-stabilization when kart has flipped or collided with obstacles

### Speed Management
- **Maximum Speed**: Depends on which car player is using. Between ~80mph and ~100mph
- **Rate of Acceleration**: Also depends on which car player is using
  - Should be somewhere between 20mph per second and 40mph per second
- **Deceleration**: Natural deceleration at all times (loses ~5% of current velocity per second)
    - Different karts can be more or less aerodynamic
- **Braking Deceleration**: Rapid deceleration when braking (-20mph per second)

### Turning Mechanics
- **Base Turn Speed**: ~45 degrees per second. But depends on handling
- **Focus Power**: However, as with all movements, this is increased by x1.5 if you're using the focus power
- **Turn Responsiveness**: 0.2 second delay between input and maximum turn angle
- **Stability Control**: Auto-adjusts kart stability when turning at high speeds

### Advanced Movement Features
- **Jump Mechanics**: Height/distance calculations based on speed and ramp angle
- **Focused Power Mechanism**:
  - If the T key and only one other key are being pressed, the effect of the other key happens 1.5 times more powerfully.
  - For example, if only W and T are being pressed, then the kart accelerates 1.5 times faster. If only D and T are being pressed, it turns 1.5 times more powerfully.
  - This adds an extra layer to gameplay, and another thing for the player to focus on as they play

## Kart Physics System

### Physics Model Type
- **Simplified Arcade Physics**: Modified realistic physics model with focus on fun over simulation
- **Physics Update Rate**: 60 physics updates per second for consistent behavior across different hardware

### Surface Interaction
- **Surface Materials Database**:
  - Stone: Base speed (1.0)
  - Grass: Reduced speed (0.7)
  - Metal: Reduced speed (0.9)
- **Weather Effects on Physics**:
  - Rain: Reduced handling due to wet ground (-15%)
  - Heat: Increased handling due to better tire grip (+10%)
  - Snow: Speed reduction (-20%)

### Collision Response System
- **Kart Collisions**:
  - Varied response based on collision velocity and angle (general karts physics)
  - Speed reduction on impact (based on collision angle)
  - Recovery assistance system to help flipped karts

### Advanced Physics Features
- **Aerodynamics System**:
  - Different karts have different levels of aerodynamics
- **Damage Model** (for Hard Mode):
  - Health points (HP) system for vehicle integrity
  - Damage affects handling gradually
- **Obstacle Physics**:
  - Student NPCs: Severe speed loss on collision
  - Small obstacles (backpacks, small items): Minor speed loss
  - Powerup boxes: No speed loss

## User Controls

### Core Control Scheme

#### Keyboard Default Controls
- **W**: Accelerate
- **S**: Brake/Reverse
- **A/D**: Turn Left/
- **J**: Jump
- **Shift**: Use Collected Powerup
- **E**: Activate Teacher Special Ability
- **F**: Fire powerup or special ability (after activation)
- **P**: Activate powerup
- **T**: Focused power key
- **V**: Push to talk in voice chat
- **M**: Mutes voice chat in multiplayer
- **Tab**: View Minimap with players as dots
- **Esc**: Pause Menu
### Visual Control Feedback
- **Speed Display**: Speedometer in corner showing current speed
- **Teacher Ability Cooldown**: Visual timer for special ability availability

### Special Controls

#### Teacher Special Abilities
- **Activation**: Press shift when ability is available
- **Cooldown System**: Each ability has a specific cooldown period before reuse
- **Targeting Controls** (for powerups/abilities that require aiming):
  - Aim will sweep around in a circle
  - Press f to fire the ability in the current aim direction

### User Interface Controls
- **Menu Navigation**: Arrow keys/WASD
- **Selection**: Enter
- **Back/Cancel**: Esc

## Relation to other core components

### Weather
- Weather affects friction, speed, and in hard mode, affects damage
- As described above

### Varying kart options
- Affects top speed, handling, aerodynamics, acceleration
- As described above

### Minimap
- Press tab to see minimap with each player as a dot on it

### Multiplayer
- Voice chat is available at all times based upon pressing V, regardless of distance to other cars