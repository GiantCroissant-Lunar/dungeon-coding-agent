# RFC011: Field-of-View (FOV) & Fog-of-War

## 📋 Metadata
- Status: 📝 Draft
- Author: Architecture Team
- Created: 2025-08-31
- Dependencies: RFC001 (Core Loop), RFC003 (Map), RFC005 (Combat - read), RFC007 (AI - read)

## 🎯 Objective
Add line-of-sight FOV and persistent fog-of-war so only tiles visible to the player (and optionally explored) are rendered at full intensity. This supports stealth, tactical awareness, and performance.

## 📖 Problem Statement
The current rendering shows the entire map. For roguelike gameplay, visibility should be constrained by walls/doors and the player’s vision radius. Previously seen ("explored") tiles should remain dimly visible for navigation.

## 🏗️ Technical Specification
- Vision Model
  - Circular radius R (config default: 8 tiles), blocked by opaque tiles (walls/closed doors).
  - Algorithm: Recursive Shadowcasting (preferred) or Permissive FOV. Deterministic with game RNG.
- Data
  - Components
    - `Visible` (transient turn-scoped)
    - `Explored` (persistent, per tile)
  - Map tiles expose `IsOpaque` and `IsWalkable` flags.
- Systems
  - `FovSystem`
    - Runs after movement/turn advance.
    - Computes visible set from player position and writes `Visible` marks; merges into `Explored`.
  - `RenderSystem` integration
    - Draw order prefers visible glyphs full brightness; explored tiles dim palette; unseen blank.
- Performance
  - Target: 80x25 viewport ≤ 1 ms per FOV update on typical hardware.

## 🖼️ UI Mockups (Terminal.Gui)
```
Visible: bright; Explored: dim; Unseen: blank

###########
#.........#
#.@@@@....#   @ player in lit corridor
#..g......#   g goblin in shadow (not drawn if unseen)
#.........#
###########
```
Status line example:
```
HP: 10/10  Depth:1  Turn:42  FOV:8
```

## 🔗 Integration Points
- `RFC005` Combat should only allow targeting within current `Visible` tiles (optional rule).
- `RFC007` AI can optionally use its own FOV later; out-of-scope here.

## ✅ Acceptance Criteria
- [ ] Moving the player updates FOV each turn; visible tiles match shadowcasting expectations.
- [ ] Explored tiles persist and render dimmer than visible tiles.
- [ ] Walls/closed doors block sight; open doors allow sight.
- [ ] Configurable radius via config (default 8).
- [ ] Unit tests cover blocking and corridor/room visibility cases.

## 🧪 Testing Strategy
- Property tests on FOV symmetry and blocking.
- Snapshot tests for small maps (ASCII fixtures) comparing visible sets.

## 📊 Performance Considerations
- Cache octant transforms; reuse buffers between turns.
- Avoid per-tile allocations; use stack/pooled structures.
