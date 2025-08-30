using Terminal.Gui;
using DungeonCodingAgent.Game.Core;
using DungeonCodingAgent.Game.UI.Views;

namespace DungeonCodingAgent.Game.UI;

/// <summary>
/// Main window that contains all UI components and manages layout
/// </summary>
public class MainWindow : Window
{
    private GameMapView? _mapView;
    private StatusBarView? _statusBar;
    private MessageLogView? _messageLog;
    private MenuBarView? _menuBar;
    private InputManager? _inputManager;

    public GameMapView MapView => _mapView!;
    public StatusBarView StatusBar => _statusBar!;
    public MessageLogView MessageLog => _messageLog!;
    public new MenuBarView MenuBar => _menuBar!;

    public MainWindow()
    {
        Title = "Dungeon Coding Agent";
        ColorScheme = DungeonColorSchemes.Default;
        
        InitializeLayout();
        SetupEventHandlers();
        SetupInputHandling();
    }

    private void InitializeLayout()
    {
        // Create menu bar
        _menuBar = new MenuBarView();
        Add(_menuBar);

        // Calculate layout dimensions
        var contentY = 1; // Below menu bar
        var statusHeight = 2;
        var messageHeight = 6;
        var mapHeight = Dim.Fill() - statusHeight - messageHeight;

        // Create game map view (main area)
        _mapView = new GameMapView()
        {
            X = 0,
            Y = contentY,
            Width = Dim.Fill(),
            Height = mapHeight
        };
        Add(_mapView);

        // Create status bar
        _statusBar = new StatusBarView()
        {
            X = 0,
            Y = Pos.Bottom(_mapView),
            Width = Dim.Fill(),
            Height = statusHeight
        };
        Add(_statusBar);

        // Create message log
        _messageLog = new MessageLogView()
        {
            X = 0,
            Y = Pos.Bottom(_statusBar),
            Width = Dim.Fill(),
            Height = messageHeight,
            Title = "Messages"
        };
        Add(_messageLog);

        // Set initial focus to map view
        _mapView.SetFocus();
    }

    private void SetupEventHandlers()
    {
        // Menu event handlers
        if (_menuBar != null)
        {
            _menuBar.NewGameRequested += OnNewGameRequested;
            _menuBar.SaveGameRequested += OnSaveGameRequested;
            _menuBar.LoadGameRequested += OnLoadGameRequested;
            _menuBar.QuitRequested += OnQuitRequested;
            _menuBar.HelpRequested += OnHelpRequested;
        }

        // Game event handlers
        GameEvents.GameStateChanged += OnGameStateChanged;
        GameEvents.MessageLogged += OnMessageLogged;
        GameEvents.PlayerStatsChanged += OnPlayerStatsChanged;
        GameEvents.MapChanged += OnMapChanged;

        // UI event handlers
        UIEvents.StateChangeRequested += OnStateChangeRequested;
        UIEvents.WindowResized += OnWindowResized;

        // Handle window resize
        Application.Resized += OnApplicationResized;
    }

    private void SetupInputHandling()
    {
        _inputManager = new InputManager();
        _inputManager.PlayerActionRequested += OnPlayerActionRequested;

        // Handle key events
        KeyDown += (e) => { OnKeyDown(e.Key); };
    }

    protected override bool OnKeyDown(Key key)
    {
        // Handle modifier keys
        var shift = (key & Key.ShiftMask) != 0;
        var ctrl = (key & Key.CtrlMask) != 0;
        var alt = (key & Key.AltMask) != 0;

        // Remove modifier bits to get base key
        var baseKey = key & ~(Key.ShiftMask | Key.CtrlMask | Key.AltMask);

        _inputManager?.ProcessKeyEvent(baseKey, shift, ctrl, alt);
        
        return true; // Key was handled
    }

    private void OnNewGameRequested()
    {
        // Initialize new game
        _messageLog?.ClearLog();
        _messageLog?.AddMessage("Starting new game...", MessageType.System);
        _statusBar?.UpdatePlayerStats(50, 50, 30, 30, 1, 0, 100, 1);
    }

    private void OnSaveGameRequested()
    {
        _messageLog?.AddMessage("Game saved", MessageType.System);
    }

    private void OnLoadGameRequested()
    {
        _messageLog?.AddMessage("Game loaded", MessageType.System);
    }

    private void OnQuitRequested()
    {
        Application.RequestStop();
    }

    private void OnHelpRequested()
    {
        _messageLog?.AddMessage("Help displayed", MessageType.System);
    }

