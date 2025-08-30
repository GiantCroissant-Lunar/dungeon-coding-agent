# RFC006: Basic Inventory System

## 📋 Metadata
- **RFC Number**: 006
- **Title**: Basic Inventory System
- **Status**: 📝 Draft
- **Author**: System Architect
- **Created**: 2025-08-30
- **Dependencies**: RFC001 (Core Game Loop), RFC004 (Player Movement System)

## 🎯 Objective

Implement a simple inventory system that allows the player to pick up, drop, and use basic items like health potions and keys.

## 📖 Problem Statement

The player needs basic item management:
- Pick up items from the ground
- Store items in a simple list-based inventory (max 10 items)
- Drop items back to the ground
- Use consumable items like health potions
- View inventory contents with 'i' key

## ✅ Definition of Done

### **Required Components**
- [ ] `Inventory` component with List<EntityReference> Items and MaxSize = 10
- [ ] `Item` component with Name, Description, ItemType, IsStackable
- [ ] `Consumable` component with EffectType (HealHealth, RestoreMana) and Amount

### **Required Systems**
- [ ] `InventorySystem` that processes pickup/drop/use actions
- [ ] Items spawn on map as entities with Position + Item + Renderable components

### **Required Actions**
- [ ] 'g' key picks up item at player position (if inventory not full)
- [ ] 'd' key opens drop menu (lists items, select with number key)  
- [ ] 'i' key opens inventory view (shows item names, close with 'i' or ESC)
- [ ] 'u' key opens use menu for consumables (heals player immediately)

### **Required Items**
- [ ] Health Potion (red '!' character, heals 25 HP)
- [ ] Mana Potion (blue '!' character, restores 15 mana)  
- [ ] Key (yellow key '♂' character, for doors in future)

### **Success Criteria**
- [ ] Player can pick up items until inventory full (show "Inventory full" message)
- [ ] Player can drop items, they appear on ground at player position
- [ ] Using health potion increases player health (capped at maximum)
- [ ] Using mana potion restores player mana (capped at maximum)
- [ ] Inventory UI shows item names clearly, updates in real-time
- [ ] All actions provide clear feedback messages

### **Error Handling**
- [ ] Cannot pick up items when inventory full
- [ ] Cannot drop items when inventory empty
- [ ] Cannot use items that aren't consumable
- [ ] Clear error messages for all invalid actions

## 🔗 Integration Points

**Dependencies**: Player stats (Health, Mana) from RFC004
**Events Published**: ItemPickedUp, ItemDropped, ItemUsed
**Events Consumed**: PlayerActionRequested

---

**Estimated Effort**: 1-2 days
**Risk Level**: Low - Simple data management
**Priority**: Medium - Enables gameplay progression