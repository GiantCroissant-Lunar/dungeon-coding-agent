## 🤖 RFC Implementation Request

**RFC**: RFC002: Terminal.Gui Application Shell  
**File**: `docs/RFC/RFC002-Terminal-Application-Shell.md`  
**Estimated Effort**: 3-4 days  
**Dependencies**: RFC001 (Core Game Loop)

## 📋 Task Description

Implement the Terminal.Gui application shell that provides the main UI framework, handles terminal initialization, manages application lifecycle, and coordinates between the game engine and terminal UI.

## ✅ Definition of Done

Please complete ALL checkboxes in the RFC specification before marking this issue as complete:

**Review the complete RFC specification**: `docs/RFC/RFC002-Terminal-Application-Shell.md`

The RFC contains detailed:
- DungeonApplication class with Terminal.Gui lifecycle
- MainWindow layout with game map, status bar, message log areas
- Input handling and key mapping system
- Color scheme configuration for different terminals
- Integration with GameEngine from RFC001

## 🎯 Success Criteria

- [ ] All RFC acceptance criteria checkboxes completed
- [ ] Application starts cleanly in terminals ≥80x24
- [ ] UI layout adapts to terminal resize events
- [ ] Keyboard input handled correctly (no key conflicts)
- [ ] Game map area renders with proper placeholder content
- [ ] Integration with GameEngine state management working
- [ ] Unit tests written and passing (>80% coverage)
- [ ] Code follows project standards from `AGENTS.md`
- [ ] RFC status updated to `✅ Complete`

## 🔗 Integration Points

**Dependencies:**
- RFC001: Core Game Loop (GameEngine, GameState, Events)

**This enables:**
- RFC003: Map Generation (GameMapView rendering target)
- RFC007: Message Log UI (MessageLogView component)
- RFC010: Status Bar UI (StatusBarView component)

**Key Integration:**
- Consumes `GameEvents.GameStateChanged` to update UI
- Publishes `InputEvents.PlayerActionRequested` for player input
- Provides rendering targets for all other UI components

## 📚 Resources

- **Agent Guidelines**: `AGENTS.md` - Terminal.Gui patterns and UI standards
- **Dependencies**: Terminal.Gui 2.0.0, GameEngine from RFC001
- **UI Layout**: See RFC specification for exact layout design

## 🌿 Branch Strategy

**Please create a feature branch for this implementation:**
- Branch name: `feature/rfc002-terminal-application-shell`
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

**@copilot This provides the UI foundation that all other systems will build upon. Please create a feature branch `feature/rfc002-terminal-application-shell` and implement the Terminal.Gui application shell according to the specification.**