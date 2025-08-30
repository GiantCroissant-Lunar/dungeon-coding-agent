using System.Text.Json;
using DungeonCodingAgent.Game.Core;
using DungeonCodingAgent.Game.Persistence;
using DungeonCodingAgent.Game.Systems;

namespace DungeonCodingAgent.Tests.Persistence;

public class SaveGameSystemTests : IDisposable
{
    private readonly string _testSaveDirectory;
    private readonly SaveGameSystem _saveSystem;

    public SaveGameSystemTests()
    {
        // Create a temporary directory for test saves
        _testSaveDirectory = Path.Combine(Path.GetTempPath(), "DungeonCodingAgentTests", Guid.NewGuid().ToString());
        _saveSystem = new SaveGameSystem(_testSaveDirectory);
    }

    [Fact]
    public void Constructor_CreatesDirectoryIfNotExists()
    {
        // Directory should be created during construction
        Assert.True(Directory.Exists(_testSaveDirectory));
    }

    [Fact]
    public void SaveFileExists_ReturnsFalseWhenNoSaveFile()
    {
        // Arrange & Act & Assert
        Assert.False(_saveSystem.SaveFileExists());
    }

    [Fact]
    public async Task SaveGameAsync_WithValidData_ReturnsTrueAndCreatesSaveFile()
    {
        // Arrange
        var gameData = CreateTestGameData();

        // Act
        var result = await _saveSystem.SaveGameAsync(gameData);

        // Assert
        Assert.True(result);
        Assert.True(_saveSystem.SaveFileExists());
    }

    [Fact]
    public async Task SaveGameAsync_WithValidData_UpdatesSaveDate()
    {
        // Arrange
        var gameData = CreateTestGameData();
        var originalDate = gameData.SaveDate;
        
        // Wait a small amount to ensure time difference
        await Task.Delay(10);

        // Act
        await _saveSystem.SaveGameAsync(gameData);

        // Assert
        Assert.True(gameData.SaveDate > originalDate);
    }

    [Fact]
    public async Task LoadGameAsync_WithNoSaveFile_ReturnsFailure()
    {
        // Act
        var (success, gameData) = await _saveSystem.LoadGameAsync();

        // Assert
        Assert.False(success);
        Assert.Null(gameData);
    }

    [Fact]
    public async Task SaveAndLoadGame_PreservesExactState()
    {
        // Arrange
        var originalData = CreateTestGameData();
        originalData.CurrentTurn = 42;
        originalData.Player.Name = "TestPlayer";
        originalData.Player.Level = 5;
        originalData.Player.Health.Current = 85;
        originalData.Player.Health.Maximum = 100;
        originalData.Player.Position.X = 10;
        originalData.Player.Position.Y = 15;

        // Act - Save
        var saveResult = await _saveSystem.SaveGameAsync(originalData);
        Assert.True(saveResult);

        // Act - Load
        var (loadResult, loadedData) = await _saveSystem.LoadGameAsync();

        // Assert
        Assert.True(loadResult);
        Assert.NotNull(loadedData);
        
        // Verify all data preserved exactly
        Assert.Equal(originalData.CurrentTurn, loadedData.CurrentTurn);
        Assert.Equal(originalData.Player.Name, loadedData.Player.Name);
        Assert.Equal(originalData.Player.Level, loadedData.Player.Level);
        Assert.Equal(originalData.Player.Health.Current, loadedData.Player.Health.Current);
        Assert.Equal(originalData.Player.Health.Maximum, loadedData.Player.Health.Maximum);
        Assert.Equal(originalData.Player.Position.X, loadedData.Player.Position.X);
        Assert.Equal(originalData.Player.Position.Y, loadedData.Player.Position.Y);
    }

    [Fact]
    public async Task SaveGameAsync_PerformanceTest_CompletesWithinOneSecond()
    {
        // Arrange
        var gameData = CreateLargeTestGameData();
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        var result = await _saveSystem.SaveGameAsync(gameData);

        // Assert
        stopwatch.Stop();
        Assert.True(result);
        Assert.True(stopwatch.ElapsedMilliseconds < 1000, $"Save took {stopwatch.ElapsedMilliseconds}ms, should be < 1000ms");
    }

