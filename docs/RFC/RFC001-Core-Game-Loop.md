# RFC001: Core Game Loop & State Management

## 📋 Metadata
- **RFC Number**: 001
- **Title**: Core Game Loop & State Management
- **Status**: ✅ Complete
- **Author**: System Architect
- **Created**: 2025-08-30
- **Dependencies**: None (foundational system)

## 🎯 Objective

Implement the core game loop and state management system that coordinates all game systems, handles turn-based logic, and manages the overall game lifecycle.

## 📖 Problem Statement

A rogue-like game requires a central coordinator that:
- Manages game state transitions (main menu, playing, paused, game over)
- Implements turn-based timing and action resolution
- Coordinates ECS systems execution order
- Handles save/load game state
- Provides event system for inter-system communication

## 🏗️ Technical Specification

### **Core Components**

#### **GameState Enum**
```csharp
public enum GameState
{
    MainMenu,
    Playing,
    Paused,
    Inventory,
    GameOver,
    Exiting
}
```

#### **GameEngine Class**
```csharp
public class GameEngine
{
    public GameState CurrentState { get; private set; }
    public World EcsWorld { get; private set; }
    public bool IsRunning { get; private set; }
    
    public event Action<GameState> StateChanged;
    public event Action<int> TurnCompleted;
    
    public void Initialize();
    public void Run();
    public void Shutdown();
    public void ChangeState(GameState newState);
    public void ProcessTurn();
}
```

#### **Turn Management**
```csharp
public class TurnManager
{
    public int CurrentTurn { get; private set; }
    public EntityId CurrentActor { get; private set; }
    
    public void BeginTurn();
    public void EndTurn();
    public EntityId GetNextActor();
    public bool CanActorAct(EntityId actor);
}
```

### **System Architecture**

#### **System Execution Order**
1. **Input System**: Process player commands
2. **AI System**: Process AI decisions  
3. **Action System**: Execute queued actions
4. **Movement System**: Process movement
5. **Combat System**: Resolve combat
6. **Physics System**: Handle collisions
7. **UI Update System**: Refresh display
8. **Render System**: Draw to terminal

#### **Event System**
```csharp
public static class GameEvents
{
    // Core game events
    public static event Action<GameState> GameStateChanged;
    public static event Action<int> TurnStarted;
    public static event Action<int> TurnEnded;
    
    // Entity events  
    public static event Action<EntityId> EntityCreated;
    public static event Action<EntityId> EntityDestroyed;
    
    // Game state events
    public static event Action GamePaused;
    public static event Action GameResumed;
}
```

### **ECS Integration**

#### **Core Game Components**
```csharp
public struct GameTime
{
    public int Turn { get; set; }
    public float RealTime { get; set; }
}

public struct ActorTurn
{
    public int Initiative { get; set; }
    public bool HasActed { get; set; }
    public float ActionPoints { get; set; }
}

public struct GameSession
{
    public string SaveName { get; set; }
    public DateTime StartTime { get; set; }
    public int PlayTime { get; set; } // seconds
}
```

## 🎮 Implementation Details

### **Game Loop Structure**
```csharp
public async Task RunGameLoop()
{
    while (IsRunning)
    {
        switch (CurrentState)
        {
            case GameState.Playing:
                ProcessGameplayFrame();
                break;
                
            case GameState.MainMenu:
                ProcessMenuFrame();
                break;
                
            case GameState.Paused:
                ProcessPausedFrame();
                break;
        }
        
        await Task.Delay(16); // ~60 FPS for UI responsiveness
    }
}

private void ProcessGameplayFrame()
{
    // Process one game turn
    if (turnManager.ShouldProcessTurn())
    {
        ProcessTurn();
    }
    
    // Update UI regardless of turn state
    uiSystem.Update();
    renderSystem.Render();
}
```

### **Turn-Based Logic**
```csharp
public void ProcessTurn()
{
    GameEvents.RaiseTurnStarted(CurrentTurn);
    
    // Process all systems in order
    inputSystem.Process();
    aiSystem.Process();
    actionSystem.Process();
    movementSystem.Process();
    combatSystem.Process();
    
    turnManager.EndTurn();
    GameEvents.RaiseTurnEnded(CurrentTurn);
}
```

