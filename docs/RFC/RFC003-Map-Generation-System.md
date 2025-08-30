# RFC003: Map Generation & Rendering System

## 📋 Metadata
- **RFC Number**: 003
- **Title**: Map Generation & Rendering System
- **Status**: 📝 Draft
- **Author**: System Architect
- **Created**: 2025-08-30
- **Dependencies**: RFC001 (Core Game Loop), RFC002 (Terminal Application Shell)

## 🎯 Objective

Implement a procedural map generation system and rendering engine that creates classic rogue-like dungeon layouts and displays them efficiently in the terminal using ASCII/Unicode characters.

## 📖 Problem Statement

The dungeon crawler needs a map system that:
- Generates procedural dungeon layouts with rooms, corridors, and features
- Provides efficient tile-based rendering in the terminal
- Supports fog-of-war and line-of-sight mechanics
- Manages large maps with viewport/camera systems
- Integrates with ECS for entity positioning and movement validation

## 🏗️ Technical Specification

### **Core Data Structures**

#### **Tile Structure**
```csharp
public struct Tile
{
    public TileType Type { get; set; }
    public bool IsWalkable { get; set; }
    public bool IsTransparent { get; set; }
    public bool IsVisible { get; set; }
    public bool IsExplored { get; set; }
    
    public Tile(TileType type)
    {
        Type = type;
        IsWalkable = type != TileType.Wall;
        IsTransparent = type != TileType.Wall;
        IsVisible = false;
        IsExplored = false;
    }
}
```

#### **GameMap Class**
```csharp
public class GameMap
{
    private readonly Tile[,] tiles;
    public int Width { get; }
    public int Height { get; }
    
    public GameMap(int width, int height)
    public Tile GetTile(int x, int y)
    public void SetTile(int x, int y, Tile tile)
    public bool IsInBounds(int x, int y)
    public bool IsWalkable(int x, int y)
    public IEnumerable<Position> GetWalkableNeighbors(Position pos)
    public void UpdateVisibility(Position viewerPos, int viewDistance)
}
```

### **Map Generation**

#### **Room-Based Generation**
```csharp
public class DungeonGenerator
{
    private readonly Random random;
    
    public GameMap GenerateDungeon(int width, int height, DungeonParameters parameters)
    
    private List<Room> GenerateRooms(int width, int height, int roomCount)
    private void ConnectRooms(GameMap map, List<Room> rooms)
    private void CarveHorizontalTunnel(GameMap map, int x1, int x2, int y)
    private void CarveVerticalTunnel(GameMap map, int y1, int y2, int x)
    private void PlaceFeatures(GameMap map, List<Room> rooms)
}

public struct Room
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    
    public Position Center => new(X + Width / 2, Y + Height / 2);
    public bool Intersects(Room other)
    public IEnumerable<Position> GetFloorPositions()
}

public class DungeonParameters
{
    public int MinRoomSize { get; set; } = 5;
    public int MaxRoomSize { get; set; } = 12;
    public int RoomCount { get; set; } = 8;
    public float RoomDensity { get; set; } = 0.3f;
    public int Seed { get; set; }
}
```

### **Visibility System**

#### **Field of View (FOV)**
```csharp
public static class FieldOfView
{
    public static void ComputeFov(GameMap map, Position origin, int radius, 
        Func<int, int, bool> isBlocked, Action<int, int> markVisible)
    
    private static void CastRay(GameMap map, Position origin, int radius, 
        float startAngle, float endAngle, Func<int, int, bool> isBlocked, 
        Action<int, int> markVisible)
}

public class VisibilitySystem
{
    public void UpdatePlayerVision(GameMap map, EntityReference player, int visionRange)
    public void RevealTile(GameMap map, int x, int y)
    public void HideTile(GameMap map, int x, int y)
    public bool IsVisible(GameMap map, int x, int y)
    public bool IsExplored(GameMap map, int x, int y)
}
```

### **Rendering System**

