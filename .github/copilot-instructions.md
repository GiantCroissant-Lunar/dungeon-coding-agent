# GitHub Copilot Coding Agent Instructions

## 🎯 Project Overview

This is a **classic rogue-like dungeon crawler** built with .NET 8, Arch ECS, and Terminal.Gui v2. The project is designed specifically for **GitHub Copilot Coding Agent testing** with parallel RFC-based development.

## 🏗️ Architecture

- **Entity Component System**: Use Arch ECS for all game state and logic
- **Terminal UI**: Terminal.Gui v2 for cross-platform terminal interface  
- **Turn-Based**: Classic rogue-like timing with discrete turns
- **Modular Design**: RFC-driven development with minimal inter-dependencies

## 📋 RFC Implementation Process

### **Before Starting Any RFC:**

1. **Read the complete RFC specification** in `docs/RFC/RFC00X-[name].md`
2. **Check dependencies** - ensure required RFCs are completed first
3. **Review integration points** - understand events consumed/published
4. **Study acceptance criteria** - know exactly what constitutes "done"

### **Implementation Guidelines:**

- **Follow ECS patterns** - Components are data, Systems are logic
- **Use project standards** - See `AGENTS.md` for detailed coding guidelines
- **Write tests** - Achieve >80% coverage with unit and integration tests
- **Fire events** - Systems communicate through `GameEvents` static class
- **Handle errors gracefully** - Validate inputs and provide user feedback

## 🎮 Game Vision

Create a **playable dungeon crawler** with:
- Procedurally generated dungeons (rooms + corridors)
- Turn-based player movement with 8-directional input
- Simple combat with melee attacks and health/damage
- Basic inventory system (pickup/drop/use items)
- Enemy AI that chases and attacks the player
- Save/load functionality with JSON persistence
- Terminal UI with game map, status bars, and message log

## 🌿 Branch Strategy

**IMPORTANT: Always use feature branches for RFC implementations**

### **Required Branch Naming Convention:**
- `feature/rfc001-[short-description]` for RFC implementations
- `feature/rfc002-terminal-application-shell`  
- `feature/rfc003-map-generation-system`
- etc.

### **Workflow:**
1. **Create feature branch** from latest main
2. **Implement RFC** with all acceptance criteria
3. **Write comprehensive tests** (>80% coverage)
4. **Update RFC status** in docs (Draft → In Progress → Complete)
5. **Open Pull Request** with detailed description
6. **Request review** - never merge directly to main

### **Branch Protection:**
- Main branch is protected - requires pull request reviews
- All RFC work MUST go through feature branches
- No direct commits to main branch allowed
- Pull requests require approval before merge

## 🔧 Technical Standards

### **Code Organization**
```
src/DungeonCodingAgent.Game/
├── Components/     # ECS data structures
├── Systems/        # ECS logic systems
├── UI/            # Terminal.Gui views
├── Core/          # Game constants, events, utilities
├── Generation/    # Map and content generation
└── Persistence/   # Save/load functionality
```

### **Key Patterns**
- **Components**: Pure data structures (structs preferred)
- **Systems**: Inherit from `SystemBase<World, float>` 
- **Events**: Use `GameEvents.RaiseXXX()` for inter-system communication
- **UI**: Separate Terminal.Gui views from game logic
- **Testing**: Test behavior, not implementation details

### **Dependencies**
- **Terminal.Gui 2.0.0** - UI framework (required for all UI)
- **Arch 2.0.0** - ECS framework (required for all game logic)
- **System.Text.Json** - Save/load serialization
- **xUnit** - Testing framework

## 🎯 RFC Priority Order

### **Foundation (Start Here)**
1. **RFC001: Core Game Loop** - Game engine, turn management, ECS world
2. **RFC002: Terminal.Gui Shell** - Application lifecycle, main window, input

### **Core Gameplay**  
3. **RFC003: Map Generation** - Dungeon layout, rendering, field-of-view
4. **RFC004: Player Movement** - Input handling, collision, position updates
5. **RFC009: Simple Enemy AI** - Basic chase/attack enemy behavior

### **Features**
6. **RFC006: Basic Inventory** - Item pickup/drop/use mechanics
7. **RFC007: Message Log UI** - Scrolling game event display
8. **RFC010: Health Status Bar** - Visual health/mana/XP progress bars
9. **RFC008: Save Game Data** - JSON persistence system

## ✅ Definition of Complete

An RFC is complete when:
- [ ] All acceptance criteria checkboxes in RFC are checked
- [ ] Unit tests written and passing
- [ ] Integration with existing systems working
- [ ] Code follows project standards
- [ ] RFC status updated from `📝 Draft` to `✅ Complete`

## 🚫 What NOT to Do

- Don't implement systems not specified in the RFC
- Don't skip writing tests  
- Don't break existing functionality
- Don't add dependencies not listed in RFC
- Don't implement UI outside Terminal.Gui framework
- Don't use inheritance for ECS components (use composition)

## 🎮 Expected User Experience

```
┌─────────────────────────────────────────────────────────────┐
│ File  Game  Help                               [Turn 15] │
├─────────────────────────────────────────────────────────────┤
│  ########################                                    │
│  #......#..............#    Controls:                       │
│  #......#..............#    Arrow keys/WASD - Move          │
│  #..@...+..............#    g - Pick up item                │
│  #......#.......g......#    i - Inventory                   │
│  #......################    Space - Wait                    │
│  ########################                                    │
├─────────────────────────────────────────────────────────────┤
│ Health: [████████░░░░░░░] 40/50 | Mana: [██████░░░░] 15/30  │
│ Level: 2 | Exp: [███░░░░░░░] 150/400 | Turn: 15            │
├─────────────────────────────────────────────────────────────┤
│ > You move west                                             │
│ > The goblin attacks you for 3 damage                      │
│ > You attack the goblin for 5 damage                       │
└─────────────────────────────────────────────────────────────┘
```

---

**Focus on creating a fun, playable rogue-like experience with clean, maintainable code. Each RFC builds toward this unified vision.**