using Arch.Core;
using Arch.Core.Utils;
using DungeonCodingAgent.Game.Components;

namespace DungeonCodingAgent.Game.Core;

/// <summary>
/// Central game engine that manages game state, coordinates systems, and handles the main game loop.
/// </summary>
public class GameEngine
{
    private readonly List<IGameSystem> _systems = new();
    private GameState _currentState = GameState.MainMenu;
    private DateTime _lastFrameTime = DateTime.UtcNow;
    private float _totalRealTime = 0f;
    
    /// <summary>
    /// Current game state.
    /// </summary>
    public GameState CurrentState 
    { 
        get => _currentState;
        private set => _currentState = value;
    }
    
    /// <summary>
    /// The ECS world containing all game entities and components.
    /// </summary>
    public World EcsWorld { get; private set; } = null!;
    
    /// <summary>
    /// Whether the game engine is currently running.
    /// </summary>
    public bool IsRunning { get; private set; }
    
    /// <summary>
    /// Turn manager for handling turn-based logic.
    /// </summary>
    public TurnManager TurnManager { get; private set; } = null!;
    
    /// <summary>
    /// Raised when the game state changes.
    /// </summary>
    public event Action<GameState>? StateChanged;
    
    /// <summary>
    /// Raised when a turn is completed.
    /// </summary>
    public event Action<int>? TurnCompleted;
    
    /// <summary>
    /// Initializes the game engine and all core systems.
    /// </summary>
    public void Initialize()
    {
        // Create ECS world
        EcsWorld = World.Create();
        
        // Initialize turn manager
        TurnManager = new TurnManager(EcsWorld);
        
        // Create game session entity with initial time component
        var gameSessionEntity = EcsWorld.Create();
        EcsWorld.Add(gameSessionEntity, new GameTime 
        { 
            Turn = 1, 
            RealTime = 0f 
        });
        EcsWorld.Add(gameSessionEntity, new GameSession
        {
            SaveName = "Default",
            StartTime = DateTime.UtcNow,
            PlayTime = 0
        });
        
        // Subscribe to turn events
        GameEvents.TurnEnded += OnTurnEnded;
        
        IsRunning = true;
    }
    
