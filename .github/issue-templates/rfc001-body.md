## 🤖 RFC Implementation Request

**RFC**: RFC001: Core Game Loop & State Management  
**File**: `docs/RFC/RFC001-Core-Game-Loop.md`  
**Estimated Effort**: 2-3 days  
**Dependencies**: None (foundational system)

## 📋 Task Description

Implement the core game engine, turn-based timing system, and ECS world management that coordinates all game systems and manages the overall game lifecycle.

## ✅ Definition of Done

Please complete ALL checkboxes in the RFC specification before marking this issue as complete:

**Review the complete RFC specification**: `docs/RFC/RFC001-Core-Game-Loop.md`

The RFC contains detailed:
- GameEngine class with state management (MainMenu, Playing, Paused, etc.)
- TurnManager for turn-based coordination  
- Event system for inter-system communication
- ECS integration with Arch framework
- Game loop architecture and system execution order

## 🎯 Success Criteria

- [ ] All RFC acceptance criteria checkboxes completed
- [ ] GameEngine manages game states correctly (menu ↔ playing ↔ paused)
- [ ] Turn-based logic processes actions sequentially  
- [ ] ECS systems execute in correct order each turn
- [ ] Event system broadcasts state changes to other systems
- [ ] Unit tests written and passing (>80% coverage)
- [ ] Code follows project standards from `AGENTS.md`
- [ ] RFC status updated to `✅ Complete`

## 🔗 Integration Points

**This is a foundational system** - other RFCs depend on this implementation:
- RFC002 (Terminal Shell) uses GameEngine
- RFC003 (Map System) uses ECS World and events  
- RFC004 (Player Movement) consumes turn events
- All other systems build on this foundation

**Key Events to Implement:**
- `GameEvents.GameStateChanged` - State transitions
- `GameEvents.TurnStarted` - Beginning of turn
- `GameEvents.TurnEnded` - End of turn

## 📚 Resources

- **Agent Guidelines**: `AGENTS.md` - ECS patterns and coding standards
- **Project Context**: `CLAUDE.md` - Overall architecture overview
- **Dependencies**: Arch ECS framework, .NET 8 patterns

## 🌿 Branch Strategy

**Please create a feature branch for this implementation:**
- Branch name: `feature/rfc001-core-game-loop`
- All commits should go to this branch
- Open a Pull Request when ready for review
- Do NOT commit directly to main branch

## 🔄 Development Process

1. **Create feature branch** from main
2. **Implement according to RFC specification** 
3. **Write comprehensive tests** (unit + integration)
4. **Update RFC status** to `🔄 In Progress` → `✅ Complete`
5. **Open Pull Request** with description of implementation
6. **Request review** - maintain branch protection

---

**@copilot This is the foundational RFC for the entire dungeon crawler. Please create a feature branch `feature/rfc001-core-game-loop` and implement the core game loop system according to the specification. This enables all other game systems to function properly.**