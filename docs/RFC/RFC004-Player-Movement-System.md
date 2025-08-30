# RFC004: Player Entity & Movement System

## 📋 Metadata
- **RFC Number**: 004
- **Title**: Player Entity & Movement System
- **Status**: 📝 Draft
- **Author**: System Architect
- **Created**: 2025-08-30
- **Dependencies**: RFC001 (Core Game Loop), RFC003 (Map Generation System)

## 🎯 Objective

Implement the player entity system with turn-based movement mechanics, input handling, collision detection, and action processing that forms the core of player interaction with the dungeon world.

## 📖 Problem Statement

The dungeon crawler needs a robust player system that:
- Creates and manages the player entity using ECS architecture
- Handles keyboard input and translates it to game actions
- Implements turn-based movement with collision detection
- Manages player statistics (health, mana, experience, level)
- Provides smooth input response while maintaining turn-based gameplay
- Integrates with map visibility and camera systems

## 🏗️ Technical Specification

### **Player Entity Components**

#### **Core Player Components**
```csharp
// Player identity and progression
public struct Player
{
    public string Name { get; set; }
    public int Level { get; set; }
    public int Experience { get; set; }
    public int ExperienceToNext { get; set; }
}

// Health and mana management  
public struct Health
{
    public int Current { get; set; }
    public int Maximum { get; set; }
    public bool IsDead => Current <= 0;
}

public struct Mana
{
    public int Current { get; set; }
    public int Maximum { get; set; }
}

// Player-specific attributes
public struct PlayerStats
{
    public int Strength { get; set; }
    public int Dexterity { get; set; }
    public int Intelligence { get; set; }
    public int Constitution { get; set; }
    public int AttackPower { get; set; }
    public int DefenseRating { get; set; }
}
```

#### **Movement and Action Components**
```csharp
public struct MovementIntent
{
    public Direction Direction { get; set; }
    public bool IsProcessed { get; set; }
}

public struct ActionQueue
{
    public List<PlayerAction> PendingActions { get; set; }
    public PlayerAction CurrentAction { get; set; }
    public bool IsProcessing { get; set; }
}

public struct TurnState
{
    public bool HasActed { get; set; }
    public float ActionPoints { get; set; }
    public int TurnNumber { get; set; }
}
```

### **Input System**

#### **Input Manager**
```csharp
public class InputManager
{
    public event Action<PlayerAction> ActionRequested;
    
    public void Initialize()
    public void ProcessKeyInput(Key key, KeyModifiers modifiers)
    public void RegisterKeyBindings()
    public PlayerAction TranslateKeyToAction(Key key, KeyModifiers modifiers)
    public void SetInputMode(InputMode mode) // Normal, Menu, Targeting
}

// Default key mappings
public static class DefaultKeyBindings
{
    // Movement: Arrow keys, WASD, Numpad, Vi keys (hjkl)
    // Actions: Space (wait), Enter (confirm), Escape (cancel)
    // Interface: i (inventory), c (character), ? (help)
    // System: Ctrl+S (save), Ctrl+Q (quit)
}

public enum InputMode
{
    Normal,      // Standard gameplay
    Menu,        // Menu navigation
    Targeting,   // Spell/item targeting
    Text         // Text input (save names, etc.)
}
```

### **Movement System**

#### **Movement Processing**
```csharp
public class MovementSystem : SystemBase<World, float>
{
    public override void Update(World world, float deltaTime)
    
    private bool ValidateMovement(Position from, Position to, GameMap map)
    private void ProcessMovementIntent(EntityReference player, MovementIntent intent)
    private void UpdatePlayerPosition(EntityReference player, Position newPosition)
    private void TriggerMovementEffects(EntityReference player, Position oldPos, Position newPos)
    private void ConsumeActionPoints(EntityReference player, float cost)
}

// Movement validation and pathfinding
public static class MovementUtils
{
    public static bool IsValidMove(Position from, Position to, GameMap map)
    public static float GetMovementCost(Direction direction) // Diagonal = 1.4, Cardinal = 1.0
    public static Direction GetDirectionFromInput(PlayerAction action)
    public static Position ApplyDirection(Position position, Direction direction)
}
```

### **Action Processing**

