using Arch.Core;

namespace DungeonCodingAgent.Game.Core;

/// <summary>
/// Central event system for inter-system communication.
/// Provides a decoupled way for systems to communicate state changes and game events.
/// </summary>
public static class GameEvents
{
    // Core game state events
    
    /// <summary>
    /// Raised when the game state changes (menu, playing, paused, etc.).
    /// </summary>
    public static event Action<GameState>? GameStateChanged;
    
    /// <summary>
    /// Raised at the beginning of each game turn.
    /// </summary>
    public static event Action<int>? TurnStarted;
    
    /// <summary>
    /// Raised at the end of each game turn.
    /// </summary>
    public static event Action<int>? TurnEnded;
    
    // Game lifecycle events
    
    /// <summary>
    /// Raised when the game is paused.
    /// </summary>
    public static event Action? GamePaused;
    
    /// <summary>
    /// Raised when the game is resumed from pause.
    /// </summary>
    public static event Action? GameResumed;
    
    // Entity lifecycle events
    
    /// <summary>
    /// Raised when an entity is created in the game world.
    /// </summary>
    public static event Action<Entity>? EntityCreated;
    
    /// <summary>
    /// Raised when an entity is destroyed in the game world.
    /// </summary>
    public static event Action<Entity>? EntityDestroyed;
    
    // Event raising methods
    
    /// <summary>
    /// Raises the GameStateChanged event.
    /// </summary>
    /// <param name="newState">The new game state.</param>
    public static void RaiseGameStateChanged(GameState newState)
    {
        GameStateChanged?.Invoke(newState);
    }
    
    /// <summary>
    /// Raises the TurnStarted event.
    /// </summary>
    /// <param name="turnNumber">The current turn number.</param>
    public static void RaiseTurnStarted(int turnNumber)
    {
        TurnStarted?.Invoke(turnNumber);
    }
    
    /// <summary>
    /// Raises the TurnEnded event.
    /// </summary>
    /// <param name="turnNumber">The completed turn number.</param>
    public static void RaiseTurnEnded(int turnNumber)
    {
        TurnEnded?.Invoke(turnNumber);
    }
    
    /// <summary>
    /// Raises the GamePaused event.
    /// </summary>
    public static void RaiseGamePaused()
    {
        GamePaused?.Invoke();
    }
    
    /// <summary>
    /// Raises the GameResumed event.
    /// </summary>
    public static void RaiseGameResumed()
    {
        GameResumed?.Invoke();
    }
    
    /// <summary>
    /// Raises the EntityCreated event.
    /// </summary>
    /// <param name="entity">The created entity.</param>
    public static void RaiseEntityCreated(Entity entity)
    {
        EntityCreated?.Invoke(entity);
    }
    
    /// <summary>
    /// Raises the EntityDestroyed event.
    /// </summary>
    /// <param name="entity">The destroyed entity.</param>
    public static void RaiseEntityDestroyed(Entity entity)
    {
        EntityDestroyed?.Invoke(entity);
    }
}