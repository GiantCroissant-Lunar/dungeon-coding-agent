# RFC005: Combat Resolution Engine

## 📋 Metadata
- **RFC Number**: 005
- **Title**: Combat Resolution Engine
- **Status**: 📝 Draft
- **Author**: System Architect
- **Created**: 2025-08-30
- **Dependencies**: RFC001 (Core Game Loop), RFC004 (Player Movement System)

## 🎯 Objective

Implement a turn-based combat system with melee attacks, damage calculation, enemy AI, and tactical positioning that provides engaging rogue-like combat mechanics.

## 📖 Problem Statement

The dungeon crawler needs a combat system that:
- Provides strategic turn-based combat with positioning importance
- Implements classic rogue-like mechanics (to-hit rolls, damage ranges, armor)
- Supports both player and AI-driven entity combat
- Handles melee combat with potential for ranged attacks
- Integrates with health/stats systems and provides combat feedback
- Creates tactical depth through terrain and positioning

## 🏗️ Technical Specification

### **Combat Components**

#### **Combat-Related Components**
```csharp
public struct CombatStats
{
    public int AttackPower { get; set; }      // Base attack damage
    public int DefenseRating { get; set; }   // Armor/defense value
    public int Accuracy { get; set; }        // To-hit bonus
    public int Evasion { get; set; }         // Dodge chance
    public int CriticalChance { get; set; }  // % chance for critical hit
    public float AttackSpeed { get; set; }   // Actions per turn modifier
}

public struct Weapon
{
    public string Name { get; set; }
    public int MinDamage { get; set; }
    public int MaxDamage { get; set; }
    public int Accuracy { get; set; }
    public int CriticalChance { get; set; }
    public WeaponType Type { get; set; }
    public int Range { get; set; }           // 1 for melee, >1 for ranged
}

public struct Armor
{
    public string Name { get; set; }
    public int DefenseValue { get; set; }
    public int EvasionPenalty { get; set; }  // Heavy armor reduces dodge
    public ArmorType Type { get; set; }
    public List<DamageType> Resistances { get; set; }
}

public struct CombatTarget
{
    public EntityReference Target { get; set; }
    public Position TargetPosition { get; set; }
    public bool IsValidTarget { get; set; }
}
```

#### **Combat State Components**
```csharp
public struct CombatAction
{
    public CombatActionType Type { get; set; }
    public EntityReference Attacker { get; set; }
    public EntityReference Target { get; set; }
    public bool IsProcessed { get; set; }
    public float ActionCost { get; set; }
}

public struct DeathFlag
{
    public bool IsDead { get; set; }
    public string DeathMessage { get; set; }
    public int ExperienceReward { get; set; }
}

public enum CombatActionType
{
    MeleeAttack,
    RangedAttack,
    Block,
    Dodge,
    UseItem,
    CastSpell
}
```

### **Enemy AI Components**

#### **Basic AI Behavior**
```csharp
public struct AIBehavior
{
    public AIType Type { get; set; }
    public float AggroRange { get; set; }     // Detection distance
    public EntityReference Target { get; set; }
    public AIState CurrentState { get; set; }
    public int TurnsSinceLastAction { get; set; }
}

public enum AIType
{
    Aggressive,    // Always attacks when player in range
    Defensive,     // Blocks and waits for player to approach
    Cowardly,      // Flees when health is low
    Patrolling,    // Moves in patterns, attacks when sees player
    Sleeping       // Inactive until player gets very close
}

public enum AIState
{
    Idle,
    Patrolling,
    Chasing,
    Attacking,
    Fleeing,
    Dead
}
```

### **Combat System Architecture**

