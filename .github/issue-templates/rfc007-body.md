## 🤖 RFC Implementation Request

**RFC**: RFC007: Simple Message Log UI  
**File**: `docs/RFC/RFC007-Simple-UI-Messages.md`  
**Estimated Effort**: 1 day  
**Dependencies**: RFC002 (Terminal.Gui Application Shell)

## 📋 Task Description

Implement a basic scrolling message log that displays game events to the player in a dedicated Terminal.Gui panel.

## ✅ Definition of Done

Please complete ALL checkboxes in the RFC specification before marking this issue as complete:

**Review the complete RFC specification**: `docs/RFC/RFC007-Simple-UI-Messages.md`

The RFC contains detailed:
- MessageLogView Terminal.Gui component taking bottom 30% of screen
- GameMessage struct with Text, MessageType, and Timestamp
- Scrollable text area with color coding (Info=white, Combat=red, System=yellow)
- Integration with GameEvents system

## 🎯 Success Criteria

- [ ] All RFC acceptance criteria checkboxes completed
- [ ] MessageLogView displays in bottom panel of main window
- [ ] Messages appear immediately when events occur
- [ ] Scrolling works with mouse wheel or page up/down keys
- [ ] Different message types display in correct colors
- [ ] Log truncates old messages when exceeding MaxMessages (50)
- [ ] Unit tests written and passing (>80% coverage)
- [ ] Code follows project standards from `AGENTS.md`  
- [ ] RFC status updated to `✅ Complete`

## 🔗 Integration Points

**Dependencies:**
- RFC002: Terminal.Gui framework and MainWindow layout

**Integration:**
- Listens to `GameEvents.MessageLogged` event
- Subscribes to player movement, combat, and item events
- Provides essential player feedback for all game actions

**Message Examples:**
- "You move north" (player movement)
- "You pick up health potion" (item interaction)
- "Goblin hits you for 3 damage" (combat)
- "Game saved" (system messages)

## 📚 Resources

- **Agent Guidelines**: `AGENTS.md` - Terminal.Gui UI patterns
- **Dependencies**: Terminal.Gui ScrollView component
- **Integration**: GameEvents system for message broadcasting

## 🌿 Branch Strategy

**Please create a feature branch for this implementation:**
- Branch name: `feature/rfc007-message-log-ui`
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

**@copilot This is a simple but essential UI component that provides player feedback. Please create a feature branch `feature/rfc007-message-log-ui` and implement according to the specification. Can be implemented independently once RFC002 is complete.**