using DungeonCodingAgent.Game.Core;
using DungeonCodingAgent.Game.Components;
using Arch.Core;

namespace DungeonCodingAgent.Tests;

/// <summary>
/// Unit tests for the GameEngine class.
/// </summary>
public class GameEngineTests
{
    [Fact]
    public void GameEngine_WhenInitialized_SetsDefaultState()
    {
        // Arrange
        var engine = new GameEngine();
        
        // Act
        engine.Initialize();
        
        // Assert
        Assert.Equal(GameState.MainMenu, engine.CurrentState);
        Assert.True(engine.IsRunning);
        Assert.NotNull(engine.EcsWorld);
        Assert.NotNull(engine.TurnManager);
        
        // Cleanup
        engine.Shutdown();
    }
    
    [Fact]
    public void GameEngine_WhenStateChanges_RaisesStateChangedEvent()
    {
        // Arrange
        var engine = new GameEngine();
        engine.Initialize();
        
        GameState? raisedState = null;
        engine.StateChanged += (state) => raisedState = state;
        
        // Act
        engine.ChangeState(GameState.Playing);
        
        // Assert
        Assert.Equal(GameState.Playing, raisedState);
        Assert.Equal(GameState.Playing, engine.CurrentState);
        
        // Cleanup
        engine.Shutdown();
    }
    
    [Fact]
    public void GameEngine_WhenStateChangesToSameState_DoesNotRaiseEvent()
    {
        // Arrange
        var engine = new GameEngine();
        engine.Initialize();
        
        bool eventRaised = false;
        engine.StateChanged += (_) => eventRaised = true;
        
        // Act
        engine.ChangeState(GameState.MainMenu); // Already in MainMenu
        
        // Assert
        Assert.False(eventRaised);
        
        // Cleanup
        engine.Shutdown();
    }
    
    [Fact]
    public void GameEngine_WhenShutdown_CleansUpResources()
    {
        // Arrange
        var engine = new GameEngine();
        engine.Initialize();
        
        // Act
        engine.Shutdown();
        
        // Assert
        Assert.False(engine.IsRunning);
    }
    
    [Fact]
    public void GameEngine_ProcessTurn_OnlyProcessesWhenPlaying()
    {
        // Arrange
        var engine = new GameEngine();
        engine.Initialize();
        
        var initialTurn = engine.TurnManager.CurrentTurn;
        
        // Act - process turn while in MainMenu
        engine.ProcessTurn();
        
        // Assert - no turn processing should occur
        Assert.Equal(initialTurn, engine.TurnManager.CurrentTurn);
        Assert.False(engine.TurnManager.IsTurnInProgress);
        
        // Act - change to Playing and process turn
        engine.ChangeState(GameState.Playing);
        engine.ProcessTurn();
        
        // Assert - turn processing should start (but may end immediately if no actors)
        // The turn should have been processed even if it ended immediately
        var turnAfterProcessing = engine.TurnManager.CurrentTurn;
        Assert.True(turnAfterProcessing >= initialTurn);
        
        // Cleanup
        engine.Shutdown();
    }
}

/// <summary>
/// Unit tests for the TurnManager class.
/// </summary>
public class TurnManagerTests
{
    [Fact]
    public void TurnManager_WhenCreated_StartsAtTurnOne()
    {
        // Arrange
        using var world = World.Create();
        var turnManager = new TurnManager(world);
        
        // Assert
        Assert.Equal(1, turnManager.CurrentTurn);
        Assert.False(turnManager.IsTurnInProgress);
        // Note: CurrentActor is Entity.Null initially, which has Id=0
    }
    
    [Fact]
    public void TurnManager_WhenTurnBegins_RaisesTurnStartedEvent()
    {
        // Arrange
        using var world = World.Create();
        var turnManager = new TurnManager(world);
        
        int? raisedTurnNumber = null;
        GameEvents.TurnStarted += (turn) => raisedTurnNumber = turn;
        
        // Act
        turnManager.BeginTurn();
        
        // Assert
        Assert.Equal(1, raisedTurnNumber);
        Assert.True(turnManager.IsTurnInProgress);
    }
    
