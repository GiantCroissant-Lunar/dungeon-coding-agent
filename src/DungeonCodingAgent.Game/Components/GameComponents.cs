namespace DungeonCodingAgent.Game.Components;

/// <summary>
/// Component representing game timing information.
/// </summary>
public struct GameTime
{
    /// <summary>
    /// Current turn number in the game.
    /// </summary>
    public int Turn { get; set; }
    
    /// <summary>
    /// Real-time elapsed since game start in seconds.
    /// </summary>
    public float RealTime { get; set; }
}

/// <summary>
/// Component representing an actor's turn information.
/// </summary>
public struct ActorTurn
{
    /// <summary>
    /// Initiative value determining turn order (higher goes first).
    /// </summary>
    public int Initiative { get; set; }
    
    /// <summary>
    /// Whether this actor has acted in the current turn.
    /// </summary>
    public bool HasActed { get; set; }
    
    /// <summary>
    /// Action points available for this turn.
    /// </summary>
    public float ActionPoints { get; set; }
}

/// <summary>
/// Component representing game session metadata.
/// </summary>
public struct GameSession
{
    /// <summary>
    /// Name of the saved game.
    /// </summary>
    public string SaveName { get; set; }
    
    /// <summary>
    /// When this game session was started.
    /// </summary>
    public DateTime StartTime { get; set; }
    
    /// <summary>
    /// Total play time in seconds.
    /// </summary>
    public int PlayTime { get; set; }
}