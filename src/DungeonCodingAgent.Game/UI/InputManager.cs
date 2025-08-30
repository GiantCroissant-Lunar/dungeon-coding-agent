using Terminal.Gui;
using DungeonCodingAgent.Game.Core;

namespace DungeonCodingAgent.Game.UI;

/// <summary>
/// Handles keyboard input and translates it to game actions
/// </summary>
public class InputManager
{
    public event Action<PlayerAction>? PlayerActionRequested;

    private readonly Dictionary<Key, PlayerAction> _keyMappings;

    public InputManager()
    {
        _keyMappings = new Dictionary<Key, PlayerAction>
        {
            // Movement
            { Key.CursorUp, PlayerAction.MoveNorth },
            { Key.k, PlayerAction.MoveNorth },
            { Key.w, PlayerAction.MoveNorth },
            
            { Key.CursorDown, PlayerAction.MoveSouth },
            { Key.j, PlayerAction.MoveSouth },
            { Key.s, PlayerAction.MoveSouth },
            
            { Key.CursorLeft, PlayerAction.MoveWest },
            { Key.h, PlayerAction.MoveWest },
            { Key.a, PlayerAction.MoveWest },
            
            { Key.CursorRight, PlayerAction.MoveEast },
            { Key.l, PlayerAction.MoveEast },
            { Key.d, PlayerAction.MoveEast },

            // Actions
            { Key.Space, PlayerAction.Wait },
            { Key.g, PlayerAction.Pickup },
            { Key.D, PlayerAction.Drop },
            { Key.i, PlayerAction.Inventory },
            { Key.c, PlayerAction.Character },
            
            // System
            { Key.S, PlayerAction.Save },
            { Key.L, PlayerAction.Load },
            { Key.q, PlayerAction.Quit },
            { Key.Esc, PlayerAction.Quit },
            { Key.F1, PlayerAction.Help }
        };

        RegisterGlobalHotkeys();
    }

    public void ProcessKeyEvent(Key key, bool shift = false, bool ctrl = false, bool alt = false)
    {
        // Handle modified keys
        if (ctrl)
        {
            switch (key)
            {
                case Key.s:
                    OnPlayerActionRequested(PlayerAction.Save);
                    return;
                case Key.l:
                    OnPlayerActionRequested(PlayerAction.Load);
                    return;
                case Key.q:
                    OnPlayerActionRequested(PlayerAction.Quit);
                    return;
            }
        }

        if (shift)
        {
            switch (key)
            {
                case Key.D:
                    OnPlayerActionRequested(PlayerAction.Drop);
                    return;
                case Key.S:
                    OnPlayerActionRequested(PlayerAction.Save);
                    return;
                case Key.L:
                    OnPlayerActionRequested(PlayerAction.Load);
                    return;
            }
        }

        // Handle normal keys
        if (_keyMappings.TryGetValue(key, out var action))
        {
            OnPlayerActionRequested(action);
        }
    }

    public void RegisterGlobalHotkeys()
    {
        // Register global hotkeys that work regardless of focus
        Application.AddKeyBinding(Key.F1, Command.HotKey, () => OnPlayerActionRequested(PlayerAction.Help));
        Application.AddKeyBinding(Key.Esc, Command.QuitToplevel, () => OnPlayerActionRequested(PlayerAction.Quit));
        Application.AddKeyBinding(Key.CtrlMask | Key.q, Command.QuitToplevel, () => OnPlayerActionRequested(PlayerAction.Quit));
    }

    private bool IsGameplayKey(Key key)
    {
        return _keyMappings.ContainsKey(key) || 
               key == Key.F1 || 
               key == Key.Esc ||
               key == (Key.CtrlMask | Key.q) ||
               key == (Key.CtrlMask | Key.s) ||
               key == (Key.CtrlMask | Key.l);
    }

    private void OnPlayerActionRequested(PlayerAction action)
    {
        PlayerActionRequested?.Invoke(action);
        InputEvents.RaisePlayerActionRequested(action);
    }

    public string GetKeyMappingHelp()
    {
        return @"
MOVEMENT:
  Arrow Keys, WASD, or hjkl - Move in four directions
  
ACTIONS:
  Space - Wait/Rest
  g - Pick up item
  Shift+D - Drop item
  i - Open inventory
  c - Character sheet

SYSTEM:
  Ctrl+S or Shift+S - Save game
  Ctrl+L or Shift+L - Load game
  Ctrl+Q, Esc, or q - Quit
  F1 - Help";
    }
}