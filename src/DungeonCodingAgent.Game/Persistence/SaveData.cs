using System.Text.Json.Serialization;
using DungeonCodingAgent.Game.Core;

namespace DungeonCodingAgent.Game.Persistence;

/// <summary>
/// Complete game save data structure containing all game state
/// </summary>
public class GameSaveData
{
    [JsonPropertyName("version")]
    public string Version { get; set; } = "1.0";
    
    [JsonPropertyName("saveDate")]
    public DateTime SaveDate { get; set; } = DateTime.UtcNow;
    
    [JsonPropertyName("currentTurn")]
    public int CurrentTurn { get; set; }
    
    [JsonPropertyName("gameState")]
    public string GameState { get; set; } = Core.GameState.Playing.ToString();
    
    [JsonPropertyName("player")]
    public PlayerSaveData Player { get; set; } = new();
    
    [JsonPropertyName("map")]
    public MapSaveData Map { get; set; } = new();
    
    [JsonPropertyName("entities")]
    public List<EntitySaveData> Entities { get; set; } = new();
    
    [JsonPropertyName("inventory")]
    public List<ItemSaveData> Inventory { get; set; } = new();
}

/// <summary>
/// Player-specific save data
/// </summary>
public class PlayerSaveData
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "Player";
    
    [JsonPropertyName("level")]
    public int Level { get; set; } = 1;
    
    [JsonPropertyName("experience")]
    public int Experience { get; set; } = 0;
    
    [JsonPropertyName("experienceToNext")]
    public int ExperienceToNext { get; set; } = 100;
    
    [JsonPropertyName("health")]
    public HealthSaveData Health { get; set; } = new();
    
    [JsonPropertyName("mana")]
    public ManaSaveData Mana { get; set; } = new();
    
    [JsonPropertyName("stats")]
    public StatsSaveData Stats { get; set; } = new();
    
    [JsonPropertyName("position")]
    public PositionSaveData Position { get; set; } = new();
}

/// <summary>
/// Map state save data
/// </summary>
public class MapSaveData
{
    [JsonPropertyName("width")]
    public int Width { get; set; } = 80;
    
    [JsonPropertyName("height")]
    public int Height { get; set; } = 24;
    
    [JsonPropertyName("tileData")]
    public List<List<TileSaveData>> TileData { get; set; } = new();
    
    [JsonPropertyName("entityPositions")]
    public Dictionary<string, PositionSaveData> EntityPositions { get; set; } = new();
}

/// <summary>
/// Health state save data
/// </summary>
public class HealthSaveData
{
    [JsonPropertyName("current")]
    public int Current { get; set; } = 100;
    
    [JsonPropertyName("maximum")]
    public int Maximum { get; set; } = 100;
}

/// <summary>
/// Mana state save data
/// </summary>
public class ManaSaveData
{
    [JsonPropertyName("current")]
    public int Current { get; set; } = 50;
    
    [JsonPropertyName("maximum")]
    public int Maximum { get; set; } = 50;
}

/// <summary>
/// Player stats save data
/// </summary>
public class StatsSaveData
{
    [JsonPropertyName("strength")]
    public int Strength { get; set; } = 10;
    
    [JsonPropertyName("dexterity")]
    public int Dexterity { get; set; } = 10;
    
    [JsonPropertyName("intelligence")]
    public int Intelligence { get; set; } = 10;
    
    [JsonPropertyName("constitution")]
    public int Constitution { get; set; } = 10;
    
    [JsonPropertyName("attackPower")]
    public int AttackPower { get; set; } = 10;
    
    [JsonPropertyName("defense")]
    public int Defense { get; set; } = 10;
}

/// <summary>
/// Position save data
/// </summary>
public class PositionSaveData
{
    [JsonPropertyName("x")]
    public int X { get; set; } = 0;
    
    [JsonPropertyName("y")]
    public int Y { get; set; } = 0;
}

/// <summary>
/// Tile save data for map persistence
/// </summary>
public class TileSaveData
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "Floor";
    
    [JsonPropertyName("isWalkable")]
    public bool IsWalkable { get; set; } = true;
    
    [JsonPropertyName("isVisible")]
    public bool IsVisible { get; set; } = false;
    
    [JsonPropertyName("isExplored")]
    public bool IsExplored { get; set; } = false;
    
    [JsonPropertyName("character")]
    public char Character { get; set; } = '.';
}

/// <summary>
/// Entity save data for non-player entities
/// </summary>
public class EntitySaveData
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
    
    [JsonPropertyName("type")]
    public string Type { get; set; } = "Unknown";
    
    [JsonPropertyName("position")]
    public PositionSaveData Position { get; set; } = new();
    
    [JsonPropertyName("health")]
    public HealthSaveData? Health { get; set; }
    
    [JsonPropertyName("renderable")]
    public RenderableSaveData? Renderable { get; set; }
}

/// <summary>
/// Renderable component save data
/// </summary>
public class RenderableSaveData
{
    [JsonPropertyName("character")]
    public char Character { get; set; } = '?';
    
    [JsonPropertyName("foregroundColor")]
    public string ForegroundColor { get; set; } = "White";
    
    [JsonPropertyName("backgroundColor")]
    public string BackgroundColor { get; set; } = "Black";
}

/// <summary>
/// Item save data for inventory and world items
/// </summary>
public class ItemSaveData
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = "Unknown Item";
    
    [JsonPropertyName("description")]
    public string Description { get; set; } = "A mysterious item.";
    
    [JsonPropertyName("type")]
    public string Type { get; set; } = "Generic";
    
    [JsonPropertyName("isStackable")]
    public bool IsStackable { get; set; } = false;
    
    [JsonPropertyName("quantity")]
    public int Quantity { get; set; } = 1;
    
    [JsonPropertyName("position")]
    public PositionSaveData? Position { get; set; } // null if in inventory
}