#### **MapRenderer**
```csharp
public class MapRenderer
{
    private readonly GameMap map;
    private Point cameraPosition;
    private Size viewportSize;
    
    public MapRenderer(GameMap map)
    public void SetCamera(Point position)
    public void SetViewport(Size size)
    public void RenderToView(GameMapView view)
    public char GetTileCharacter(Tile tile)
    public string GetTileColor(Tile tile, bool isVisible, bool isExplored)
}
```

#### **Camera System**
```csharp
public class Camera
{
    public Point Position { get; private set; }
    public Size ViewportSize { get; private set; }
    
    public void CenterOn(Position target)
    public void MoveTo(Point position)
    public Rectangle GetViewBounds()
    public bool IsInView(Position worldPos)
    public Point WorldToScreen(Position worldPos)
    public Position ScreenToWorld(Point screenPos)
}
```

## 🎮 Implementation Details

### **Map Generation Algorithm**
```csharp
public GameMap GenerateDungeon(int width, int height, DungeonParameters parameters)
{
    var map = new GameMap(width, height);
    
    // 1. Fill with walls
    FillWithWalls(map);
    
    // 2. Generate rooms
    var rooms = GenerateRooms(width, height, parameters);
    
    // 3. Carve out rooms
    foreach (var room in rooms)
    {
        CarveRoom(map, room);
    }
    
    // 4. Connect rooms with corridors
    ConnectRooms(map, rooms);
    
    // 5. Place stairs and features
    PlaceStairs(map, rooms);
    PlaceFeatures(map, rooms);
    
    return map;
}
```

### **FOV Algorithm (Shadow Casting)**
```csharp
public static void ComputeFov(GameMap map, Position origin, int radius,
    Func<int, int, bool> isBlocked, Action<int, int> markVisible)
{
    // Mark origin as visible
    markVisible(origin.X, origin.Y);
    
    // Cast rays in 8 octants
    for (int octant = 0; octant < 8; octant++)
    {
        CastLight(map, origin, radius, 1, 1.0, 0.0, octant, isBlocked, markVisible);
    }
}
```

### **Efficient Rendering**
```csharp
public void RenderToView(GameMapView view)
{
    var viewBounds = camera.GetViewBounds();
    var buffer = new StringBuilder();
    
    for (int y = viewBounds.Top; y < viewBounds.Bottom; y++)
    {
        for (int x = viewBounds.Left; x < viewBounds.Right; x++)
        {
            if (map.IsInBounds(x, y))
            {
                var tile = map.GetTile(x, y);
                var character = GetRenderCharacter(x, y, tile);
                buffer.Append(character);
            }
            else
            {
                buffer.Append(' ');
            }
        }
        buffer.AppendLine();
    }
    
    view.SetContent(buffer.ToString());
}
```

## ✅ Acceptance Criteria

### **Functional Requirements**
- [ ] Generate varied dungeon layouts with 6-12 rooms per level
- [ ] Rooms connected by corridors, no isolated areas
- [ ] Stairs placed in accessible locations (up in first room, down in last)
- [ ] FOV system reveals tiles within vision range (typically 8-10 tiles)
- [ ] Fog-of-war hides unseen areas, shows explored tiles in different color
- [ ] Camera follows player smoothly, keeps player centered when possible
- [ ] Map rendering supports viewport sizes from 40x20 to 120x40
- [ ] Handles maps up to 200x200 tiles efficiently

