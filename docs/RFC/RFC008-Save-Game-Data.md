# RFC008: Save Game Data System

## 📋 Metadata
- **RFC Number**: 008
- **Title**: Save Game Data System
- **Status**: 📝 Draft
- **Author**: System Architect
- **Created**: 2025-08-30
- **Dependencies**: RFC001 (Core Game Loop)

## 🎯 Objective

Implement basic save/load functionality that persists player progress, map state, and game session to JSON files.

## 📖 Problem Statement

Players need to save their progress and continue playing later. The system should save player stats, inventory, current map, and position.

## ✅ Definition of Done

### **Required Data Structures**
- [ ] `GameSaveData` class with Player stats, Inventory items, MapData, CurrentPosition
- [ ] `MapSaveData` class with Width, Height, TileData array, EntityPositions
- [ ] `PlayerSaveData` class with Name, Level, Health, Mana, Experience, Stats

### **Required Functionality**
- [ ] Ctrl+S saves game to `saves/quicksave.json`
- [ ] Ctrl+L loads game from `saves/quicksave.json`  
- [ ] Save includes: all player data, current map layout, item positions, current turn
- [ ] Load restores: player state, map, entities, game turn counter

### **File Management**
- [ ] Creates `saves/` directory if it doesn't exist
- [ ] Uses JSON serialization for human-readable save files
- [ ] Handles file I/O errors gracefully (shows error messages)
- [ ] Validates save file format before loading

### **Success Criteria**
- [ ] Save game, close program, restart, load game - identical state restored
- [ ] Player position, health, mana, inventory preserved exactly
- [ ] Map layout and all items in same positions
- [ ] Save/load operations complete in <1 second
- [ ] Clear feedback: "Game saved" / "Game loaded" messages

### **Error Handling**
- [ ] Cannot save during combat or menu screens
- [ ] File permission errors show helpful messages  
- [ ] Corrupted save files detected and reported
- [ ] Missing save file shows "No save file found"

### **Technical Requirements**
- [ ] Uses System.Text.Json for serialization
- [ ] Save files stored in user documents or game directory
- [ ] Atomic save operations (temp file + rename to prevent corruption)

## 🔗 Integration Points

**Dependencies**: All game components must be serializable
**Events Published**: GameSaved, GameLoaded
**Events Consumed**: SaveRequested, LoadRequested

---

**Estimated Effort**: 2 days
**Risk Level**: Medium - File I/O and serialization complexity
**Priority**: Medium - Important for player experience