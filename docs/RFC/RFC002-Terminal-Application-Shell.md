# RFC002: Terminal.Gui Application Shell

## 📋 Metadata
- **RFC Number**: 002
- **Title**: Terminal.Gui Application Shell
- **Status**: 📝 Draft
- **Author**: System Architect
- **Created**: 2025-08-30
- **Dependencies**: RFC001 (Core Game Loop)

## 🎯 Objective

Implement the Terminal.Gui application shell that provides the main UI framework, handles terminal initialization, manages application lifecycle, and coordinates between the game engine and terminal UI.

## 📖 Problem Statement

The dungeon crawler needs a robust terminal UI foundation that:
- Initializes and configures Terminal.Gui properly
- Provides responsive UI layout that works across different terminal sizes
- Manages application lifecycle (startup, shutdown, error handling)
- Bridges between game logic (ECS) and UI representation
- Handles terminal-specific concerns (colors, input, rendering)

## 🏗️ Technical Specification

### **Core Application Structure**

#### **DungeonApplication Class**
```csharp
public class DungeonApplication : Application
{
    private GameEngine gameEngine;
    private MainWindow mainWindow;
    private bool isInitialized;
    
    public static void Initialize()
    public static void Run()
    public static new void Shutdown()
    
    protected override void Init()
    protected override void Shutdown()
    private void SetupColorSchemes()
    private void HandleGlobalExceptions(Exception ex)
}
```

#### **MainWindow Layout**
```csharp
public class MainWindow : Window
{
    private GameMapView mapView;
    private StatusBarView statusBar;
    private MessageLogView messageLog;
    private MenuBarView menuBar;
    
    public MainWindow()
    private void InitializeLayout()
    private void SetupEventHandlers()
    public void UpdateGameState(GameState state)
}
```

### **UI Architecture**

#### **Layout Design**
```
┌─────────────────────────────────────────────────────────────┐
│ File  Game  Options  Help                          [Status] │ <- MenuBar
├─────────────────────────────────────────────────────────────┤
│                                                             │
│                     Game Map View                           │ <- GameMapView
│  ########################                                   │   (main area)
│  #......#.............#                                     │
│  #......#.............#                                     │
│  #..@...#.............#                                     │
│  #......###############                                     │
│  ########################                                   │
│                                                             │
├─────────────────────────────────────────────────────────────┤
│ Health: ████████████████████ | Mana: ████████░░░░░░░░░░     │ <- StatusBarView
├─────────────────────────────────────────────────────────────┤
│ Message Log:                                                │ <- MessageLogView
│ > You entered the dungeon                                   │   (scrollable)
│ > A goblin appears!                                         │
│ > You attack the goblin for 5 damage                       │
└─────────────────────────────────────────────────────────────┘
```

#### **Color Scheme Configuration**
```csharp
public static class DungeonColorSchemes
{
    public static ColorScheme Default { get; }
    public static ColorScheme Game { get; }
    public static ColorScheme Menu { get; }
    public static ColorScheme Status { get; }
    public static ColorScheme Combat { get; }
    
    static DungeonColorSchemes()
    {
        // Define color schemes for different UI contexts
    }
}
```

### **View Components**

#### **GameMapView**
```csharp
public class GameMapView : View
{
    private GameMap currentMap;
    private Point cameraPosition;
    private int viewportWidth;
    private int viewportHeight;
    
    public GameMapView()
    public override void OnDrawContent(Rectangle viewport)
    public void UpdateMap(GameMap map)
    public void CenterOnEntity(EntityId entity)
    public void HandleInput(KeyEvent keyEvent)
}
```

#### **StatusBarView**  
```csharp
public class StatusBarView : View
{
    private ProgressBar healthBar;
    private ProgressBar manaBar;
    private Label levelLabel;
    private Label experienceLabel;
    
    public StatusBarView()
    public void UpdatePlayerStats(PlayerStats stats)
    private void RefreshBars()
}
```

#### **MessageLogView**
```csharp
public class MessageLogView : ScrollView
{
    private List<GameMessage> messages;
    private int maxMessages;
    
    public MessageLogView()
    public void AddMessage(string message, MessageType type)
    public void AddMessage(GameMessage message)
    public void ClearLog()
    private void ScrollToBottom()
}
```