#### **Combat Resolution System**
```csharp
public class CombatSystem : SystemBase<World, float>
{
    public override void Update(World world, float deltaTime)
    
    private void ProcessCombatActions(World world)
    private CombatResult ResolveMeleeAttack(EntityReference attacker, EntityReference target)
    private bool CalculateHit(CombatStats attackerStats, CombatStats defenderStats)
    private int CalculateDamage(CombatStats attackerStats, Weapon weapon, bool isCritical)
    private void ApplyDamage(EntityReference target, int damage, DamageType type)
    private void HandleEntityDeath(EntityReference entity, EntityReference killer)
}

public struct CombatResult
{
    public bool Hit { get; set; }
    public bool Critical { get; set; }
    public int Damage { get; set; }
    public bool TargetDied { get; set; }
    public string Message { get; set; }
}
```

#### **AI System**
```csharp
public class AISystem : SystemBase<World, float>
{
    public override void Update(World world, float deltaTime)
    
    private void ProcessAIBehavior(EntityReference entity, AIBehavior behavior)
    private PlayerAction DecideAction(EntityReference entity, AIBehavior behavior)
    private bool CanSeeTarget(EntityReference entity, EntityReference target)
    private List<EntityReference> FindNearbyEnemies(EntityReference entity, float range)
    private Direction GetDirectionToTarget(Position from, Position to)
    private void UpdateAIState(EntityReference entity, AIBehavior behavior)
}
```

### **Combat Mechanics**

#### **Hit Calculation**
```csharp
private bool CalculateHit(CombatStats attacker, CombatStats defender)
{
    // Base hit chance: 50% + (attacker.Accuracy - defender.Evasion)
    int hitChance = 50 + attacker.Accuracy - defender.Evasion;
    
    // Clamp between 5% and 95% (always some chance of hit/miss)
    hitChance = Math.Clamp(hitChance, 5, 95);
    
    return Random.Shared.Next(1, 101) <= hitChance;
}
```

#### **Damage Calculation**
```csharp
private int CalculateDamage(CombatStats attackerStats, Weapon weapon, bool isCritical)
{
    // Roll damage in weapon range
    int baseDamage = Random.Shared.Next(weapon.MinDamage, weapon.MaxDamage + 1);
    
    // Add attacker's attack power
    int totalDamage = baseDamage + attackerStats.AttackPower;
    
    // Critical hit doubles damage
    if (isCritical)
        totalDamage *= 2;
        
    return Math.Max(1, totalDamage); // Minimum 1 damage
}

private void ApplyDamage(EntityReference target, int damage, CombatStats defenderStats)
{
    // Armor reduces damage
    int finalDamage = Math.Max(1, damage - defenderStats.DefenseRating);
    
    // Apply to health
    var health = world.Get<Health>(target);
    health.Current = Math.Max(0, health.Current - finalDamage);
    world.Set(target, health);
    
    // Check for death
    if (health.Current <= 0)
    {
        world.Add<DeathFlag>(target, new DeathFlag { IsDead = true });
    }
}
```

### **Enemy Creation and Management**

#### **Enemy Factory**
```csharp
public static class EnemyFactory
{
    public static EntityReference CreateGoblin(World world, Position position)
    {
        return world.Create(
            new Position { X = position.X, Y = position.Y },
            new Health { Current = 20, Maximum = 20 },
            new CombatStats 
            { 
                AttackPower = 3, DefenseRating = 1, Accuracy = 5, 
                Evasion = 2, CriticalChance = 5, AttackSpeed = 1.0f 
            },
            new Weapon 
            { 
                Name = "Rusty Dagger", MinDamage = 1, MaxDamage = 4, 
                Accuracy = 0, CriticalChance = 5, Type = WeaponType.Dagger, Range = 1 
            },
            new AIBehavior 
            { 
                Type = AIType.Aggressive, AggroRange = 6f, 
                CurrentState = AIState.Patrolling 
            },
            new Renderable { Character = 'g', ForegroundColor = "green", RenderOrder = 50 },
            new ActorTurn { Initiative = Random.Shared.Next(1, 20), MaxActionPoints = 100f }
        );
    }
    
    // Additional enemy types: Orc, Skeleton, Troll, etc.
}

// Enemy difficulty scaling
public static class EnemyScaling
{
    public static void ScaleEnemyForLevel(EntityReference enemy, int dungeonLevel)
    {
        var health = world.Get<Health>(enemy);
        var combat = world.Get<CombatStats>(enemy);
        
        // Scale health and combat stats based on dungeon depth
        float multiplier = 1.0f + (dungeonLevel - 1) * 0.15f;
        
        health.Maximum = (int)(health.Maximum * multiplier);
        health.Current = health.Maximum;
        combat.AttackPower = (int)(combat.AttackPower * multiplier);
        
        world.Set(enemy, health);
        world.Set(enemy, combat);
    }
}
```

