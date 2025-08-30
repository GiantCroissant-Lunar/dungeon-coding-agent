using Arch.Core;

namespace DungeonCodingAgent.Game.Core;

/// <summary>
/// Static event system for inter-system communication in the game.
/// Provides loose coupling between game systems through event publishing/subscribing.
/// </summary>
public static class GameEvents
{
    // Core game events
    public static event Action<GameState>? GameStateChanged;
    public static event Action<int>? TurnStarted;
    public static event Action<int>? TurnEnded;
    
    // Entity events  
    public static event Action<Entity>? EntityCreated;
    public static event Action<Entity>? EntityDestroyed;
    
    // Game state events
    public static event Action? GamePaused;
    public static event Action? GameResumed;
    
    // Save/Load events (RFC008)
    public static event Action? SaveRequested;
    public static event Action? LoadRequested;
    public static event Action<string>? GameSaved;
    public static event Action<string>? GameLoaded;
    public static event Action<string>? SaveLoadError;
    
    // Player events
    public static event Action<Entity>? PlayerCreated;
    public static event Action<Entity, Position>? PlayerMoved;
    public static event Action<Entity, int>? PlayerHealthChanged;
    public static event Action<Entity, int>? PlayerManaChanged;
    public static event Action<Entity>? PlayerLeveledUp;
    public static event Action<Entity>? PlayerDied;
    
    // Map events
    public static event Action? MapChanged;
    public static event Action<string>? MessageLogged;

    // Event raising methods for save/load system
    public static void RaiseSaveRequested() => SaveRequested?.Invoke();
    public static void RaiseLoadRequested() => LoadRequested?.Invoke();
    public static void RaiseGameSaved(string saveFile) => GameSaved?.Invoke(saveFile);
    public static void RaiseGameLoaded(string saveFile) => GameLoaded?.Invoke(saveFile);
    public static void RaiseSaveLoadError(string error) => SaveLoadError?.Invoke(error);
    
    // Event raising methods for other systems
    public static void RaiseGameStateChanged(GameState newState) => GameStateChanged?.Invoke(newState);
    public static void RaiseTurnStarted(int turnNumber) => TurnStarted?.Invoke(turnNumber);
    public static void RaiseTurnEnded(int turnNumber) => TurnEnded?.Invoke(turnNumber);
    public static void RaiseEntityCreated(Entity entity) => EntityCreated?.Invoke(entity);
    public static void RaiseEntityDestroyed(Entity entity) => EntityDestroyed?.Invoke(entity);
    public static void RaiseGamePaused() => GamePaused?.Invoke();
    public static void RaiseGameResumed() => GameResumed?.Invoke();
    public static void RaisePlayerCreated(Entity player) => PlayerCreated?.Invoke(player);
    public static void RaisePlayerMoved(Entity player, Position position) => PlayerMoved?.Invoke(player, position);
    public static void RaisePlayerHealthChanged(Entity player, int newHealth) => PlayerHealthChanged?.Invoke(player, newHealth);
    public static void RaisePlayerManaChanged(Entity player, int newMana) => PlayerManaChanged?.Invoke(player, newMana);
    public static void RaisePlayerLeveledUp(Entity player) => PlayerLeveledUp?.Invoke(player);
    public static void RaisePlayerDied(Entity player) => PlayerDied?.Invoke(player);
    public static void RaiseMapChanged() => MapChanged?.Invoke();
    public static void RaiseMessageLogged(string message) => MessageLogged?.Invoke(message);
}

/// <summary>
/// Game state enumeration for state management
/// </summary>
public enum GameState
{
    MainMenu,
    Playing,
    Paused,
    GameOver,
    Saving,
    Loading
}

/// <summary>
/// Position component for entity positioning
/// </summary>
public struct Position
{
    public int X { get; set; }
    public int Y { get; set; }
    
    public Position(int x, int y)
    {
        X = x;
        Y = y;
    }
}