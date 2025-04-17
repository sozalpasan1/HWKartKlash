# HW KartKlash

## Game Concept

A multiplayer racing game set on the Harvard-Westlake campus. Players race go-karts around a track that follows the actual layout of the school grounds, featuring iconic locations and challenging terrain.

## Track Layout Example

The race track follows a circuit around the Harvard-Westlake campus:
(./HW KK map draft 1.png)

- **Starting Line**: Begins at the stop light on coldwaterheading south-bound, then once the light changes the players turn left into the parking lot
- **First Section**: Leads into the parking lot with spikes and a challenging dip
- **Second Section**: Navigates through a narrow passage going towards the hamilton parking lot and then through to the senior, there are cars blokcing the loading zone so you must go around then towards security kiosk
- **Third Section**: Curves back up toward the security kiosk
- **Fourth Section**: traverse up towards the steep fire road leading up to St. Saviour's Chapel
- **Fifth Section**: Behind the chapel is another steep descent(making an artificial ramp) into the teacher parking lot. or there is an alternate route up near the admissions buliding where there is a ramp that launches you up into the air and you land onto the exit of the faculty lot. 
- **Sixth Section**: Route continues toward Weiler Hall and to around the track with obstacles
- **Seventh Section**: drives up the bleachers leading to lower quad, then to upper quad then to the cafeteria 
- **Eighth Section**: Exit the cafeteria and goes through the main pathway to the taper gym, and through the taper gym
- **Final Section**: exits the taper gym and goes up the handicap ramp and restarts the lap at the entry of the parking lot.

## Game Features
- All racing karts have identical performance specifications
- Racers can collect various powerups throughout the track
- There are multiple maps you can go through instead of just one
    - ex. All-roof path, all-stairs path, just track, cafeteria route, etc
- Feature for with/without "railings" where there are railings on the edges of the paths to make it easier for new players
- Hidden Shortcuts
- Moving Obsacles 
    - Lizards, students, etc 
- In-game currency as rewards for placing high in races in order to purhcase skins, upgrades, teachers, etc. 
- Weather, time of day, seasons 

- Players can choose from different Harvard-Westlake teachers as drivers
    - Each teacher character has 2 unique special abilities:
        - 1st: based on the subject they teach (general)
        - 2nd: based on their personality (specific)
    - Teachers have unique voice lines potentially sampled from the teachers themselves  

## Teacher Abilities
    - *Everything is subject to playtesting and balancing
    - Mr. Nealis
        - Cart: Ford F450 SUPER DUTY
        - Ability: Press 'k' to enter autopilot (cart will drive optimal path) and for 10 seconds jump in the truck bed and equip an automatic turret. Dealing enough damage (landing bullets for ~3 seconds) to other racers will cause them to spin out for a duration.
    - Mr. Theiss
        - Cart: Tesla Cybertruck
        - Ability: Press 'k' to activate a "Crypto-flip" which generates a sha1 hash from the race time passed at that moment on screen and gains a speed boost for a duration of the first number in seconds (hexidecimal)
    - Mr. Yaron
        - Ability: Press 'k' to activate "Mossad Agent" Order an airstrike on an area of the map (The user has 5 seconds to select the airstrike location, during which time the car drives in autopilot)
        - Cart: Merkava M48 Heavy Tank
        - Passive: "Cockroach" 2% slower overall movement speed, but also 15% lower durations for all debuffs 
    - Mr. Commons
        - Cart: FLoating Grad Cap
        - Ability: Press 'k' to activate "Pursuit of Educational Excellence"- Obstruct all other players' views with a student handbook for 5 seconds
    - Earl (Security Guard)
        - Cart: Harley Davidson Motorcycle
        - Ability: Press 'k' to activate "TONY!!" Tony zooms across the track just in front of the user on a golf cart running over racers
    - Mr. Varney
        - Cart: Subaru Forester
        - Ability: Press 'k' to activate "Dance Dance Revolution"- All other racers are slightly slowed (5%) for a duration while a disco ball obstructs their view for 3 seconds
    - Pairot
        - Cart: Food truck (Thai) or vietnamese
        - Ability: "No Charge" Disable all other powerups for a duration
    - Preciado
        - Cart: Running
        - Ability: Hold 'k' to "Sprint" Speeds up his run by 10% and if he hits someone they spin out, but post ability he gets tired and slows for a duration. The Sprint can last up to 5 seconds
    - Mr. X  
        - Cart: Eco-friendly smart car
        - Ability 'k' renewable energy.  Recharges 
        your energy boost for your EV car.  Puts a blinking 'turtle' mode on everyone close for 2.5 seconds 

## Developed Map Ideas:
### Cafeteria Chaos:

- Once the players enter the quad the final section, they must go through the cafeteria. 
- players burst through the double doors of the Harvard-Westlake cafeteria—only to instantly shrink to the size of a salt shaker.
- the cafeteria transforms into a maze of oversized obstacles: towering cartons of milk, sliding meatballs, bouncing apples, and puddles of spilled Gatorade that act like sticky traps.
- all while pyro is chasing you with a giant ladel, stomping on the ground. if this ladel makes contact with a cart, it sends the cart spinning out to the side.
- players enter through the cafeteria entrance and circle around the middle counter, and then they exit through the checkout station and must get checked out

## Powerups
- Racers can collect various powerups avaliable to all racers throughout the track
- Racers can drift for a tiered speed boost based off of drift time

### Powerup Ideas
🔴 Offensive Powerups
- Pop Quiz:	Temporarily blinds nearby racers with a flurry of quiz papers.
- Red Reprimand Slip:  Homing attack that targets the racer directly in front. Inspired by detention slips.
- Lunch Rush:	Drops spilled cafeteria trays behind you, causing others to slip.
- Dean’s Call: Freezes a random player with a "please come to the Dean’s Office" notice.
- Fire Drill:  Triggers a loud bell — all players must pull over briefly unless they have “Permission to Leave Class” powerup.
- Trigonometry:    Release sin curves that push other players back/slow them down
- Falling Grades:  Create pools of "F"s on the floor bahind the player that will slow other players down because depression
- Mandatory Meeting in Rugby:  Choose another player to slow for a duration
- All School Assembly: Slow all other players for a duration

🔵 Defensive Powerups
Permission Slip Shield  
    Absorbs the next incoming attack. Has a school crest animation.
Campus Map Warp Teleports you forward to the next checkpoint, bypassing obstacles.
Library Silence Bubble  Surrounds your kart in a quiet zone — you can’t be targeted for 5 seconds.

🟢 Utility / Speed Boosts
H-W Spirit Boost    Short burst of speed. Kart leaves a trail of red-and-black streamers.
Excused absence Teleport to the end of the section of the track that the player is currently on

🟣 Trick / Terrain Manipulation
Sprinkler Trap  Activates sprinklers on a section of the track, making it slippery.
Science Lab Spill   Leaves a glowing goo trail — racers who hit it spin out.

## Campus Overview

<!-- Image 1: Aerial view of Harvard-Westlake campus showing Rugby Theatre, athletic fields, and swimming pool -->
![Harvard-Westlake Campus Aerial View 1](./hw1.png)

<!-- Image 2: Aerial view showing St. Saviour's Chapel, Mudd Library, Rugby Theatre, and the main field -->
![Harvard-Westlake Campus Aerial View 2](./hw2.png)


## Initial Image
![Generated Image for Idea](./generated1.png)
![Generated Image for Idea](./generated2.png)