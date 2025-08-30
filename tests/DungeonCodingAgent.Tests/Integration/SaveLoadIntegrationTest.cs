using DungeonCodingAgent.Game.Core;
using DungeonCodingAgent.Game.Persistence;
using DungeonCodingAgent.Game.Systems;

namespace DungeonCodingAgent.Tests.Integration;

/// <summary>
/// Integration test demonstrating the complete save/load functionality
/// </summary>
public class SaveLoadIntegrationTest
{
    [Fact]
    public async Task CompleteGameSaveLoadCycle_PreservesAllGameState()
    {
        // Arrange - Create a temporary directory for this test
        var testSaveDirectory = Path.Combine(Path.GetTempPath(), "SaveLoadDemo", Guid.NewGuid().ToString());
        using var saveSystem = new SaveGameSystem(testSaveDirectory);
        
        // Create initial game state
        var originalGame = new GameSaveData
        {
            Version = "1.0",
            CurrentTurn = 150,
            GameState = GameState.Playing.ToString(),
            Player = new PlayerSaveData
            {
                Name = "AdvancedPlayer",
                Level = 7,
                Experience = 850,
                ExperienceToNext = 200,
                Health = new HealthSaveData { Current = 85, Maximum = 120 },
                Mana = new ManaSaveData { Current = 30, Maximum = 75 },
                Stats = new StatsSaveData 
                { 
                    Strength = 18, 
                    Dexterity = 16, 
                    Intelligence = 20,
                    Constitution = 14,
                    AttackPower = 22,
                    Defense = 15
                },
                Position = new PositionSaveData { X = 25, Y = 18 }
            },
            Map = new MapSaveData
            {
                Width = 80,
                Height = 24,
                TileData = GenerateTestMapData(),
                EntityPositions = new Dictionary<string, PositionSaveData>
                {
                    { "player", new PositionSaveData { X = 25, Y = 18 } },
                    { "goblin_1", new PositionSaveData { X = 30, Y = 20 } },
                    { "chest_1", new PositionSaveData { X = 40, Y = 15 } }
                }
            },
            Entities = new List<EntitySaveData>
            {
                new()
                {
                    Id = "goblin_1",
                    Type = "Enemy",
                    Position = new PositionSaveData { X = 30, Y = 20 },
                    Health = new HealthSaveData { Current = 15, Maximum = 25 },
                    Renderable = new RenderableSaveData { Character = 'g', ForegroundColor = "Red" }
                },
                new()
                {
                    Id = "chest_1",
                    Type = "Container",
                    Position = new PositionSaveData { X = 40, Y = 15 },
                    Renderable = new RenderableSaveData { Character = 'C', ForegroundColor = "Yellow" }
                }
            },
            Inventory = new List<ItemSaveData>
            {
                new()
                {
                    Id = "health_potion_1",
                    Name = "Health Potion",
                    Description = "Restores 25 HP",
                    Type = "Consumable",
                    IsStackable = true,
                    Quantity = 3
                },
                new()
                {
                    Id = "iron_sword",
                    Name = "Iron Sword",
                    Description = "A sturdy iron blade",
                    Type = "Weapon",
                    IsStackable = false,
                    Quantity = 1
                }
            }
        };

        // Act - Save the game
        var saveResult = await saveSystem.SaveGameAsync(originalGame);
        Assert.True(saveResult, "Save operation should succeed");

        // Verify save file exists
        Assert.True(saveSystem.SaveFileExists(), "Save file should exist after saving");

        // Get save info
        var saveInfo = saveSystem.GetSaveInfo();
        Assert.NotNull(saveInfo);
        Assert.True(saveInfo.Value.FileSize > 0, "Save file should have content");

        // Act - Load the game
        var (loadResult, loadedGame) = await saveSystem.LoadGameAsync();
        
        // Assert - Verify load succeeded
        Assert.True(loadResult, "Load operation should succeed");
        Assert.NotNull(loadedGame);

        // Verify all core game data
        Assert.Equal(originalGame.Version, loadedGame.Version);
        Assert.Equal(originalGame.CurrentTurn, loadedGame.CurrentTurn);
        Assert.Equal(originalGame.GameState, loadedGame.GameState);

        // Verify player data is identical
        var origPlayer = originalGame.Player;
        var loadPlayer = loadedGame.Player;
        
        Assert.Equal(origPlayer.Name, loadPlayer.Name);
        Assert.Equal(origPlayer.Level, loadPlayer.Level);
        Assert.Equal(origPlayer.Experience, loadPlayer.Experience);
        Assert.Equal(origPlayer.ExperienceToNext, loadPlayer.ExperienceToNext);
        
        // Verify health and mana
        Assert.Equal(origPlayer.Health.Current, loadPlayer.Health.Current);
        Assert.Equal(origPlayer.Health.Maximum, loadPlayer.Health.Maximum);
        Assert.Equal(origPlayer.Mana.Current, loadPlayer.Mana.Current);
        Assert.Equal(origPlayer.Mana.Maximum, loadPlayer.Mana.Maximum);
        
        // Verify stats
        Assert.Equal(origPlayer.Stats.Strength, loadPlayer.Stats.Strength);
        Assert.Equal(origPlayer.Stats.Dexterity, loadPlayer.Stats.Dexterity);
        Assert.Equal(origPlayer.Stats.Intelligence, loadPlayer.Stats.Intelligence);
        Assert.Equal(origPlayer.Stats.Constitution, loadPlayer.Stats.Constitution);
        Assert.Equal(origPlayer.Stats.AttackPower, loadPlayer.Stats.AttackPower);
        Assert.Equal(origPlayer.Stats.Defense, loadPlayer.Stats.Defense);
        
        // Verify position
        Assert.Equal(origPlayer.Position.X, loadPlayer.Position.X);
        Assert.Equal(origPlayer.Position.Y, loadPlayer.Position.Y);

        // Verify map data
        Assert.Equal(originalGame.Map.Width, loadedGame.Map.Width);
        Assert.Equal(originalGame.Map.Height, loadedGame.Map.Height);
        Assert.Equal(originalGame.Map.TileData.Count, loadedGame.Map.TileData.Count);
        Assert.Equal(originalGame.Map.EntityPositions.Count, loadedGame.Map.EntityPositions.Count);

        // Verify entities
        Assert.Equal(originalGame.Entities.Count, loadedGame.Entities.Count);
        for (int i = 0; i < originalGame.Entities.Count; i++)
        {
            var origEntity = originalGame.Entities[i];
            var loadEntity = loadedGame.Entities[i];
            
            Assert.Equal(origEntity.Id, loadEntity.Id);
            Assert.Equal(origEntity.Type, loadEntity.Type);
            Assert.Equal(origEntity.Position.X, loadEntity.Position.X);
            Assert.Equal(origEntity.Position.Y, loadEntity.Position.Y);
        }

        // Verify inventory
        Assert.Equal(originalGame.Inventory.Count, loadedGame.Inventory.Count);
        for (int i = 0; i < originalGame.Inventory.Count; i++)
        {
            var origItem = originalGame.Inventory[i];
            var loadItem = loadedGame.Inventory[i];
            
            Assert.Equal(origItem.Id, loadItem.Id);
            Assert.Equal(origItem.Name, loadItem.Name);
            Assert.Equal(origItem.Type, loadItem.Type);
            Assert.Equal(origItem.Quantity, loadItem.Quantity);
            Assert.Equal(origItem.IsStackable, loadItem.IsStackable);
        }

        // Clean up
        if (Directory.Exists(testSaveDirectory))
        {
            Directory.Delete(testSaveDirectory, true);
        }
    }

    private static List<List<TileSaveData>> GenerateTestMapData()
    {
        var tileData = new List<List<TileSaveData>>();
        
        // Create a simple 10x10 test map
        for (int x = 0; x < 10; x++)
        {
            var column = new List<TileSaveData>();
            for (int y = 0; y < 10; y++)
            {
                // Create walls on edges, floor inside
                var isEdge = x == 0 || x == 9 || y == 0 || y == 9;
                column.Add(new TileSaveData
                {
                    Type = isEdge ? "Wall" : "Floor",
                    IsWalkable = !isEdge,
                    Character = isEdge ? '#' : '.',
                    IsExplored = true,
                    IsVisible = x >= 2 && x <= 7 && y >= 2 && y <= 7 // Visible area
                });
            }
            tileData.Add(column);
        }
        
        return tileData;
    }
}