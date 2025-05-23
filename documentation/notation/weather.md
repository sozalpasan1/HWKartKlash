# Weather System 

Each map has a fixed weather condition that affects kart physics through multipliers applied to existing vehicle stats. These multipliers are applied at race start and modify parameters like traction, visibility, speed, and health decay. All vehicle types share the same base system but may apply weather differently via modifiers.

## Core Classes

### WeatherManager
- **Variables:**
  - `currentWeather: {Rain, Snow, Fog, Hail, Heat}`
  - `activeMultipliers: HashMap<String, double>` - this hashmap stores the multipliers currently active
- **Methods:**
  - `InitWeather(mapID: String)`
  - `ApplyWeatherTo(vehicle: Vehicle)` - depending on the current weather, applies multipliers to the vehicle of the user
  - `ResetWeatherEffects(vehicle: Vehicle)`

### WeatherProfile
Each weather condition has an associated `WeatherProfile` storing multipliers for physics variables.

## Process Flow
1. **Map Load:** `WeatherManager.InitWeather(mapID)` sets the fixed weather
2. **Weather Init:** Correct `WeatherEffect` is instantiated
3. **Pre-Race:** `WeatherManager.ApplyEffects()` updates vehicles
4. **Race Start:** Vehicles operate under altered physics
5. **Race Ongoing:** Vehicles continue to update physics based on modified stats
6. **Race End:** Effects stop, vehicles reset to defaults which means offloading of weather

#### Example (Rain):
```json
{
  "tractionMultiplier": 0.6,
  "visionMultiplier": 0.80,
  "speedMultiplier": 1.10,
  "healthDrainPerSecond": 1.0
}