### **State Management**
```csharp
public void ChangeState(GameState newState)
{
    var previousState = CurrentState;
    CurrentState = newState;
    
    // Handle state transitions
    OnStateExit(previousState);
    OnStateEnter(newState);
    
    StateChanged?.Invoke(newState);
    GameEvents.RaiseGameStateChanged(newState);
}
```

## ✅ Acceptance Criteria

### **Functional Requirements**
- [x] Game loop runs at consistent rate (targeting ~60 FPS for UI)
- [x] Turn-based logic processes player and AI actions sequentially
- [x] State transitions work correctly (menu ↔ playing ↔ paused)
- [x] ECS systems execute in correct order
- [x] Event system broadcasts game state changes
- [x] Game can be paused and resumed
- [ ] Graceful shutdown saves game state

### **Technical Requirements**
- [x] All game state is managed through ECS components
- [x] Turn manager handles initiative and action points correctly
- [x] Event system uses weak references to prevent memory leaks
- [x] Game loop is responsive to user input
- [x] System execution order is configurable
- [ ] Game state persists between sessions

### **Testing Requirements**
- [x] Unit tests for GameEngine state transitions
- [x] Unit tests for TurnManager turn progression
- [x] Integration tests for system execution order
- [x] Performance tests for game loop timing
- [x] Mock tests for event system reliability

### **Integration Requirements**
- [ ] Integrates with Terminal.Gui application lifecycle
- [x] Provides hooks for UI system updates
- [ ] Supports save/load system integration (RFC008)
- [x] Compatible with all other game systems

## 🔗 Integration Points

### **Dependencies**
- **Arch ECS**: Core entity-component-system framework
- **Terminal.Gui**: Application lifecycle and UI updates

### **Dependents**
- **RFC002**: Terminal.Gui Application Shell (uses GameEngine)
- **RFC004**: Player Movement System (consumes turn events)
- **RFC005**: Combat System (processes during turn resolution)
- **RFC008**: Save/Load System (persists game state)

### **Events Published**
```csharp
GameEvents.GameStateChanged    // State transitions
GameEvents.TurnStarted        // Beginning of turn
GameEvents.TurnEnded          // End of turn  
GameEvents.GamePaused         // Game paused
GameEvents.GameResumed        // Game resumed
```

### **Events Consumed**
```csharp
PlayerInput.ActionRequested   // From input system
UIEvents.StateChangeRequested // From UI system
SaveSystem.SaveRequested      // From save system
```

## 📊 Performance Considerations

### **Optimization Targets**
- Game loop maintains 60 FPS for UI responsiveness
- Turn processing completes within 100ms
- State transitions happen within 50ms
- Memory allocation minimal during gameplay

### **Resource Management**
- ECS world cleanup on state transitions
- Event handler weak references
- System pooling for frequently created objects

## 🧪 Testing Strategy

### **Unit Tests**
```csharp
[Fact]
public void GameEngine_WhenStateChanges_RaisesStateChangedEvent()

[Fact] 
public void TurnManager_WhenTurnEnds_AdvancesToNextActor()

[Fact]
public void GameLoop_WhenPaused_StopsProcessingTurns()
```

### **Integration Tests**
- Full game loop with mock systems
- State persistence across pause/resume cycles
- Event propagation through system chain

### **Performance Tests**
- Game loop timing consistency
- Memory usage during extended gameplay
- Event system performance under load

## 📝 Implementation Notes

### **Phase 1: Basic Game Loop**
1. Implement GameEngine with basic state management
2. Create simple turn-based timing
3. Set up event system foundation

### **Phase 2: ECS Integration**
1. Integrate Arch ECS world management
2. Implement system execution pipeline
3. Add turn-based component handling

### **Phase 3: Advanced Features**
1. Add save/load state hooks
2. Implement performance monitoring
3. Add configuration system for game timing

## 📚 References

- [Game Programming Patterns - Game Loop](http://gameprogrammingpatterns.com/game-loop.html)
- [Arch ECS Documentation](https://github.com/genaray/Arch)
- [Terminal.Gui Application Model](https://gui-cs.github.io/Terminal.Gui/)

---

## 🎯 Implementation Ready

This RFC provides complete specification for the core game loop and state management system. The implementation can proceed independently and will serve as the foundation for all other game systems.

**Estimated Effort**: 2-3 days for full implementation including tests
**Risk Level**: Low - well-established patterns
**Priority**: Critical - foundational system required by others