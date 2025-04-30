# Currency, Stats, Standings, and Account System

We will use Firebase Authentication and Realtime Database for user accounts and persistent data. When a player logs in or signs up with email/password (via a `PlayerAccount.Login()` method), FirebaseAuth handles the credentials. 

Upon successful login, we use a `FirebaseService` class to load the player’s data from the database (e.g. under `/users/{username}/`). The Realtime Database stores all player info in JSON trees and syncs data across clients in real-time. For example, each user record can have fields like `currency` or `coinBalance` to track virtual currency.

## Core Classes

### PlayerAccount
- **Variables:** `username: String`, `currencyManager: CurrencyManager`, `statsManager: StatsManager`
- **Key Point** Each player account will have instances of the currency manager and stats manager classes, which will store those variables.
- **Methods:** `Login()`, `Logout()`, `LoadProfile()`
- Interacts with `FirebaseService` to load/save data.

### CurrencyManager
- **Variables:** `balance: int`
- **Methods:** `AddCurrency(amount: int)`, `SpendCurrency(cost: int)`, `GetBalance(): int`
- Updates Firebase using `FirebaseService.SaveData("/users/{id}/currency", newBalance)`

### StatsManager
- **Variables:** `lapTimes: List<float>`, `bestLap: float`, `averageTime: float`, and many more!
- **Methods:** `SaveStats()`, `LoadStats()`, `UpdateLap(mapID: String, time: float)`
- Stores lap data per map and computes averages.

### RaceManager
- **Variables:** `currentStandings: List<PlayerID>`, `placements: List<PlayerID>`
- **Methods:** `StartRace()`, `EndRace()`, `CalculateStandings()`, `AwardCurrency()`
- Updates stats and awards currency after each race.

### StandingsManager
- **Methods:** `UpdateStandings(newOrder: List<PlayerID>)`, `GetStandings()`
- Updates race standings in real-time using Firebase or networking.

### FirebaseService
- **Variables:** `auth: FirebaseAuth`, `database: FirebaseDatabase`
- **Methods:** `Login(email, pass)`, `Logout()`, `SaveData(path: String, data)`, `LoadData(path: String)`
- Centralized interface for all backend data actions.

## User Flow (Login → Race → Stats/Shop)
1. **Login:** Authenticates via Firebase → loads profile
2. **Main Menu:** Select race, view stats, or shop
3. **Join Race:** Initializes positions and starts real-time updates
4. **During Race:** Live standings updated and broadcast
5. **End Race:** Final positions saved, currency rewarded, stats updated
6. **Post-Race:** Player can shop or view updated stats
7. **Logout:** All data persisted in Firebase