# 🌼 Flower Whisper

A lightweight, interactive, semi-photorealistic flower simulation game.

---

# 1. Objective

Build a playable prototype with:

- Interactive flower entities
- Growth simulation system
- Basic UI and collection system
- Real-time animation (wind + touch)

Target:
- Desktop (primary)
- WebGL (optional)

---

# 2. Tech Stack

- Engine: Unity (URP)
- Language: C#
- Rendering: Universal Render Pipeline
- Input: Mouse

---

# 3. Project Structure

Create the following structure:

/Assets
  /Scenes
  /Scripts
  /Prefabs
  /Models
  /Materials
  /Textures
  /UI

---

# 4. Core Systems

## 4.1 Flower Entity

Each flower must be a prefab with:

Fields:

- id (string)
- species (string)
- growthStage (int: 0–5)
- health (float: 0–100)
- water (float: 0–100)
- sunlight (float: 0–100)

---

## 4.2 Growth System

Implement time-based simulation:

- Tick interval: 10 seconds

Logic:

IF water > 50 AND sunlight > 50:
    growthStage increases (random chance)
ELSE:
    health decreases

Max stage: 5

---

## 4.3 Interaction System

Implement:

- Click → small animation
- Hold → zoom-in camera
- Drag water → increase water value
- Mouse movement → wind effect

Use raycasting to detect flower objects.

---

## 4.4 Animation System

Must include:

- Wind animation (shader-based preferred)
- Growth animation (scale or morph)
- Touch feedback (scale or rotation)

---

## 4.5 Collection System

Maintain a list of discovered flowers:

- Unlock when growthStage == 5
- Store in memory (ScriptableObject or JSON)

---

# 5. Visual Requirements

## 5.1 Models

Priority:

1. Download from Sketchfab (low poly, CC license)
2. If unavailable → generate placeholder (simple mesh)

Constraints:

- < 10k tris
- FBX or GLB

---

## 5.2 Materials

Use PBR:

- BaseColor
- Normal
- Roughness

Optional:

- Transparency (for petals)

---

## 5.3 Wind Shader

Implement vertex displacement:

vertex.y += sin(time + position.x) * intensity

Apply to all plant materials.

---

# 6. Scene Setup

Create a simple scene:

- Ground plane
- 3–5 flowers
- Basic lighting (directional light)

---

# 7. UI

Minimal UI:

- Water indicator
- Growth stage display
- Collection panel (list)

---

# 8. Performance Constraints

- Target FPS ≥ 50
- Enable GPU instancing if possible
- Use LOD if assets are heavy

---

# 9. Build Output

Produce:

- Playable Unity scene
- Build (Standalone)
- Optional: WebGL build

---

# 10. Automation Rules

If asset not found:
→ create placeholder mesh

If animation missing:
→ generate simple animation

If FPS < 50:
→ reduce texture size
→ disable shadows

---

# 11. Testing Requirements

- No runtime errors
- All interactions functional
- At least 3 flower types present

---

# 12. Human Intervention Required

The following require human input:

1. Asset licensing validation
2. Art style consistency review
3. Gameplay quality evaluation
4. Final build & publishing

---

# 13. Execution Plan

1. Initialize Unity project (URP)
2. Create base scene
3. Import or generate flower models
4. Implement Flower Entity system
5. Implement Growth system
6. Implement Interaction system
7. Add animation & shader
8. Add UI
9. Optimize performance
10. Build project

---

# 14. Definition of Done

- Scene runs without errors
- Flowers grow over time
- Player can interact with flowers
- Collection system updates correctly
- Build is playable