### **Visual Requirements**  
- [ ] Clear visual distinction between walls (#), floors (.), doors (+), stairs (<>)
- [ ] Visible tiles rendered in full color/brightness
- [ ] Explored but not visible tiles rendered dimmed/gray
- [ ] Unexplored tiles not rendered (black/empty)
- [ ] Smooth scrolling when player moves near viewport edges

### **Performance Requirements**
- [ ] Map generation completes in <500ms for 100x100 map
- [ ] FOV calculation completes in <50ms for 10-tile radius
- [ ] Rendering updates complete in <16ms (60 FPS)
- [ ] Memory usage scales linearly with map size
- [ ] No visible lag during player movement

### **Integration Requirements**
- [ ] Integrates with ECS for entity positioning
- [ ] Supports collision detection for movement system
- [ ] Provides spawn points for player and entities
- [ ] Compatible with save/load system (serializable map data)
- [ ] Events fired for map changes and visibility updates

## 🔗 Integration Points

### **Dependencies**
- **RFC001**: Core Game Loop (ECS World, Events)
- **RFC002**: Terminal.Gui Shell (GameMapView rendering target)

### **Dependents**
- **RFC004**: Player Movement (collision detection, position validation)
- **RFC005**: Combat System (line-of-sight for ranged attacks)
- **RFC008**: Save/Load System (map serialization)

### **Events Published**
```csharp
GameEvents.MapGenerated        // New map created
GameEvents.MapChanged          // Map modified
GameEvents.TileRevealed        // FOV reveals new tile
GameEvents.PlayerVisibilityChanged // Player vision updated
```

### **Events Consumed**
```csharp
GameEvents.PlayerMoved         // Update FOV and camera
GameEvents.EntityMoved         // Update entity visibility
GameEvents.NewGameStarted      // Generate new map
```

## 📊 Performance Considerations

### **Memory Optimization**
- Use struct for Tile (value type, cache-friendly)
- Pre-allocate tile arrays to avoid GC pressure  
- Pool StringBuilder objects for rendering
- Cache visibility calculations where possible

### **Rendering Optimization**
- Only render visible viewport area
- Use dirty region tracking to minimize redraws
- Cache character/color lookups
- Batch Terminal.Gui drawing operations

### **Generation Performance**
- Use efficient room placement algorithms
- Minimize array allocations during generation
- Consider iterative vs recursive corridor carving
- Profile and optimize bottleneck areas

## 🧪 Testing Strategy

### **Unit Tests**
```csharp
[Fact]
public void DungeonGenerator_WhenGenerated_AllRoomsConnected()

[Fact] 
public void FieldOfView_WhenCalculated_RevealsCorrectTiles()

[Fact]
public void GameMap_WhenCheckedForWalkable_ReturnsCorrectValue()

[Fact]
public void Camera_WhenCentered_KeepsTargetInView()
```

### **Integration Tests**
- Full map generation and rendering pipeline
- FOV calculation with entity movement
- Performance tests with various map sizes

### **Visual Tests**
- Manual verification of generated layouts
- FOV visualization in different scenarios
- Camera behavior during player movement

## 📝 Implementation Notes

### **Phase 1: Core Map System**
1. Implement Tile, GameMap, and basic room generation
2. Add simple rectangular rooms with corridor connections
3. Basic rendering without FOV

### **Phase 2: Visibility System**  
1. Implement shadow-casting FOV algorithm
2. Add explored/visible tile state management
3. Integrate with rendering system

### **Phase 3: Advanced Features**
1. Improve generation with more room shapes
2. Add doors, secret areas, and special rooms
3. Optimize performance for large maps

### **Algorithm References**
- **Room Generation**: Modified BSP or random placement
- **FOV**: Symmetric shadow casting or recursive ray casting
- **Pathfinding**: A* for corridor generation validation

## 📚 References

- [Rogue Basin - Dungeon Generation](http://roguebasin.roguelikedevelopment.org/index.php/Articles#Dungeon_generation)
- [FOV Using Recursive Shadowcasting](http://roguebasin.roguelikedevelopment.org/index.php/FOV_using_recursive_shadowcasting)
- [ASCII Art and Terminal Graphics](https://en.wikipedia.org/wiki/ASCII_art)

---

## 🎯 Implementation Ready

This RFC provides complete specification for the map generation and rendering system. The implementation can work independently of other game systems and provides the foundation for player movement and gameplay.

**Estimated Effort**: 4-5 days for full implementation including tests
**Risk Level**: Medium - Complex algorithms, performance sensitive
**Priority**: High - Essential for gameplay visualization