#### **Player Action System**
```csharp
public class PlayerActionSystem : SystemBase<World, float>
{
    public override void Update(World world, float deltaTime)
    
    private void ProcessAction(EntityReference player, PlayerAction action)
    private void HandleMovementAction(EntityReference player, PlayerAction action)
    private void HandleInteractionAction(EntityReference player, PlayerAction action)
    private void HandleSystemAction(EntityReference player, PlayerAction action)
    private bool CanPerformAction(EntityReference player, PlayerAction action)
}

// Action categories and costs
public static class ActionCosts
{
    public const float Move = 100f;           // Standard move
    public const float MoveDiagonal = 140f;   // Diagonal move (√2 * 100)
    public const float Wait = 100f;           // Wait/rest
    public const float Pickup = 50f;          // Pick up item
    public const float Use = 100f;            // Use item/interact
    public const float Attack = 100f;         // Melee attack
}
```

### **Player Creation and Management**

#### **Player Factory**
```csharp
public static class PlayerFactory
{
    public static EntityReference CreatePlayer(World world, string name, Position startingPosition)
    {
        var player = world.Create(
            new Player { Name = name, Level = 1, Experience = 0 },
            new Position { X = startingPosition.X, Y = startingPosition.Y },
            new Health { Current = 100, Maximum = 100 },
            new Mana { Current = 50, Maximum = 50 },
            new PlayerStats 
            { 
                Strength = 10, Dexterity = 10, Intelligence = 10, 
                Constitution = 10, AttackPower = 5, DefenseRating = 2 
            },
            new Renderable { Character = '@', ForegroundColor = "white", RenderOrder = 100 },
            new TurnState { HasActed = false, ActionPoints = 100f },
            new ActionQueue { PendingActions = new List<PlayerAction>() }
        );
        
        return player;
    }
}
```

## 🎮 Implementation Details

### **Turn-Based Movement Flow**
```csharp
// 1. Input Phase
InputManager receives key press
→ Translates to PlayerAction
→ Adds to player's ActionQueue

// 2. Processing Phase (during game turn)
PlayerActionSystem processes pending actions
→ Validates action legality
→ Checks action point costs
→ Executes movement/interaction
→ Consumes action points
→ Marks turn as completed

// 3. Resolution Phase
MovementSystem updates positions
→ Validates collision with map
→ Triggers visibility updates
→ Fires movement events
→ Updates camera position
```

### **Collision Detection**
```csharp
private bool ValidateMovement(Position from, Position to, GameMap map)
{
    // Check map boundaries
    if (!map.IsInBounds(to.X, to.Y))
        return false;
        
    // Check tile walkability
    if (!map.IsWalkable(to.X, to.Y))
        return false;
        
    // Check for other entities (when implemented)
    if (HasEntityAt(to))
        return false;
        
    return true;
}
```

### **Input Handling with Buffering**
```csharp
public void ProcessKeyInput(Key key, KeyModifiers modifiers)
{
    var action = TranslateKeyToAction(key, modifiers);
    
    if (action != PlayerAction.None)
    {
        // Buffer input for turn-based processing
        if (CanQueueAction(action))
        {
            ActionRequested?.Invoke(action);
        }
        else
        {
            // Provide feedback for invalid actions
            GameEvents.RaiseMessageLogged($"Cannot perform {action} right now", MessageType.Warning);
        }
    }
}
```

### **Stats and Progression**
```csharp
public static class PlayerProgression
{
    public static void GainExperience(EntityReference player, int amount)
    public static bool CheckLevelUp(EntityReference player)
    public static void LevelUp(EntityReference player)
    public static int GetExperienceForLevel(int level)
    public static void UpdateDerivedStats(EntityReference player)
}

// Level progression formula: Level² × 100 + Level × 50
private static int GetExperienceForLevel(int level)
{
    return level * level * 100 + level * 50;
}
```

## ✅ Acceptance Criteria

### **Functional Requirements**
- [ ] Player entity created with starting stats (health: 100, mana: 50, level: 1)
- [ ] Eight-directional movement using arrow keys, WASD, numpad, or vi keys
- [ ] Movement blocked by walls, map boundaries, and other obstacles
- [ ] Turn-based timing: player actions consume appropriate action points
- [ ] Input buffering prevents missed keypresses during processing
- [ ] Player position updates camera and triggers FOV recalculation
- [ ] Wait action (spacebar) allows player to skip turn
- [ ] Invalid movement attempts show helpful feedback messages

### **Input Response Requirements**
- [ ] Keyboard input processed within 16ms (responsive feel)
- [ ] No double-movement from single keypress
- [ ] All standard roguelike key bindings supported
- [ ] Configurable key bindings (stored in settings)
- [ ] Input blocked during non-player turns or menu screens

