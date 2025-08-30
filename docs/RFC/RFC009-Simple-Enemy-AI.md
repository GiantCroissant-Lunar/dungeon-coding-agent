# RFC009: Simple Enemy AI

## 📋 Metadata
- **RFC Number**: 009  
- **Title**: Simple Enemy AI
- **Status**: 📝 Draft
- **Author**: System Architect
- **Created**: 2025-08-30
- **Dependencies**: RFC001 (Core Game Loop), RFC003 (Map Generation), RFC004 (Player Movement)

## 🎯 Objective

Implement basic enemy AI that creates 2-3 enemy types that move toward the player and attack when adjacent.

## 📖 Problem Statement

The game needs enemies that provide challenge but are simple to implement: they should detect the player, chase them, and attack when close enough.

## ✅ Definition of Done

### **Required Enemy Types**
- [ ] **Goblin** ('g' green char): 20 HP, attacks for 3-6 damage, moves every turn toward player
- [ ] **Orc** ('o' red char): 35 HP, attacks for 5-9 damage, moves every other turn (slower)
- [ ] **Skeleton** ('s' white char): 15 HP, attacks for 2-4 damage, moves every turn but only if player within 5 tiles

### **Required Components**
- [ ] `EnemyAI` component with AggroRange, LastSeenPlayerPos, ChaseSpeed  
- [ ] `CombatStats` component with AttackPower, Defense, Health
- [ ] Enemy entities have Position, Renderable, EnemyAI, CombatStats, Health components

### **Required AI Behavior**
- [ ] **Detection**: Enemy detects player when within AggroRange (line-of-sight not required)
- [ ] **Chasing**: Enemy moves toward player's last known position using simple pathfinding
- [ ] **Combat**: When adjacent to player, enemy attacks instead of moving
- [ ] **Simple Pathfinding**: Move toward player, prefer cardinal directions, avoid walls

### **Required Systems**  
- [ ] `EnemyAISystem` processes all enemies each turn after player acts
- [ ] `EnemySpawner` places 3-5 enemies randomly in dungeon rooms (not starting room)

### **Success Criteria**
- [ ] Enemies spawn in random rooms when new game starts
- [ ] Enemies remain idle until player comes within aggro range
- [ ] Once aggroed, enemies chase player through corridors and around corners
- [ ] Enemies attack when adjacent, dealing damage to player
- [ ] Player can fight back and kill enemies (they disappear when health ≤ 0)
- [ ] AI runs smoothly without noticeable delays between turns

### **Simple Pathfinding**
- [ ] Get direction vector toward player (dx, dy)
- [ ] Try to move in primary direction (largest component of dx or dy)
- [ ] If blocked, try secondary direction  
- [ ] If both blocked, try diagonal movement
- [ ] If all blocked, wait (don't get stuck in infinite loops)

## 🔗 Integration Points

**Dependencies**: Map collision detection, player position, combat system
**Events Published**: EnemySpawned, EnemyAggroed, EnemyAttacked
**Events Consumed**: PlayerMoved, TurnStarted

---

**Estimated Effort**: 2-3 days
**Risk Level**: Medium - AI behavior can be unpredictable
**Priority**: High - Core gameplay requires enemies