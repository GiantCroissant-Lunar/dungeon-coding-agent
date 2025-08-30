using Terminal.Gui;
using DungeonCodingAgent.Game.Core;

namespace DungeonCodingAgent.Game.UI.Views;

/// <summary>
/// Menu bar that provides access to game functions
/// </summary>
public class MenuBarView : MenuBar
{
    public event Action? NewGameRequested;
    public event Action? SaveGameRequested;
    public event Action? LoadGameRequested;
    public event Action? QuitRequested;
    public event Action? HelpRequested;
    public event Action? AboutRequested;

    public MenuBarView() : base()
    {
        ColorScheme = DungeonColorSchemes.Menu;
        InitializeMenu();
    }

    private void InitializeMenu()
    {
        var fileMenu = new MenuBarItem("_File", new MenuItem[]
        {
            new MenuItem("_New Game", "Start a new game", () => OnNewGameRequested()),
            null!, // Separator
            new MenuItem("_Save Game", "Save current progress", () => OnSaveGameRequested()),
            new MenuItem("_Load Game", "Load saved game", () => OnLoadGameRequested()),
            null!, // Separator
            new MenuItem("_Quit", "Exit the game", () => OnQuitRequested())
        });

        var gameMenu = new MenuBarItem("_Game", new MenuItem[]
        {
            new MenuItem("_Inventory", "Open inventory", () => RequestStateChange(GameState.Inventory)),
            new MenuItem("_Character", "View character sheet", () => { /* TODO: Character sheet */ }),
            null!, // Separator
            new MenuItem("_Pause", "Pause game", () => RequestStateChange(GameState.Paused)),
            new MenuItem("_Resume", "Resume game", () => RequestStateChange(GameState.Playing))
        });

        var optionsMenu = new MenuBarItem("_Options", new MenuItem[]
        {
            new MenuItem("_Controls", "View controls", () => ShowControls()),
            new MenuItem("_Display", "Display settings", () => { /* TODO: Display settings */ }),
            null!, // Separator
            new MenuItem("_Preferences", "Game preferences", () => { /* TODO: Preferences */ })
        });

        var helpMenu = new MenuBarItem("_Help", new MenuItem[]
        {
            new MenuItem("_Help", "Show help", () => OnHelpRequested()),
            new MenuItem("_Controls", "View controls", () => ShowControls()),
            null!, // Separator
            new MenuItem("_About", "About this game", () => OnAboutRequested())
        });

        Menus = new MenuBarItem[] { fileMenu, gameMenu, optionsMenu, helpMenu };
    }

    private void OnNewGameRequested()
    {
        var result = MessageBox.Query("New Game", "Start a new game? Current progress will be lost.", "Yes", "No");
        if (result == 0)
        {
            NewGameRequested?.Invoke();
            RequestStateChange(GameState.Playing);
        }
    }

    private void OnSaveGameRequested()
    {
        SaveGameRequested?.Invoke();
        // Show save feedback
        Application.MainLoop.Invoke(() =>
        {
            MessageBox.Query("Save Game", "Game saved successfully!", "OK");
        });
    }

    private void OnLoadGameRequested()
    {
        var result = MessageBox.Query("Load Game", "Load saved game? Current progress will be lost.", "Yes", "No");
        if (result == 0)
        {
            LoadGameRequested?.Invoke();
        }
    }

    private void OnQuitRequested()
    {
        var result = MessageBox.Query("Quit Game", "Are you sure you want to quit?", "Yes", "No");
        if (result == 0)
        {
            QuitRequested?.Invoke();
            RequestStateChange(GameState.Exiting);
        }
    }

    private void OnHelpRequested()
    {
        HelpRequested?.Invoke();
        ShowHelp();
    }

    private void OnAboutRequested()
    {
        AboutRequested?.Invoke();
        ShowAbout();
    }

    private void RequestStateChange(GameState newState)
    {
        UIEvents.RaiseStateChangeRequested(newState);
    }

    private void ShowControls()
    {
        var controlsText = @"
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
  F1 - Help

TIPS:
  - Explore rooms and corridors
  - Fight monsters to gain experience
  - Collect items to improve your character
  - Save frequently!
";

        var dialog = new Dialog("Controls", 60, 20);
        var textView = new TextView()
        {
            X = 1,
            Y = 1,
            Width = Dim.Fill() - 1,
            Height = Dim.Fill() - 3,
            ReadOnly = true,
            Text = controlsText,
            WordWrap = true
        };
        
        var okButton = new Button("OK")
        {
            X = Pos.Center(),
            Y = Pos.Bottom(textView) + 1
        };
        okButton.Clicked += () => dialog.RequestStop();

        dialog.Add(textView, okButton);
        Application.Run(dialog);
    }

    private void ShowHelp()
    {
        var helpText = @"
DUNGEON CODING AGENT

Welcome to a classic roguelike dungeon crawler!

OBJECTIVE:
Explore the dungeon, fight monsters, collect treasure,
and try to survive as long as possible.

GAMEPLAY:
- Move around using arrow keys or WASD
- Fight monsters by moving into them
- Pick up items with 'g'
- Use your inventory with 'i'
- Rest and recover with Space

The game is turn-based - every action you take
advances time by one turn.

Good luck, adventurer!
";

        var dialog = new Dialog("Help", 50, 18);
        var textView = new TextView()
        {
            X = 1,
            Y = 1,
            Width = Dim.Fill() - 1,
            Height = Dim.Fill() - 3,
            ReadOnly = true,
            Text = helpText,
            WordWrap = true
        };
        
        var okButton = new Button("OK")
        {
            X = Pos.Center(),
            Y = Pos.Bottom(textView) + 1
        };
        okButton.Clicked += () => dialog.RequestStop();

        dialog.Add(textView, okButton);
        Application.Run(dialog);
    }

    private void ShowAbout()
    {
        var aboutText = @"
DUNGEON CODING AGENT
Version 1.0

A classic roguelike dungeon crawler built with:
- .NET 8
- Arch ECS
- Terminal.Gui v2

Created as part of the GitHub Copilot Coding Agent
testing project implementing RFC-based development.

This game demonstrates modern ECS architecture
in a classic terminal-based roguelike format.
";

        var dialog = new Dialog("About", 45, 15);
        var textView = new TextView()
        {
            X = 1,
            Y = 1,
            Width = Dim.Fill() - 1,
            Height = Dim.Fill() - 3,
            ReadOnly = true,
            Text = aboutText,
            WordWrap = true
        };
        
        var okButton = new Button("OK")
        {
            X = Pos.Center(),
            Y = Pos.Bottom(textView) + 1
        };
        okButton.Clicked += () => dialog.RequestStop();

        dialog.Add(textView, okButton);
        Application.Run(dialog);
    }
}