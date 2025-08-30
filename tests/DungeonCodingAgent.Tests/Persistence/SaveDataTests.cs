using System.Text.Json;
using DungeonCodingAgent.Game.Persistence;

namespace DungeonCodingAgent.Tests.Persistence;

public class SaveDataTests
{
    [Fact]
    public void GameSaveData_Serialization_RoundTripPreservesData()
    {
        // Arrange
        var originalData = new GameSaveData
        {
            Version = "1.0",
            SaveDate = DateTime.UtcNow,
            CurrentTurn = 42,
            GameState = "Playing",
            Player = new PlayerSaveData
            {
                Name = "TestPlayer",
                Level = 5,
                Experience = 150,
                ExperienceToNext = 200,
                Health = new HealthSaveData { Current = 85, Maximum = 100 },
                Mana = new ManaSaveData { Current = 30, Maximum = 50 },
                Stats = new StatsSaveData 
                { 
                    Strength = 12, 
                    Dexterity = 14, 
                    Intelligence = 16,
                    Constitution = 13,
                    AttackPower = 15,
                    Defense = 11
                },
                Position = new PositionSaveData { X = 10, Y = 15 }
            },
            Map = new MapSaveData
            {
                Width = 80,
                Height = 24,
                TileData = new List<List<TileSaveData>>
                {
                    new() { new TileSaveData { Type = "Floor", IsWalkable = true, Character = '.' } }
                },
                EntityPositions = new Dictionary<string, PositionSaveData>
                {
                    { "player", new PositionSaveData { X = 10, Y = 15 } }
                }
            },
            Entities = new List<EntitySaveData>
            {
                new()
                {
                    Id = "entity1",
                    Type = "Monster",
                    Position = new PositionSaveData { X = 5, Y = 8 },
                    Health = new HealthSaveData { Current = 20, Maximum = 25 },
                    Renderable = new RenderableSaveData 
                    { 
                        Character = 'g', 
                        ForegroundColor = "Red", 
                        BackgroundColor = "Black" 
                    }
                }
            },
            Inventory = new List<ItemSaveData>
            {
                new()
                {
                    Id = "potion1",
                    Name = "Health Potion",
                    Description = "Restores health",
                    Type = "Consumable",
                    IsStackable = true,
                    Quantity = 3
                }
            }
        };

        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        // Act - Serialize and deserialize
        var json = JsonSerializer.Serialize(originalData, options);
        var deserializedData = JsonSerializer.Deserialize<GameSaveData>(json, options);

        // Assert
        Assert.NotNull(deserializedData);
        Assert.Equal(originalData.Version, deserializedData.Version);
        Assert.Equal(originalData.CurrentTurn, deserializedData.CurrentTurn);
        Assert.Equal(originalData.GameState, deserializedData.GameState);
        
        // Check player data
        Assert.Equal(originalData.Player.Name, deserializedData.Player.Name);
        Assert.Equal(originalData.Player.Level, deserializedData.Player.Level);
        Assert.Equal(originalData.Player.Health.Current, deserializedData.Player.Health.Current);
        Assert.Equal(originalData.Player.Position.X, deserializedData.Player.Position.X);
        
        // Check map data
        Assert.Equal(originalData.Map.Width, deserializedData.Map.Width);
        Assert.Equal(originalData.Map.Height, deserializedData.Map.Height);
        Assert.Single(deserializedData.Map.TileData);
        
        // Check entities
        Assert.Single(deserializedData.Entities);
        Assert.Equal(originalData.Entities[0].Id, deserializedData.Entities[0].Id);
        Assert.Equal(originalData.Entities[0].Type, deserializedData.Entities[0].Type);
        
        // Check inventory
        Assert.Single(deserializedData.Inventory);
        Assert.Equal(originalData.Inventory[0].Name, deserializedData.Inventory[0].Name);
        Assert.Equal(originalData.Inventory[0].Quantity, deserializedData.Inventory[0].Quantity);
    }

