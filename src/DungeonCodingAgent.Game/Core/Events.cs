namespace DungeonCodingAgent.Game.Core;

/// <summary>
/// Static class for managing and publishing game events
/// </summary>
public static class GameEvents
{
    public static event Action<GameState>? GameStateChanged;
    public static event Action? PlayerMoved;
    public static event Action<string>? MessageLogged;
    public static event Action? PlayerStatsChanged;
    public static event Action? MapChanged;

    public static void RaiseGameStateChanged(GameState newState)
    {
        GameStateChanged?.Invoke(newState);
    }

    public static void RaisePlayerMoved()
    {
        PlayerMoved?.Invoke();
    }

    public static void RaiseMessageLogged(string message)
    {
        MessageLogged?.Invoke(message);
    }

    public static void RaisePlayerStatsChanged()
    {
        PlayerStatsChanged?.Invoke();
    }

    public static void RaiseMapChanged()
    {
        MapChanged?.Invoke();
    }
}

/// <summary>
/// Static class for managing input events
/// </summary>
public static class InputEvents
{
    public static event Action<PlayerAction>? PlayerActionRequested;

    public static void RaisePlayerActionRequested(PlayerAction action)
    {
        PlayerActionRequested?.Invoke(action);
    }
}

/// <summary>
/// Static class for managing UI events
/// </summary>
public static class UIEvents
{
    public static event Action<GameState>? StateChangeRequested;
    public static event Action? WindowResized;

    public static void RaiseStateChangeRequested(GameState newState)
    {
        StateChangeRequested?.Invoke(newState);
    }

    public static void RaiseWindowResized()
    {
        WindowResized?.Invoke();
    }
}

/// <summary>
/// Player actions that can be requested from input
/// </summary>
public enum PlayerAction
{
    MoveNorth,
    MoveSouth,
    MoveEast,
    MoveWest,
    Wait,
    Pickup,
    Drop,
    Inventory,
    Character,
    Save,
    Load,
    Quit,
    Help
}