### **Input Handling**

#### **Input Manager**
```csharp
public class InputManager
{
    public event Action<PlayerAction> PlayerActionRequested;
    
    public void ProcessKeyEvent(KeyEvent keyEvent)
    public void RegisterGlobalHotkeys()
    private PlayerAction MapKeyToAction(Key key)
    private bool IsGameplayKey(Key key)
}

public enum PlayerAction
{
    MoveNorth, MoveSouth, MoveEast, MoveWest,
    Wait, Pickup, Drop, Inventory, Character,
    Save, Load, Quit, Help
}
```

## 🎮 Implementation Details

### **Application Lifecycle**
```csharp
public static class Program
{
    public static void Main(string[] args)
    {
        try
        {
            DungeonApplication.Initialize();
            DungeonApplication.Run();
        }
        catch (Exception ex)
        {
            HandleCriticalError(ex);
        }
        finally
        {
            DungeonApplication.Shutdown();
        }
    }
    
    private static void HandleCriticalError(Exception ex)
    {
        // Log error and show user-friendly message
        Console.WriteLine($"Fatal error: {ex.Message}");
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}
```

### **Terminal Configuration**
```csharp
private void InitializeTerminal()
{
    // Configure Terminal.Gui
    Application.UseSystemConsole = true;
    
    // Set up proper Unicode support
    Console.OutputEncoding = System.Text.Encoding.UTF8;
    
    // Configure color support detection
    var colorScheme = DetectColorSupport() ? 
        DungeonColorSchemes.Default : 
        DungeonColorSchemes.Monochrome;
        
    Colors.Base = colorScheme;
}

private bool DetectColorSupport()
{
    // Detect terminal color capabilities
    return Application.Driver.Colors.HasColors;
}
```

### **Game Engine Integration**
```csharp
private void IntegrateGameEngine()
{
    gameEngine = new GameEngine();
    gameEngine.Initialize();
    
    // Subscribe to game events
    gameEngine.StateChanged += OnGameStateChanged;
    GameEvents.PlayerMoved += OnPlayerMoved;
    GameEvents.MessageLogged += OnMessageLogged;
    GameEvents.PlayerStatsChanged += OnPlayerStatsChanged;
}

private void OnGameStateChanged(GameState newState)
{
    Application.MainLoop.Invoke(() =>
    {
        mainWindow.UpdateGameState(newState);
        
        switch (newState)
        {
            case GameState.Playing:
                mapView.CanFocus = true;
                break;
            case GameState.MainMenu:
                ShowMainMenu();
                break;
            case GameState.Paused:
                ShowPauseDialog();
                break;
        }
    });
}
```

### **Responsive Layout**
```csharp
public void UpdateLayout()
{
    var bounds = Bounds;
    
    // Menu bar always at top
    menuBar.Frame = new Rectangle(0, 0, bounds.Width, 1);
    
    // Calculate available space
    var availableHeight = bounds.Height - 3; // menu + status + message
    var mapHeight = (int)(availableHeight * 0.7); // 70% for map
    var messageHeight = availableHeight - mapHeight;
    
    // Update view frames
    mapView.Frame = new Rectangle(0, 1, bounds.Width, mapHeight);
    statusBar.Frame = new Rectangle(0, 1 + mapHeight, bounds.Width, 1);
    messageLog.Frame = new Rectangle(0, 2 + mapHeight, bounds.Width, messageHeight);
}
```

## ✅ Acceptance Criteria

### **Functional Requirements**
- [ ] Application starts up cleanly in any terminal size ≥80x24
- [ ] UI layout adapts to terminal resize events
- [ ] All keyboard input is handled correctly (no key conflicts)
- [ ] Game map renders with proper ASCII/Unicode characters
- [ ] Status bar updates in real-time with player stats
- [ ] Message log scrolls properly and displays game messages
- [ ] Menu system provides access to game functions
- [ ] Application shuts down gracefully on Ctrl+C or quit command