## 🎮 Implementation Details

### **Combat Turn Flow**
```csharp
// 1. Initiative Phase (handled by TurnManager from RFC001)
// Sort entities by initiative + random tie-breaking

// 2. Action Selection Phase
// Player: Input → CombatAction
// AI: Decision making → CombatAction

// 3. Action Resolution Phase
foreach (var entity in turnOrder)
{
    if (HasCombatAction(entity))
    {
        var result = ResolveCombatAction(entity);
        DisplayCombatMessage(result);
        
        if (result.TargetDied)
        {
            HandleDeath(result.Target, entity);
        }
    }
}

// 4. Cleanup Phase
// Remove dead entities, gain experience, update UI
```

### **AI Decision Making**
```csharp
private PlayerAction DecideAction(EntityReference entity, AIBehavior behavior)
{
    switch (behavior.CurrentState)
    {
        case AIState.Idle:
            // Look for targets, start patrolling
            return FindTargetOrPatrol(entity, behavior);
            
        case AIState.Chasing:
            // Move toward target or attack if adjacent
            return ChaseTarget(entity, behavior.Target);
            
        case AIState.Attacking:
            // Attack target if in range
            return AttackIfPossible(entity, behavior.Target);
            
        case AIState.Fleeing:
            // Move away from threats
            return FleeFromDanger(entity);
            
        default:
            return PlayerAction.Wait;
    }
}
```

### **Combat Messages**
```csharp
private void GenerateCombatMessage(CombatResult result, string attackerName, string targetName)
{
    string message;
    
    if (!result.Hit)
    {
        message = $"{attackerName} misses {targetName}.";
    }
    else if (result.Critical)
    {
        message = $"{attackerName} critically hits {targetName} for {result.Damage} damage!";
    }
    else
    {
        message = $"{attackerName} hits {targetName} for {result.Damage} damage.";
    }
    
    if (result.TargetDied)
    {
        message += $" {targetName} dies!";
    }
    
    GameEvents.RaiseMessageLogged(message, MessageType.Combat);
}
```

## ✅ Acceptance Criteria

### **Functional Requirements**
- [ ] Player can attack adjacent enemies with melee weapons
- [ ] Hit chance calculated based on attacker accuracy vs defender evasion
- [ ] Damage rolls within weapon range, modified by stats and armor
- [ ] Critical hits occur at appropriate rates and deal double damage
- [ ] Entity death removes entity and grants experience to killer
- [ ] AI enemies detect player within aggro range and pursue/attack
- [ ] Combat messages clearly describe all actions and results
- [ ] Turn order respects initiative values

### **Combat Balance Requirements**
- [ ] Player starting stats allow reasonable chance to hit/avoid early enemies
- [ ] Enemy difficulty scales appropriately with dungeon depth
- [ ] Combat feels tactical with positioning and timing importance
- [ ] Death occurs at reasonable rate (not too easy or too hard)
- [ ] Critical hits add excitement without dominating combat

### **AI Behavior Requirements**
- [ ] Aggressive enemies actively seek and attack player
- [ ] Defensive enemies respond to threats but don't initiate
- [ ] Cowardly enemies flee when health drops below threshold
- [ ] AI makes reasonable tactical decisions (not obviously stupid)
- [ ] AI respects turn-based timing and action point costs