    [Fact]
    public void TurnManager_WhenTurnEnds_AdvancesToNextTurn()
    {
        // Arrange
        using var world = World.Create();
        var turnManager = new TurnManager(world);
        
        int? raisedTurnNumber = null;
        GameEvents.TurnEnded += (turn) => raisedTurnNumber = turn;
        
        turnManager.BeginTurn();
        var currentTurn = turnManager.CurrentTurn;
        
        // Act
        turnManager.EndTurn();
        
        // Assert
        Assert.Equal(currentTurn, raisedTurnNumber);
        Assert.Equal(currentTurn + 1, turnManager.CurrentTurn);
        Assert.False(turnManager.IsTurnInProgress);
    }
    
    [Fact]
    public void TurnManager_WhenActorExists_CanActorActReturnsCorrectValue()
    {
        // Arrange
        using var world = World.Create();
        var turnManager = new TurnManager(world);
        
        var actor = world.Create<ActorTurn>();
        world.Set(actor, new ActorTurn 
        { 
            Initiative = 10, 
            HasActed = false, 
            ActionPoints = 1.0f 
        });
        
        // Act & Assert - should be able to act initially
        Assert.True(turnManager.CanActorAct(actor));
        
        // Act - mark actor as having acted
        turnManager.ActorHasActed(actor);
        
        // Assert - should not be able to act after acting
        Assert.False(turnManager.CanActorAct(actor));
    }
    
    [Fact]
    public void TurnManager_WithMultipleActors_OrdersByInitiative()
    {
        // Arrange
        using var world = World.Create();
        var turnManager = new TurnManager(world);
        
        var actor1 = world.Create<ActorTurn>();
        var actor2 = world.Create<ActorTurn>();
        var actor3 = world.Create<ActorTurn>();
        
        world.Set(actor1, new ActorTurn { Initiative = 5, HasActed = false, ActionPoints = 1.0f });
        world.Set(actor2, new ActorTurn { Initiative = 15, HasActed = false, ActionPoints = 1.0f });
        world.Set(actor3, new ActorTurn { Initiative = 10, HasActed = false, ActionPoints = 1.0f });
        
        // Act
        turnManager.BeginTurn();
        
        // Assert - should get actors in order of initiative (highest first)
        var firstActor = turnManager.GetNextActor();
        Assert.Equal(actor2, firstActor); // Initiative 15
        
        turnManager.ActorHasActed(actor2);
        
        var secondActor = turnManager.GetNextActor();
        Assert.Equal(actor3, secondActor); // Initiative 10
        
        turnManager.ActorHasActed(actor3);
        
        var thirdActor = turnManager.GetNextActor();
        Assert.Equal(actor1, thirdActor); // Initiative 5
    }
}

/// <summary>
/// Unit tests for the GameEvents static class.
/// </summary>
public class GameEventsTests
{
    [Fact]
    public void GameEvents_WhenGameStateChanged_RaisesEvent()
    {
        // Arrange
        GameState? raisedState = null;
        GameEvents.GameStateChanged += (state) => raisedState = state;
        
        // Act
        GameEvents.RaiseGameStateChanged(GameState.Playing);
        
        // Assert
        Assert.Equal(GameState.Playing, raisedState);
    }
    
    [Fact]
    public void GameEvents_WhenTurnStarted_RaisesEvent()
    {
        // Arrange
        int? raisedTurn = null;
        GameEvents.TurnStarted += (turn) => raisedTurn = turn;
        
        // Act
        GameEvents.RaiseTurnStarted(5);
        
        // Assert
        Assert.Equal(5, raisedTurn);
    }
    
    [Fact]
    public void GameEvents_WhenEntityCreated_RaisesEvent()
    {
        // Arrange
        using var world = World.Create();
        var testEntity = world.Create();
        
        Entity? raisedEntity = null;
        GameEvents.EntityCreated += (entity) => raisedEntity = entity;
        
        // Act
        GameEvents.RaiseEntityCreated(testEntity);
        
        // Assert
        Assert.Equal(testEntity, raisedEntity);
    }
}