### **Stats and Progression Requirements**
- [ ] Player stats affect movement and action costs appropriately
- [ ] Experience gain triggers level up at correct thresholds
- [ ] Level up increases health, mana, and core stats
- [ ] Health and mana can be consumed and restored
- [ ] Player death (health ≤ 0) triggers game over state

### **Integration Requirements**
- [ ] Movement integrates smoothly with map visibility system
- [ ] Position changes update camera following logic
- [ ] Player actions fire appropriate events for other systems
- [ ] Compatible with save/load system (all components serializable)

## 🔗 Integration Points

### **Dependencies**
- **RFC001**: Core Game Loop (turn management, ECS World)
- **RFC003**: Map Generation System (collision detection, position validation)

### **Dependents**
- **RFC005**: Combat System (player actions, stats)
- **RFC006**: Inventory System (item interaction)
- **RFC002**: Terminal.Gui Shell (input handling, display updates)

### **Events Published**
```csharp
GameEvents.PlayerCreated           // New player entity
GameEvents.PlayerMoved            // Position changed
GameEvents.PlayerHealthChanged    // Health modified
GameEvents.PlayerManaChanged      // Mana modified
GameEvents.PlayerLeveledUp        // Level increased
GameEvents.PlayerActionPerformed  // Action completed
GameEvents.PlayerDied             // Health reached 0
```

### **Events Consumed**
```csharp
GameEvents.TurnStarted            // Begin processing player actions
GameEvents.NewGameStarted         // Create new player
GameEvents.GameLoaded             // Restore player from save
InputEvents.ActionRequested       // Player input received
```

## 📊 Performance Considerations

### **Input Responsiveness**
- Process input immediately but queue for turn execution
- Provide instant visual feedback for valid inputs
- Use async processing for non-blocking input handling
- Maintain 60 FPS UI updates regardless of turn timing

### **Memory Efficiency**
- Use struct components for cache-friendly ECS access
- Pool action objects to reduce garbage collection
- Minimize string allocations in hot paths
- Cache frequently accessed component references

### **Turn Processing**
- Batch multiple quick actions in single turn when appropriate
- Prioritize player actions over AI for responsiveness
- Use efficient collision detection (spatial partitioning if needed)

## 🧪 Testing Strategy

### **Unit Tests**
```csharp
[Fact]
public void PlayerFactory_WhenCreated_HasCorrectStartingStats()

[Fact]
public void MovementSystem_WhenMoveValid_UpdatesPosition()

[Fact]
public void MovementSystem_WhenMoveBlocked_RemainsInPlace()

[Fact]
public void InputManager_WhenKeyPressed_GeneratesCorrectAction()

[Fact]
public void PlayerProgression_WhenExperienceGained_CalculatesLevelCorrectly()
```

### **Integration Tests**
- Full turn cycle with input → action → movement
- Player movement with camera and FOV updates
- Level progression with stat increases
- Input buffering under load conditions

### **Performance Tests**
- Input latency measurements
- Movement system performance with large maps
- Memory allocation during extended gameplay

## 📝 Implementation Notes

### **Phase 1: Core Player Entity**
1. Implement player creation with basic components
2. Add simple movement validation and position updates
3. Basic input handling for movement actions

### **Phase 2: Advanced Movement**
1. Add diagonal movement and action point system
2. Implement collision detection with map
3. Add wait action and turn state management

### **Phase 3: Stats and Progression**
1. Implement experience gain and level progression
2. Add health/mana management systems
3. Create configurable key binding system

### **Input Design Philosophy**
- Immediate feedback for all inputs (even invalid ones)
- Clear visual indication of turn state
- Forgiving input buffering for rapid players
- Intuitive key mappings following roguelike conventions

## 📚 References

- [Roguelike Input Handling Best Practices](http://roguebasin.roguelikedevelopment.org/index.php/Input_handling)
- [Turn-Based Game Design Patterns](http://gameprogrammingpatterns.com/command.html)
- [ECS Movement System Architecture](https://www.gamedev.net/articles/programming/general-and-gameplay-programming/understanding-component-entity-systems-r3013/)

---

## 🎯 Implementation Ready

This RFC provides complete specification for the player entity and movement system. The implementation provides the foundation for player interaction and serves as a reference for AI entity movement in future RFCs.

**Estimated Effort**: 3-4 days for full implementation including tests
**Risk Level**: Medium - Input handling complexity, performance requirements
**Priority**: Critical - Core gameplay mechanic, blocks other features