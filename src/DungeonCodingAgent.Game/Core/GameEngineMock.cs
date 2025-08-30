using DungeonCodingAgent.Game.Core;
using DungeonCodingAgent.Game.Persistence;
using DungeonCodingAgent.Game.Systems;

namespace DungeonCodingAgent.Game.Core;

/// <summary>
/// Simple input manager for handling save/load key combinations
/// </summary>
public class InputManager
{
    private readonly SaveGameSystem _saveSystem;
    private GameSaveData? _currentGameData;

    public InputManager(SaveGameSystem saveSystem)
    {
        _saveSystem = saveSystem;
    }

    /// <summary>
    /// Process a key input and handle save/load commands
    /// </summary>
    /// <param name="key">The key that was pressed</param>
    /// <param name="isCtrlPressed">Whether Ctrl was held down</param>
    public async Task<bool> ProcessInput(ConsoleKey key, bool isCtrlPressed = false)
    {
        if (!isCtrlPressed) return false;

        switch (key)
        {
            case ConsoleKey.S:
                await HandleSaveRequest();
                return true;
                
            case ConsoleKey.L:
                await HandleLoadRequest();
                return true;
                
            default:
                return false;
        }
    }

    /// <summary>
    /// Set the current game data to be saved
    /// </summary>
    public void SetCurrentGameData(GameSaveData gameData)
    {
        _currentGameData = gameData;
    }

    /// <summary>
    /// Get the last loaded game data
    /// </summary>
    public GameSaveData? GetCurrentGameData() => _currentGameData;

    private async Task HandleSaveRequest()
    {
        if (_currentGameData == null)
        {
            GameEvents.RaiseSaveLoadError("No game data to save");
            return;
        }

        GameEvents.RaiseSaveRequested();
        await _saveSystem.SaveGameAsync(_currentGameData);
    }

    private async Task HandleLoadRequest()
    {
        GameEvents.RaiseLoadRequested();
        var (success, gameData) = await _saveSystem.LoadGameAsync();
        
        if (success && gameData != null)
        {
            _currentGameData = gameData;
        }
    }
}

/// <summary>
/// Game engine mock for demonstrating save/load functionality
/// </summary>
public class GameEngineMock : IDisposable
{
    private readonly SaveGameSystem _saveSystem;
    private readonly InputManager _inputManager;
    private GameSaveData _gameData;

    public GameEngineMock()
    {
        _saveSystem = new SaveGameSystem();
        _inputManager = new InputManager(_saveSystem);
        _gameData = CreateNewGame();
        _inputManager.SetCurrentGameData(_gameData);
        
        // Subscribe to events
        GameEvents.GameSaved += OnGameSaved;
        GameEvents.GameLoaded += OnGameLoaded;
        GameEvents.SaveLoadError += OnSaveLoadError;
    }

    /// <summary>
    /// Create a new game with sample data
    /// </summary>
    public GameSaveData CreateNewGame()
    {
        return new GameSaveData
        {
            Version = "1.0",
            CurrentTurn = 1,
            GameState = GameState.Playing.ToString(),
            Player = new PlayerSaveData
            {
                Name = "Hero",
                Level = 1,
                Experience = 0,
                ExperienceToNext = 100,
                Health = new HealthSaveData { Current = 100, Maximum = 100 },
                Mana = new ManaSaveData { Current = 50, Maximum = 50 },
                Stats = new StatsSaveData(),
                Position = new PositionSaveData { X = 40, Y = 12 }
            },
            Map = new MapSaveData
            {
                Width = 80,
                Height = 24,
                TileData = new List<List<TileSaveData>>(),
                EntityPositions = new Dictionary<string, PositionSaveData>()
            },
            Entities = new List<EntitySaveData>(),
            Inventory = new List<ItemSaveData>()
        };
    }

    /// <summary>
    /// Simulate game progression
    /// </summary>
    public void AdvanceGame()
    {
        _gameData.CurrentTurn++;
        _gameData.Player.Experience += 10;
        
        // Simulate player movement
        var random = new Random();
        _gameData.Player.Position.X = Math.Max(0, Math.Min(79, _gameData.Player.Position.X + random.Next(-1, 2)));
        _gameData.Player.Position.Y = Math.Max(0, Math.Min(23, _gameData.Player.Position.Y + random.Next(-1, 2)));
        
        // Update input manager with new data
        _inputManager.SetCurrentGameData(_gameData);
        
        Console.WriteLine($"Turn {_gameData.CurrentTurn}: Player at ({_gameData.Player.Position.X}, {_gameData.Player.Position.Y}), Experience: {_gameData.Player.Experience}");
    }

    /// <summary>
    /// Simulate handling input
    /// </summary>
    public async Task HandleInput(ConsoleKey key, bool isCtrlPressed = false)
    {
        await _inputManager.ProcessInput(key, isCtrlPressed);
    }

    /// <summary>
    /// Get current game state for display
    /// </summary>
    public string GetGameStatus()
    {
        return $"Turn: {_gameData.CurrentTurn} | Player: {_gameData.Player.Name} (Level {_gameData.Player.Level}) | " +
               $"Position: ({_gameData.Player.Position.X}, {_gameData.Player.Position.Y}) | " +
               $"Health: {_gameData.Player.Health.Current}/{_gameData.Player.Health.Maximum} | " +
               $"Experience: {_gameData.Player.Experience}";
    }

    /// <summary>
    /// Check if save file exists
    /// </summary>
    public bool HasSaveFile() => _saveSystem.SaveFileExists();

    private void OnGameSaved(string saveFile)
    {
        Console.WriteLine($"✓ Game saved to: {saveFile}");
    }

    private void OnGameLoaded(string saveFile)
    {
        // Update our game data with the loaded data
        var loadedData = _inputManager.GetCurrentGameData();
        if (loadedData != null)
        {
            _gameData = loadedData;
            Console.WriteLine($"✓ Game loaded from: {saveFile}");
        }
    }

    private void OnSaveLoadError(string error)
    {
        Console.WriteLine($"✗ Error: {error}");
    }

    public void Dispose()
    {
        _saveSystem?.Dispose();
        GameEvents.GameSaved -= OnGameSaved;
        GameEvents.GameLoaded -= OnGameLoaded;
        GameEvents.SaveLoadError -= OnSaveLoadError;
    }
}