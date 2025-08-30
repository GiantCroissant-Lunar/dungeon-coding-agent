using Arch.Core;

namespace DungeonCodingAgent.Game.Core;

/// <summary>
/// Core game engine that manages game state and coordinates systems
/// </summary>
public class GameEngine
{
    public GameState CurrentState { get; private set; } = GameState.MainMenu;
    public World EcsWorld { get; private set; }
    public bool IsRunning { get; private set; }

    public event Action<GameState>? StateChanged;
    public event Action<int>? TurnCompleted;

    public GameEngine()
    {
        EcsWorld = World.Create();
        IsRunning = false;
    }

    public void Initialize()
    {
        if (IsRunning)
            return;

        IsRunning = true;
        ChangeState(GameState.MainMenu);
    }

    public void Shutdown()
    {
        if (!IsRunning)
            return;

        IsRunning = false;
        ChangeState(GameState.Exiting);
        EcsWorld?.Dispose();
    }

    public void ChangeState(GameState newState)
    {
        if (CurrentState == newState)
            return;

        CurrentState = newState;
        StateChanged?.Invoke(newState);
        GameEvents.RaiseGameStateChanged(newState);
    }

    public void Update(float deltaTime)
    {
        if (!IsRunning)
            return;

        // Basic update logic - will be expanded in future RFCs
        switch (CurrentState)
        {
            case GameState.Playing:
                // Game logic updates would go here
                break;
            case GameState.MainMenu:
                // Menu logic would go here
                break;
            case GameState.Paused:
                // Paused state logic
                break;
            case GameState.Inventory:
                // Inventory state logic
                break;
            case GameState.GameOver:
                // Game over logic
                break;
            case GameState.Exiting:
                IsRunning = false;
                break;
        }
    }
}