    private void OnGameStateChanged(GameState newState)
    {
        // Update UI based on game state
        switch (newState)
        {
            case GameState.Playing:
                Title = "Dungeon Coding Agent - Playing";
                break;
            case GameState.Paused:
                Title = "Dungeon Coding Agent - Paused";
                break;
            case GameState.MainMenu:
                Title = "Dungeon Coding Agent - Main Menu";
                break;
            case GameState.GameOver:
                Title = "Dungeon Coding Agent - Game Over";
                break;
            case GameState.Inventory:
                Title = "Dungeon Coding Agent - Inventory";
                break;
            case GameState.Exiting:
                Application.RequestStop();
                break;
        }

        SetNeedsDisplay();
    }

    private void OnMessageLogged(string message)
    {
        _messageLog?.AddMessage(message);
    }

    private void OnPlayerStatsChanged()
    {
        // Stats would be updated by the game engine
        // For now, we'll just refresh the display
        _statusBar?.SetNeedsDisplay();
    }

    private void OnMapChanged()
    {
        _mapView?.SetNeedsDisplay();
    }

    private void OnStateChangeRequested(GameState newState)
    {
        // Forward state change request to game engine
        // This would normally be handled by the application
        GameEvents.RaiseGameStateChanged(newState);
    }

    private void OnWindowResized()
    {
        UpdateLayout();
    }

    private void OnApplicationResized()
    {
        UpdateLayout();
        UIEvents.RaiseWindowResized();
    }

    public void UpdateLayout()
    {
        if (Bounds.Width < 80 || Bounds.Height < 24)
        {
            // Show warning for small terminals
            _messageLog?.AddMessage("Terminal too small! Minimum size: 80x24", MessageType.Error);
        }

        // Recalculate layout if needed
        var contentY = 1;
        var statusHeight = 2;
        var messageHeight = Math.Max(4, Bounds.Height / 6); // At least 4 lines, or 1/6 of screen
        var mapHeight = Bounds.Height - contentY - statusHeight - messageHeight - 1;

        if (_mapView != null)
        {
            _mapView.Height = Math.Max(10, mapHeight);
        }

        if (_messageLog != null)
        {
            _messageLog.Height = messageHeight;
            _messageLog.Y = Pos.Bottom(_statusBar);
        }

        SetNeedsDisplay();
    }

    public void UpdateGameState(GameState state)
    {
        OnGameStateChanged(state);
    }

    private void OnPlayerActionRequested(PlayerAction action)
    {
        // Handle player actions
        switch (action)
        {
            case PlayerAction.MoveNorth:
            case PlayerAction.MoveSouth:
            case PlayerAction.MoveEast:
            case PlayerAction.MoveWest:
                _messageLog?.AddMessage($"Player moved {action.ToString().Replace("Move", "").ToLower()}", MessageType.Normal);
                break;
            case PlayerAction.Wait:
                _messageLog?.AddMessage("You wait", MessageType.Normal);
                break;
            case PlayerAction.Pickup:
                _messageLog?.AddMessage("You pick up an item", MessageType.Success);
                break;
            case PlayerAction.Drop:
                _messageLog?.AddMessage("You drop an item", MessageType.Normal);
                break;
            case PlayerAction.Inventory:
                GameEvents.RaiseGameStateChanged(GameState.Inventory);
                break;
            case PlayerAction.Character:
                _messageLog?.AddMessage("Character sheet (not implemented)", MessageType.System);
                break;
            case PlayerAction.Save:
                OnSaveGameRequested();
                break;
            case PlayerAction.Load:
                OnLoadGameRequested();
                break;
            case PlayerAction.Quit:
                OnQuitRequested();
                break;
            case PlayerAction.Help:
                OnHelpRequested();
                break;
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Unsubscribe from events
            GameEvents.GameStateChanged -= OnGameStateChanged;
            GameEvents.MessageLogged -= OnMessageLogged;
            GameEvents.PlayerStatsChanged -= OnPlayerStatsChanged;
            GameEvents.MapChanged -= OnMapChanged;
            UIEvents.StateChangeRequested -= OnStateChangeRequested;
            UIEvents.WindowResized -= OnWindowResized;
            Application.Resized -= OnApplicationResized;

            if (_menuBar != null)
            {
                _menuBar.NewGameRequested -= OnNewGameRequested;
                _menuBar.SaveGameRequested -= OnSaveGameRequested;
                _menuBar.LoadGameRequested -= OnLoadGameRequested;
                _menuBar.QuitRequested -= OnQuitRequested;
                _menuBar.HelpRequested -= OnHelpRequested;
            }

            _mapView?.Dispose();
            _statusBar?.Dispose();
            _messageLog?.Dispose();
            _menuBar?.Dispose();
        }
        
        base.Dispose(disposing);
    }
}