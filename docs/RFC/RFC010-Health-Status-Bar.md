# RFC010: Health and Status Bar UI

## 📋 Metadata
- **RFC Number**: 010
- **Title**: Health and Status Bar UI
- **Status**: 📝 Draft
- **Author**: System Architect
- **Created**: 2025-08-30
- **Dependencies**: RFC002 (Terminal.Gui Application Shell), RFC004 (Player Movement)

## 🎯 Objective

Implement a status bar that displays player health, mana, level, and experience as visual progress bars and text.

## 📖 Problem Statement

Players need immediate visual feedback about their character's vital statistics without opening menus.

## ✅ Definition of Done

### **Required UI Layout**
- [ ] Status bar takes up 2 rows at bottom of screen, above message log
- [ ] **Row 1**: `Health: [████████████████████] 85/100 | Mana: [██████░░░░░░░░░░░░░░] 30/50`
- [ ] **Row 2**: `Level: 3 | Exp: [████████████░░░░░░░░] 1250/2000 | Turn: 47`

### **Required Components**
- [ ] `StatusBarView` Terminal.Gui component
- [ ] Real-time updates when player stats change
- [ ] Progress bars using block characters (██ for filled, ░░ for empty)
- [ ] Color coding: Health (red), Mana (blue), Experience (yellow)

### **Required Visual Elements**
- [ ] Health bar: 20 characters wide, shows current/max HP
- [ ] Mana bar: 20 characters wide, shows current/max MP  
- [ ] Experience bar: 20 characters wide, shows progress to next level
- [ ] Text labels with current/max values and percentage

### **Success Criteria**
- [ ] Bars update immediately when player health/mana changes
- [ ] Colors display correctly on color and monochrome terminals
- [ ] Layout adapts to terminal width (minimum 80 characters)
- [ ] Performance: updates complete in <16ms (no UI lag)
- [ ] Experience bar fills and resets when player levels up

### **Color Requirements**
- [ ] Health bar: Red filled blocks, dark red empty blocks
- [ ] Mana bar: Blue filled blocks, dark blue empty blocks
- [ ] Experience bar: Yellow filled blocks, dark gray empty blocks
- [ ] Status text: White on black background

### **Integration Requirements**
- [ ] Listens to PlayerHealthChanged, PlayerManaChanged, PlayerLeveledUp events
- [ ] Updates turn counter from game loop
- [ ] Handles window resize events gracefully

## 🔗 Integration Points

**Dependencies**: Player stats system, Terminal.Gui framework, game events
**Events Consumed**: PlayerHealthChanged, PlayerManaChanged, PlayerLeveledUp, TurnStarted

---

**Estimated Effort**: 1 day
**Risk Level**: Low - Simple UI display
**Priority**: High - Essential player feedback