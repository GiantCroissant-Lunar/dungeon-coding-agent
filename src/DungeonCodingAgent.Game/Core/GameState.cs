namespace DungeonCodingAgent.Game.Core;

/// <summary>
/// Represents the current state of the game for state management and transitions.
/// </summary>
public enum GameState
{
    /// <summary>
    /// Game is in the main menu.
    /// </summary>
    MainMenu,
    
    /// <summary>
    /// Game is actively being played.
    /// </summary>
    Playing,
    
    /// <summary>
    /// Game is paused.
    /// </summary>
    Paused,
    
    /// <summary>
    /// Player is viewing inventory.
    /// </summary>
    Inventory,
    
    /// <summary>
    /// Game has ended (player died or won).
    /// </summary>
    GameOver,
    
    /// <summary>
    /// Game is shutting down.
    /// </summary>
    Exiting
}