### **Integration Requirements**
- [ ] Combat integrates with health/mana systems from RFC004
- [ ] Experience gain triggers level progression
- [ ] Combat actions respect turn-based timing from RFC001
- [ ] Messages appear in UI message log
- [ ] Dead entities are properly cleaned up

## 🔗 Integration Points

### **Dependencies**
- **RFC001**: Core Game Loop (turn management, ECS systems)
- **RFC004**: Player Movement System (stats, health, positioning)

### **Dependents**
- **RFC006**: Inventory System (weapons, armor, combat items)
- **RFC007**: UI Panel Components (health bars, combat log)

### **Events Published**
```csharp
GameEvents.CombatStarted          // Combat initiated
GameEvents.AttackResolved         // Attack hit/missed
GameEvents.EntityDied             // Entity health reached 0
GameEvents.ExperienceGained       // Player gained XP
GameEvents.CombatEnded            // All combat resolved
```

### **Events Consumed**
```csharp
GameEvents.PlayerActionRequested  // Player attack commands
GameEvents.TurnStarted           // Process AI decisions
GameEvents.EntityMoved           // Update AI target tracking
```

## 📊 Performance Considerations

### **AI Performance**
- Limit AI calculations to entities within player vision range
- Cache pathfinding results for multiple turns
- Use simple heuristics rather than complex planning algorithms
- Process AI in batches rather than individually

### **Combat Calculations**
- Pre-calculate hit/damage tables where possible
- Use integer math for performance-critical calculations
- Pool damage calculation objects to reduce allocations
- Batch similar calculations together

### **Entity Management**
- Efficient cleanup of dead entities
- Spatial partitioning for proximity queries
- Limit active enemy count based on performance

## 🧪 Testing Strategy

### **Unit Tests**
```csharp
[Fact]
public void CombatSystem_WhenAttackHits_DamageAppliedCorrectly()

[Fact]
public void CombatSystem_WhenEntityDies_CorrectExperienceAwarded()

[Fact]
public void AISystem_WhenPlayerInRange_SwitchesToChasing()

[Fact]
public void HitCalculation_WithEqualStats_Returns50PercentChance()

[Fact]
public void DamageCalculation_WithCritical_DoublesBaseDamage()
```

### **Integration Tests**
- Full combat sequence from attack to experience gain
- AI behavior transitions in various scenarios
- Combat with multiple enemies and targeting
- Performance with large numbers of entities

### **Balance Testing**
- Player survival rates against different enemy types
- Combat duration and engagement quality
- AI behavior effectiveness and believability

## 📝 Implementation Notes

### **Phase 1: Basic Combat**
1. Implement core combat resolution (hit, damage, death)
2. Add basic player melee attack functionality
3. Simple enemy creation and health management

### **Phase 2: AI System**
1. Implement basic aggressive AI behavior
2. Add target detection and chasing logic
3. Create variety of enemy types with different stats

### **Phase 3: Advanced Combat**
1. Add critical hits, armor, and advanced damage types
2. Implement different AI personality types
3. Add tactical depth with positioning bonuses

### **Combat Design Philosophy**
- Clear cause-and-effect for all combat actions
- Randomness adds excitement without removing player agency
- Tactical depth through positioning and resource management
- Readable combat log helps players understand mechanics

## 📚 References

- [Roguelike Combat Systems](http://roguebasin.roguelikedevelopment.org/index.php/Articles#Combat)
- [AI for Turn-Based Games](https://www.gamedeveloper.com/programming/designing-ai-for-turn-based-games)
- [Game Balance in RPG Combat](https://www.gdcvault.com/play/1015848/Math-for-Game-Programmers-Balancing)

---

## 🎯 Implementation Ready

This RFC provides complete specification for the combat resolution engine with AI opponents. The implementation creates the foundation for engaging tactical combat while maintaining the classic rogue-like feel.

**Estimated Effort**: 4-5 days for full implementation including tests
**Risk Level**: High - Complex AI behavior, game balance critical
**Priority**: High - Core gameplay mechanic, defines game difficulty curve