# RFC007: Simple Message Log UI

## 📋 Metadata
- **RFC Number**: 007
- **Title**: Simple Message Log UI
- **Status**: 📝 Draft
- **Author**: System Architect  
- **Created**: 2025-08-30
- **Dependencies**: RFC002 (Terminal.Gui Application Shell)

## 🎯 Objective

Implement a basic scrolling message log that displays game events to the player in a dedicated panel.

## 📖 Problem Statement

Players need to see what's happening in the game through text messages like "You hit the goblin for 5 damage" or "You picked up a health potion."

## ✅ Definition of Done

### **Required Components**
- [ ] `GameMessage` struct with Text, MessageType (Info, Combat, System), and Timestamp
- [ ] `MessageLog` class with AddMessage, GetMessages, Clear methods and MaxMessages = 50

### **Required UI**
- [ ] `MessageLogView` terminal UI component taking bottom 30% of screen height
- [ ] Scrollable text area showing most recent messages at bottom
- [ ] Different colors: Info (white), Combat (red), System (yellow)  
- [ ] Auto-scroll to show newest messages

### **Required Integration**
- [ ] Listens to `GameEvents.MessageLogged` event
- [ ] Subscribe to all relevant game events (movement, combat, items)
- [ ] Messages display format: "[Turn 5] You move north." or "[Turn 12] Goblin hits you for 3 damage."

### **Required Messages**
- [ ] Player movement: "You move [direction]"
- [ ] Item pickup: "You pick up [item name]"  
- [ ] Combat: "[Attacker] hits [target] for [damage] damage"
- [ ] System: "Game saved", "Welcome to the dungeon!"

### **Success Criteria**
- [ ] Messages appear immediately when events occur
- [ ] Scrolling works with mouse wheel or page up/down keys
- [ ] Message log persists for entire game session
- [ ] Colors display correctly on different terminals
- [ ] Log truncates old messages when exceeding MaxMessages

### **Technical Requirements**
- [ ] Uses Terminal.Gui ScrollView component
- [ ] Efficient string rendering (no performance lag)
- [ ] Thread-safe message adding (if needed)

## 🔗 Integration Points

**Dependencies**: Terminal.Gui framework, GameEvents system
**Events Consumed**: MessageLogged, PlayerMoved, CombatResolved, ItemInteraction

---

**Estimated Effort**: 1 day
**Risk Level**: Low - Simple UI component
**Priority**: High - Essential for player feedback