    [Fact]
    public async Task LoadGameAsync_PerformanceTest_CompletesWithinOneSecond()
    {
        // Arrange
        var gameData = CreateLargeTestGameData();
        await _saveSystem.SaveGameAsync(gameData);
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Act
        var (success, _) = await _saveSystem.LoadGameAsync();

        // Assert
        stopwatch.Stop();
        Assert.True(success);
        Assert.True(stopwatch.ElapsedMilliseconds < 1000, $"Load took {stopwatch.ElapsedMilliseconds}ms, should be < 1000ms");
    }

    [Fact]
    public async Task LoadGameAsync_WithCorruptedFile_ReturnsFailure()
    {
        // Arrange - Create a corrupted save file
        var saveFilePath = Path.Combine(_testSaveDirectory, "quicksave.json");
        await File.WriteAllTextAsync(saveFilePath, "{ invalid json content }");

        // Act
        var (success, gameData) = await _saveSystem.LoadGameAsync();

        // Assert
        Assert.False(success);
        Assert.Null(gameData);
    }

    [Fact]
    public async Task LoadGameAsync_WithEmptyFile_ReturnsFailure()
    {
        // Arrange - Create an empty save file
        var saveFilePath = Path.Combine(_testSaveDirectory, "quicksave.json");
        await File.WriteAllTextAsync(saveFilePath, "");

        // Act
        var (success, gameData) = await _saveSystem.LoadGameAsync();

        // Assert
        Assert.False(success);
        Assert.Null(gameData);
    }

    [Fact]
    public void DeleteSave_WithExistingSave_RemovesFile()
    {
        // Arrange
        var saveFilePath = Path.Combine(_testSaveDirectory, "quicksave.json");
        File.WriteAllText(saveFilePath, "test content");
        Assert.True(File.Exists(saveFilePath));

        // Act
        var result = _saveSystem.DeleteSave();

        // Assert
        Assert.True(result);
        Assert.False(File.Exists(saveFilePath));
    }

    [Fact]
    public void DeleteSave_WithNoSave_ReturnsFalse()
    {
        // Act
        var result = _saveSystem.DeleteSave();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetSaveInfo_WithExistingSave_ReturnsCorrectInfo()
    {
        // Arrange
        var gameData = CreateTestGameData();
        await _saveSystem.SaveGameAsync(gameData);

        // Act
        var saveInfo = _saveSystem.GetSaveInfo();

        // Assert
        Assert.NotNull(saveInfo);
        Assert.True(saveInfo.Value.FileSize > 0);
        Assert.True(saveInfo.Value.SaveDate <= DateTime.Now);
    }

    [Fact]
    public void GetSaveInfo_WithNoSave_ReturnsNull()
    {
        // Act
        var saveInfo = _saveSystem.GetSaveInfo();

        // Assert
        Assert.Null(saveInfo);
    }

    private static GameSaveData CreateTestGameData()
    {
        return new GameSaveData
        {
            Version = "1.0",
            CurrentTurn = 1,
            GameState = GameState.Playing.ToString(),
            Player = new PlayerSaveData
            {
                Name = "TestPlayer",
                Level = 1,
                Experience = 0,
                ExperienceToNext = 100,
                Health = new HealthSaveData { Current = 100, Maximum = 100 },
                Mana = new ManaSaveData { Current = 50, Maximum = 50 },
                Stats = new StatsSaveData(),
                Position = new PositionSaveData { X = 0, Y = 0 }
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

    private static GameSaveData CreateLargeTestGameData()
    {
        var gameData = CreateTestGameData();
        
        // Add more data to test performance
        for (int x = 0; x < gameData.Map.Width; x++)
        {
            var column = new List<TileSaveData>();
            for (int y = 0; y < gameData.Map.Height; y++)
            {
                column.Add(new TileSaveData
                {
                    Type = "Floor",
                    IsWalkable = true,
                    Character = '.'
                });
            }
            gameData.Map.TileData.Add(column);
        }

        // Add test entities
        for (int i = 0; i < 50; i++)
        {
            gameData.Entities.Add(new EntitySaveData
            {
                Id = $"entity_{i}",
                Type = "TestEntity",
                Position = new PositionSaveData { X = i % 80, Y = i % 24 }
            });
        }

        // Add test inventory items
        for (int i = 0; i < 10; i++)
        {
            gameData.Inventory.Add(new ItemSaveData
            {
                Id = $"item_{i}",
                Name = $"Test Item {i}",
                Type = "TestItem"
            });
        }

        return gameData;
    }

    public void Dispose()
    {
        _saveSystem?.Dispose();
        
        if (Directory.Exists(_testSaveDirectory))
        {
            Directory.Delete(_testSaveDirectory, true);
        }
    }
}