# 🗺️ Kart Game Minimap Tech Spec

## Overview

This document outlines the design and functionality of the **Minimap** feature for the HW Kart Klash. The minimap provides a simplified, top-down 2D view of the race track that shows the player's position in real time. It is designed for usability and minimal distraction during gameplay.

---

## 🎯 Features

- **Real-Time Player Tracking**  
  Displays the player's current position on the minimap, updating continuously during the race.

- **Toggle Visibility**  
  Players can press the **`M` key** to show or hide the minimap during gameplay.

- **Auto-Activate on Start**  
  The minimap automatically appears when the race begins (after loading completes).

---

## 🖼️ Appearance

- **Perspective**: Top-down 2D view.
- **Map Content**: Roads only, no scenery or elevation.
- **Player Indicator**: Highlighted icon that stands out from the rest of the map.
- **Dynamic Viewport**: Only a portion of the map is shown at any time, centering around the player’s position.

---

## 🧱 Dependencies

- **Track Map**
  - Requires extraction of road layout for 2D projection.
- **Player (Kart)**
  - Must access the player’s transform or position in Unity.
- **Camera System**
  - Used to render or update the minimap display.
- **Unity UI System**
  - Utilized for the minimap interface, toggle button, and player icon rendering.

---

## 🛠️ Implementation Notes

- **No Custom Code Required**  
  The minimap is assembled entirely within Unity using built-in tools and UI elements.
  (Refer to relevant tutorial/documentation for setup.)

- **Low Priority**  
  This feature is considered optional and may be deferred depending on development schedule.

---

## 📌 Controls

| Key | Action           |
|-----|------------------|
| `M` | Toggle minimap   |