    /// <summary>
    /// Starts the main game loop.
    /// </summary>
    public async Task RunAsync()
    {
        if (!IsRunning)
        {
            throw new InvalidOperationException("Game engine must be initialized before running.");
        }
        
        _lastFrameTime = DateTime.UtcNow;
        
        while (IsRunning)
        {
            var currentTime = DateTime.UtcNow;
            var deltaTime = (float)(currentTime - _lastFrameTime).TotalSeconds;
            _lastFrameTime = currentTime;
            _totalRealTime += deltaTime;
            
            // Update game time component
            UpdateGameTime(deltaTime);
            
            // Process frame based on current state
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
                    
                case GameState.Inventory:
                    ProcessInventoryFrame();
                    break;
                    
                case GameState.GameOver:
                    ProcessGameOverFrame();
                    break;
                    
                case GameState.Exiting:
                    IsRunning = false;
                    break;
            }
            
            // Run at ~60 FPS for UI responsiveness
            await Task.Delay(16);
        }
    }
    
    /// <summary>
    /// Shuts down the game engine and cleans up resources.
    /// </summary>
    public void Shutdown()
    {
        IsRunning = false;
        
        // Unsubscribe from events
        GameEvents.TurnEnded -= OnTurnEnded;
        
        // Dispose ECS world
        EcsWorld?.Dispose();
    }
    
    /// <summary>
    /// Changes the current game state.
    /// </summary>
    /// <param name="newState">The new state to transition to.</param>
    public void ChangeState(GameState newState)
    {
        if (_currentState == newState)
        {
            return;
        }
        
        var previousState = _currentState;
        _currentState = newState;
        
        // Handle state transitions
        OnStateExit(previousState);
        OnStateEnter(newState);
        
        // Raise events
        StateChanged?.Invoke(newState);
        GameEvents.RaiseGameStateChanged(newState);
    }
    
    /// <summary>
    /// Processes a single game turn when in Playing state.
    /// </summary>
    public void ProcessTurn()
    {
        if (CurrentState != GameState.Playing)
        {
            return;
        }
        
        // Begin turn if not already started
        if (!TurnManager.IsTurnInProgress)
        {
            TurnManager.BeginTurn();
        }
        
        // Process systems in order (placeholder for now - actual systems will be added in other RFCs)
        ProcessGameSystems();
        
        // Check if turn should end (all actors have acted)
        if (TurnManager.GetNextActor() == Entity.Null)
        {
            TurnManager.EndTurn();
        }
    }
    
    /// <summary>
    /// Registers a game system with the engine.
    /// </summary>
    /// <param name="system">The system to register.</param>
    public void RegisterSystem(IGameSystem system)
    {
        if (!_systems.Contains(system))
        {
            _systems.Add(system);
        }
    }
    
    /// <summary>
    /// Unregisters a game system from the engine.
    /// </summary>
    /// <param name="system">The system to unregister.</param>
    public void UnregisterSystem(IGameSystem system)
    {
        _systems.Remove(system);
    }
    
    private void ProcessGameplayFrame()
    {
        // Process turn-based logic
        ProcessTurn();
        
        // Update any real-time systems (UI, rendering, etc.)
        foreach (var system in _systems)
        {
            system.Update(EcsWorld, 0.016f); // ~60 FPS delta
        }
    }
    
    private void ProcessMenuFrame()
    {
        // Process menu-specific systems
        foreach (var system in _systems)
        {
            if (system.ShouldUpdateInState(CurrentState))
            {
                system.Update(EcsWorld, 0.016f);
            }
        }
    }
    
    private void ProcessPausedFrame()
    {
        // Only update UI systems while paused
        foreach (var system in _systems)
        {
            if (system.ShouldUpdateInState(CurrentState))
            {
                system.Update(EcsWorld, 0.016f);
            }
        }
    }
    
    private void ProcessInventoryFrame()
    {
        // Process inventory-specific systems
        foreach (var system in _systems)
        {
            if (system.ShouldUpdateInState(CurrentState))
            {
                system.Update(EcsWorld, 0.016f);
            }
        }
    }
    
    private void ProcessGameOverFrame()
    {
        // Process game over screen systems
        foreach (var system in _systems)
        {
            if (system.ShouldUpdateInState(CurrentState))
            {
                system.Update(EcsWorld, 0.016f);
            }
        }
    }
    
    private void ProcessGameSystems()
    {
        // Execute systems in the order specified by RFC
        // (Actual system implementations will be added in other RFCs)
        foreach (var system in _systems)
        {
            if (system.ShouldUpdateInState(CurrentState))
            {
                system.Update(EcsWorld, 0.016f);
            }
        }
    }
    
    private void OnStateExit(GameState state)
    {
        switch (state)
        {
            case GameState.Playing:
                // Save any in-progress turn state
                break;
            case GameState.Paused:
                GameEvents.RaiseGameResumed();
                break;
        }
    }
    
    private void OnStateEnter(GameState state)
    {
        switch (state)
        {
            case GameState.Paused:
                GameEvents.RaiseGamePaused();
                break;
            case GameState.Playing:
                // Resume or start gameplay
                break;
        }
    }
    
    private void OnTurnEnded(int turnNumber)
    {
        TurnCompleted?.Invoke(turnNumber);
    }
    
    private void UpdateGameTime(float deltaTime)
    {
        EcsWorld.Query(in new QueryDescription().WithAll<GameTime>(), (Entity entity, ref GameTime gameTime) =>
        {
            gameTime.RealTime = _totalRealTime;
            gameTime.Turn = TurnManager.CurrentTurn;
        });
        
        EcsWorld.Query(in new QueryDescription().WithAll<GameSession>(), (Entity entity, ref GameSession session) =>
        {
            session.PlayTime = (int)_totalRealTime;
        });
    }
}

/// <summary>
/// Interface for game systems that can be registered with the GameEngine.
/// </summary>
public interface IGameSystem
{
    /// <summary>
    /// Updates the system with the given world and delta time.
    /// </summary>
    /// <param name="world">The ECS world.</param>
    /// <param name="deltaTime">Time since last update in seconds.</param>
    void Update(World world, float deltaTime);
    
    /// <summary>
    /// Determines if this system should update in the given game state.
    /// </summary>
    /// <param name="state">The current game state.</param>
    /// <returns>True if the system should update, false otherwise.</returns>
    bool ShouldUpdateInState(GameState state);
}