### **Visual Requirements**
- [ ] Colors work correctly on color-capable terminals
- [ ] Monochrome fallback works on terminals without color
- [ ] UI elements are clearly distinguishable
- [ ] Text is readable in all supported color schemes
- [ ] Layout looks proper in 80x24 minimum and larger sizes

### **Performance Requirements**
- [ ] UI updates complete within 16ms (60 FPS)
- [ ] Keyboard input has minimal latency (<50ms)
- [ ] Screen redraws are efficient (no flicker)
- [ ] Memory usage remains stable during extended gameplay

### **Integration Requirements**
- [ ] Game engine events trigger appropriate UI updates
- [ ] Input events are translated to game actions correctly
- [ ] Error handling prevents UI from crashing on game errors
- [ ] Save/load operations work through UI (when implemented)

## 🔗 Integration Points

### **Dependencies**
- **RFC001**: Core Game Loop (GameEngine, GameState, Events)

### **Dependents**
- **RFC003**: Map Generation (GameMapView rendering)
- **RFC004**: Player Movement (input handling)
- **RFC007**: UI Panel Components (status bar, message log)

### **Events Consumed**
```csharp
GameEvents.GameStateChanged     // Update UI state
GameEvents.PlayerMoved         // Update map display
GameEvents.MessageLogged       // Add to message log
GameEvents.PlayerStatsChanged  // Update status bar
GameEvents.MapChanged          // Refresh map view
```

### **Events Published**  
```csharp
InputEvents.PlayerActionRequested  // Player input
UIEvents.StateChangeRequested     // Menu selections
UIEvents.WindowResized             // Layout changes
```

## 📊 Performance Considerations

### **Rendering Optimization**
- Use Terminal.Gui's built-in dirty region tracking
- Cache rendered map tiles between updates
- Minimize string allocations during rendering
- Use StringBuilder for complex text formatting

### **Memory Management**
- Dispose of views properly on shutdown
- Use object pooling for frequently created UI elements
- Implement weak event handlers to prevent memory leaks

### **Responsiveness**
- Process input events on main thread
- Use background threads for non-UI game logic
- Implement proper async/await for long-running operations

## 🧪 Testing Strategy

### **Unit Tests**
```csharp
[Fact]
public void DungeonApplication_WhenInitialized_SetsUpUICorrectly()

[Fact]
public void GameMapView_WhenMapUpdates_RendersCorrectly()

[Fact]
public void InputManager_WhenKeyPressed_TranslatesToCorrectAction()
```

### **Integration Tests**
- Full application startup/shutdown cycle
- UI responsiveness under load
- Error handling during game state changes

### **Manual Testing**
- Test on different terminal emulators
- Verify color schemes on various terminals
- Test keyboard input handling
- Verify layout on different screen sizes

## 📝 Implementation Notes

### **Phase 1: Basic Shell**
1. Implement DungeonApplication and MainWindow
2. Set up basic layout with placeholder views
3. Add Terminal.Gui initialization and configuration

### **Phase 2: View Implementation**
1. Implement GameMapView with basic rendering
2. Create StatusBarView with progress bars
3. Add MessageLogView with scrolling

### **Phase 3: Integration**
1. Connect input handling to game engine
2. Implement responsive layout system
3. Add error handling and recovery

### **Platform Considerations**
- Test on Windows Terminal, PowerShell, CMD
- Verify Unicode support across platforms
- Handle terminal-specific quirks (colors, input)

## 📚 References

- [Terminal.Gui Documentation](https://gui-cs.github.io/Terminal.Gui/)
- [Terminal.Gui Keyboard Handling](https://gui-cs.github.io/Terminal.Gui/docs/keyboard.html)
- [Console Application Best Practices](https://docs.microsoft.com/en-us/dotnet/core/deploying/)

---

## 🎯 Implementation Ready

This RFC provides complete specification for the Terminal.Gui application shell. The implementation provides the UI foundation that all other systems will build upon.

**Estimated Effort**: 3-4 days for full implementation including tests
**Risk Level**: Medium - Terminal.Gui platform dependencies  
**Priority**: High - Required foundation for user interaction