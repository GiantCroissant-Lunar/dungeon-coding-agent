# 🤖 GitHub Copilot Coding Agent Instructions

## Project Overview
You are working on a classic rogue-like dungeon crawler built with .NET, Arch ECS, and Terminal.Gui v2. This project uses a modular, RFC-driven architecture designed for parallel agent development.

## 🏗️ Architecture Patterns

### **Entity Component System (Arch ECS)**
- Use Arch ECS for all game state and logic
- Follow composition over inheritance
- Keep components as pure data structures
- Implement game logic in systems, not components

```csharp
// Example Component (data only)
public struct Position
{
    public int X { get; set; }
    public int Y { get; set; }
}

// Example System (logic only)  
public class MovementSystem : SystemBase<World, float>
{
    public override void Update(World world, float deltaTime)
    {
        // System implementation
    }
}
```

### **Terminal.Gui v2 Patterns**
- Use Terminal.Gui for all UI components
- Separate UI logic from game logic
- Follow Terminal.Gui's event-driven patterns
- Use Views and Layouts for UI composition

```csharp
// Example Terminal.Gui View
public class GameMapView : View
{
    public GameMapView()
    {
        ColorScheme = Colors.Base;
        CanFocus = true;
    }
    
    public override void OnDrawContent(Rectangle viewport)
    {
        // Rendering implementation
    }
}
```

## 📋 Code Standards

### **Naming Conventions**
- **Namespaces**: `DungeonCodingAgent.{System}.{SubSystem}`
- **Classes**: PascalCase, descriptive names
- **Methods**: PascalCase, verb-based names
- **Variables**: camelCase for local, PascalCase for properties
- **Constants**: UPPER_SNAKE_CASE

### **File Organization**
```
src/DungeonCodingAgent.Game/
├── Systems/           # ECS Systems (game logic)
├── Components/        # ECS Components (data structures)
├── UI/               # Terminal.Gui Views and UI logic
├── Core/             # Game state, constants, utilities
├── Generation/       # Map and content generation
└── Persistence/      # Save/load functionality
```

### **Code Style Guidelines**
- Use `var` for obvious types, explicit types for clarity
- Prefer readonly fields and immutable structures
- Use nullable reference types appropriately
- Follow standard C# formatting (use `dotnet format`)

## 🧪 Testing Requirements

### **Test Structure**
- Create tests alongside implementation
- Follow Arrange-Act-Assert pattern
- Use descriptive test method names
- Test behavior, not implementation details

```csharp
[Fact]
public void MovementSystem_WhenPlayerMovesRight_UpdatesPositionCorrectly()
{
    // Arrange
    var world = World.Create();
    var player = world.Create(new Position { X = 0, Y = 0 });
    
    // Act
    var system = new MovementSystem();
    system.MoveEntity(player, Direction.Right);
    
    // Assert
    var position = world.Get<Position>(player);
    Assert.Equal(1, position.X);
}
```

### **Testing Guidelines**
- Unit test all systems in isolation
- Mock external dependencies
- Test edge cases and error conditions
- Ensure tests are deterministic

## 📝 RFC Implementation Process

### **Before Starting Implementation**
1. **Read the assigned RFC completely**
2. **Understand system boundaries and interfaces**
3. **Identify dependencies on other systems**
4. **Plan implementation approach**

### **Implementation Guidelines**
- Implement interfaces before implementations
- Start with data structures (components)
- Build systems that operate on components
- Create UI components last
- Add tests throughout development

### **Definition of Done**
Each RFC is complete when:
- ✅ All acceptance criteria are met
- ✅ Unit tests achieve >80% coverage
- ✅ Integration points work with existing systems
- ✅ Code follows project standards
- ✅ Documentation is updated

## 🔗 System Integration

### **Inter-System Communication**
- Use events for loose coupling between systems
- Define clear interfaces for system boundaries
- Avoid direct dependencies between systems
- Use dependency injection where appropriate

### **Event System Pattern**
```csharp
public class GameEvents
{
    public static event Action<EntityId, int> PlayerHealthChanged;
    public static event Action<EntityId, Position> PlayerMoved;
    
    public static void RaisePlayerMoved(EntityId player, Position newPosition)
    {
        PlayerMoved?.Invoke(player, newPosition);
    }
}
```

### **Service Registration**
Use constructor injection for dependencies:
```csharp
public class GameEngine
{
    private readonly IMapGenerator mapGenerator;
    private readonly IRenderSystem renderSystem;
    
    public GameEngine(IMapGenerator mapGenerator, IRenderSystem renderSystem)
    {
        this.mapGenerator = mapGenerator;
        this.renderSystem = renderSystem;
    }
}
```

## 🎮 Game-Specific Patterns

### **Turn-Based Logic**
- All actions must be processed in discrete turns
- Use command pattern for player actions
- Implement action queuing for AI entities
- Ensure deterministic turn resolution

### **Map Representation**
```csharp
public struct Tile
{
    public TileType Type { get; set; }
    public bool IsWalkable { get; set; }
    public bool IsVisible { get; set; }
    public bool IsExplored { get; set; }
}

public class GameMap
{
    private readonly Tile[,] tiles;
    public int Width { get; }
    public int Height { get; }
    
    public Tile GetTile(int x, int y) => tiles[x, y];
}
```

### **Rendering System**
- Use Unicode/ASCII characters for visual representation
- Support color schemes through Terminal.Gui
- Implement viewport/camera for large maps
- Cache rendered content for performance

## 🚫 Common Pitfalls to Avoid