    [Fact]
    public void PlayerSaveData_DefaultValues_AreValid()
    {
        // Arrange & Act
        var playerData = new PlayerSaveData();

        // Assert
        Assert.Equal("Player", playerData.Name);
        Assert.Equal(1, playerData.Level);
        Assert.Equal(0, playerData.Experience);
        Assert.Equal(100, playerData.ExperienceToNext);
        Assert.NotNull(playerData.Health);
        Assert.NotNull(playerData.Mana);
        Assert.NotNull(playerData.Stats);
        Assert.NotNull(playerData.Position);
    }

    [Fact]
    public void MapSaveData_DefaultValues_AreValid()
    {
        // Arrange & Act
        var mapData = new MapSaveData();

        // Assert
        Assert.Equal(80, mapData.Width);
        Assert.Equal(24, mapData.Height);
        Assert.NotNull(mapData.TileData);
        Assert.NotNull(mapData.EntityPositions);
        Assert.Empty(mapData.TileData);
        Assert.Empty(mapData.EntityPositions);
    }

    [Fact]
    public void TileSaveData_DefaultValues_AreValid()
    {
        // Arrange & Act
        var tileData = new TileSaveData();

        // Assert
        Assert.Equal("Floor", tileData.Type);
        Assert.True(tileData.IsWalkable);
        Assert.False(tileData.IsVisible);
        Assert.False(tileData.IsExplored);
        Assert.Equal('.', tileData.Character);
    }

    [Fact]
    public void ItemSaveData_DefaultValues_AreValid()
    {
        // Arrange & Act
        var itemData = new ItemSaveData();

        // Assert
        Assert.Equal(string.Empty, itemData.Id);
        Assert.Equal("Unknown Item", itemData.Name);
        Assert.Equal("A mysterious item.", itemData.Description);
        Assert.Equal("Generic", itemData.Type);
        Assert.False(itemData.IsStackable);
        Assert.Equal(1, itemData.Quantity);
        Assert.Null(itemData.Position);
    }

    [Fact]
    public void EntitySaveData_DefaultValues_AreValid()
    {
        // Arrange & Act
        var entityData = new EntitySaveData();

        // Assert
        Assert.Equal(string.Empty, entityData.Id);
        Assert.Equal("Unknown", entityData.Type);
        Assert.NotNull(entityData.Position);
        Assert.Null(entityData.Health);
        Assert.Null(entityData.Renderable);
    }

    [Fact]
    public void HealthAndManaSaveData_DefaultValues_AreValid()
    {
        // Arrange & Act
        var healthData = new HealthSaveData();
        var manaData = new ManaSaveData();

        // Assert
        Assert.Equal(100, healthData.Current);
        Assert.Equal(100, healthData.Maximum);
        Assert.Equal(50, manaData.Current);
        Assert.Equal(50, manaData.Maximum);
    }

    [Fact]
    public void StatsSaveData_DefaultValues_AreValid()
    {
        // Arrange & Act
        var statsData = new StatsSaveData();

        // Assert
        Assert.Equal(10, statsData.Strength);
        Assert.Equal(10, statsData.Dexterity);
        Assert.Equal(10, statsData.Intelligence);
        Assert.Equal(10, statsData.Constitution);
        Assert.Equal(10, statsData.AttackPower);
        Assert.Equal(10, statsData.Defense);
    }

    [Fact]
    public void RenderableSaveData_DefaultValues_AreValid()
    {
        // Arrange & Act
        var renderableData = new RenderableSaveData();

        // Assert
        Assert.Equal('?', renderableData.Character);
        Assert.Equal("White", renderableData.ForegroundColor);
        Assert.Equal("Black", renderableData.BackgroundColor);
    }

    [Fact]
    public void JsonSerialization_ProducesHumanReadableOutput()
    {
        // Arrange
        var gameData = new GameSaveData
        {
            CurrentTurn = 5,
            Player = new PlayerSaveData { Name = "Hero", Level = 2 }
        };

        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        // Act
        var json = JsonSerializer.Serialize(gameData, options);

        // Assert
        Assert.Contains("\"currentTurn\": 5", json);
        Assert.Contains("\"name\": \"Hero\"", json);
        Assert.Contains("\"level\": 2", json);
        
        // Verify it's properly indented (human-readable)
        Assert.Contains("{\n", json);
        Assert.Contains("  \"", json); // Indentation
    }
}