### **ECS Anti-Patterns**
- ❌ Don't put logic in components
- ❌ Don't create circular dependencies between systems  
- ❌ Don't access components directly from UI code
- ❌ Don't use inheritance for components

### **Terminal.Gui Mistakes**
- ❌ Don't block the UI thread with game logic
- ❌ Don't bypass Terminal.Gui's event system
- ❌ Don't hardcode colors or layout dimensions
- ❌ Don't forget to dispose of resources properly

### **Performance Considerations**
- ⚡ Cache frequently accessed data
- ⚡ Use object pooling for frequently created objects
- ⚡ Minimize string allocations in render loops
- ⚡ Profile before optimizing

## 📦 Package and Dependency Guidelines

### **Adding New Dependencies**
- Prefer lightweight, focused libraries
- Check compatibility with .NET 8
- Verify license compatibility
- Document dependency rationale in pull request

### **Current Core Dependencies**
- **Terminal.Gui 2.0.0**: UI framework (required)
- **Arch 2.0.0**: ECS framework (required)
- **xUnit**: Testing framework (tests only)

## 🔄 Development Workflow

### **Branch Naming**
- Use `copilot/feature-description` format
- Keep names concise but descriptive
- Example: `copilot/movement-system`, `copilot/combat-engine`

### **Commit Messages**
```
feat(system): implement player movement system

- Add Position and Velocity components
- Implement MovementSystem with collision detection
- Add unit tests for movement boundaries
- Integrate with input handling

Closes #[issue-number]
```

### **Pull Request Requirements**
- Link to relevant RFC
- Include test results
- Demonstrate feature working in terminal
- Update documentation if needed

## 🎯 Success Criteria

### **Code Quality Metrics**
- All tests pass
- Code coverage >80%
- No build warnings
- Follows coding standards

### **Functional Requirements**
- Feature works as specified in RFC
- Integrates properly with existing systems
- Handles edge cases gracefully
- Provides good user experience

## 📚 Resources

### **Documentation Links**
- [Terminal.Gui Documentation](https://gui-cs.github.io/Terminal.Gui/)
- [Arch ECS Documentation](https://github.com/genaray/Arch)
- [.NET 8 Documentation](https://docs.microsoft.com/en-us/dotnet/)

### **Project Context**
- Read `CLAUDE.md` for project overview
- Check `docs/RFC/` for system specifications
- Review existing code for patterns and conventions

## 🚀 RFC Assignment Process

### **IMPORTANT: Preventing Duplicate Work**

**Before starting any RFC implementation:**

1. **Create an Issue First**: When you pick up an RFC, immediately create a GitHub issue with:
   - Title: `"Implement [RFC Number]: [RFC Title]"`  
   - Body: `"Working on RFC [number]. Status: In Progress. ETA: [X] days"`
   - Assign it to yourself
   - Add label: `rfc-implementation`

2. **Check for Existing Issues**: Always check existing issues before starting work
   - Look for issues with `rfc-implementation` label
   - If an RFC is already assigned, choose a different one

3. **Update RFC Status**: In your first commit, update the RFC status:
   - Change `Status: 📝 Draft` to `Status: 🔄 In Progress`
   - Add your name: `Implementer: @your-github-handle`

### **Available RFCs (Ready for Assignment)**

**High Priority - Core Foundation:**
- **RFC001: Core Game Loop** - Game engine, turn management, state system
- **RFC002: Terminal.Gui Shell** - UI framework, application lifecycle  
- **RFC003: Map Generation System** - Dungeon layout, rendering, FOV

**Medium Priority - Essential Gameplay:**
- **RFC004: Player Movement** - Input handling, collision, player entity
- **RFC009: Simple Enemy AI** - Basic enemies with chase/attack behavior
- **RFC006: Basic Inventory** - Item pickup/drop/use system

**Low Priority - Polish Features:**
- **RFC007: Simple Message Log** - Scrolling text display for game events
- **RFC010: Health Status Bar** - Visual health/mana/XP bars
- **RFC008: Save Game Data** - JSON save/load functionality

### **RFC Completion Checklist**

When you finish an RFC:

- [ ] All acceptance criteria checkboxes completed
- [ ] Unit tests written and passing
- [ ] Integration tests verify system works with others
- [ ] Code follows project standards (see above)
- [ ] Documentation updated (XML comments, README if needed)
- [ ] RFC status updated to `Status: ✅ Complete`
- [ ] GitHub issue closed with summary comment

### **Parallel Work Strategy**

**Safe Parallel RFCs** (minimal dependencies):
- RFC007 (Message Log) + RFC010 (Status Bar) - Both UI components  
- RFC006 (Inventory) + RFC008 (Save System) - Independent data systems
- RFC003 (Map Generation) + RFC009 (Enemy AI) - If RFC001 is complete

**Sequential Dependencies:**
- RFC001 → RFC002 → Everything else (core foundation first)
- RFC004 → RFC009 (enemies need player movement)
- RFC002 → RFC007, RFC010 (UI needs application shell)

---

## 🎮 Happy Coding!

Remember: You're building a classic rogue-like experience with modern architecture. Each RFC is designed to be **small, focused, and completable in 1-3 days**. The modular design allows multiple agents to work simultaneously while building a cohesive game.

When in doubt, prioritize:
1. **Functionality** - Does it work as specified in the RFC?
2. **Definition of Done** - Are ALL checkboxes completed?
3. **Integration** - Does it play well with existing systems?
4. **Communication** - Did you claim your RFC via GitHub issue?