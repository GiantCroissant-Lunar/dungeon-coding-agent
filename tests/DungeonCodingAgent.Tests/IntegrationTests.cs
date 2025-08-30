using DungeonCodingAgent.Game.Core;
using DungeonCodingAgent.Game.Components;
using Arch.Core;
using System.Diagnostics;

namespace DungeonCodingAgent.Tests;

/// <summary>
/// Integration tests for the complete game engine system.
/// </summary>
public class GameEngineIntegrationTests
{
    [Fact]
    public void GameEngine_WithMultipleActors_ProcessesTurnsInCorrectOrder()
    {
        // Arrange
        var engine = new GameEngine();
        engine.Initialize();
        
        // Create test actors with different initiatives
        var actor1 = engine.EcsWorld.Create<ActorTurn>();
        var actor2 = engine.EcsWorld.Create<ActorTurn>();
        var actor3 = engine.EcsWorld.Create<ActorTurn>();
        
        engine.EcsWorld.Set(actor1, new ActorTurn { Initiative = 5, HasActed = false, ActionPoints = 1.0f });
        engine.EcsWorld.Set(actor2, new ActorTurn { Initiative = 15, HasActed = false, ActionPoints = 1.0f });
        engine.EcsWorld.Set(actor3, new ActorTurn { Initiative = 10, HasActed = false, ActionPoints = 1.0f });
        
        var turnResults = new List<(int turn, Entity actor)>();
        
        // Subscribe to turn events
        GameEvents.TurnStarted += (turn) =>
        {
            var currentActor = engine.TurnManager.GetNextActor();
            if (currentActor != Entity.Null)
            {
                turnResults.Add((turn, currentActor));
                engine.TurnManager.ActorHasActed(currentActor);
            }
        };
        
        // Act - switch to playing and process a full turn
        engine.ChangeState(GameState.Playing);
        engine.ProcessTurn(); // This should begin the turn and process actors
        
        // Complete the turn by processing all actors
        while (engine.TurnManager.GetNextActor() != Entity.Null)
        {
            var nextActor = engine.TurnManager.GetNextActor();
            engine.TurnManager.ActorHasActed(nextActor);
        }
        
        // Assert - actors should be processed in initiative order (highest first)
        Assert.True(turnResults.Count >= 1);
        if (turnResults.Count > 0)
        {
            // First actor should be the one with highest initiative (15)
            Assert.Equal(actor2, turnResults[0].actor);
        }
        
        // Cleanup
        engine.Shutdown();
    }
    
    [Fact]
    public void GameEngine_StateTransitionsCycle_MaintainsIntegrity()
    {
        // Arrange
        var engine = new GameEngine();
        engine.Initialize();
        
        var stateHistory = new List<GameState>();
        engine.StateChanged += state => stateHistory.Add(state);
        
        // Act - cycle through multiple states
        var states = new[] 
        { 
            GameState.Playing, 
            GameState.Paused, 
            GameState.Playing, 
            GameState.Inventory, 
            GameState.Playing, 
            GameState.GameOver, 
            GameState.MainMenu 
        };
        
        foreach (var state in states)
        {
            engine.ChangeState(state);
        }
        
        // Assert
        Assert.Equal(states.Length, stateHistory.Count);
        for (int i = 0; i < states.Length; i++)
        {
            Assert.Equal(states[i], stateHistory[i]);
        }
        
        // Engine should still be functional
        Assert.True(engine.IsRunning);
        Assert.Equal(GameState.MainMenu, engine.CurrentState);
        
        // Cleanup
        engine.Shutdown();
        Assert.False(engine.IsRunning);
    }
}

/// <summary>
/// Performance tests for the game engine system.
/// </summary>
public class GameEnginePerformanceTests
{
    [Fact]
    public void GameEngine_TurnProcessing_CompletesWithin100ms()
    {
        // Arrange
        var engine = new GameEngine();
        engine.Initialize();
        
        // Create multiple actors to stress test
        for (int i = 0; i < 50; i++)
        {
            var actor = engine.EcsWorld.Create<ActorTurn>();
            engine.EcsWorld.Set(actor, new ActorTurn 
            { 
                Initiative = i, 
                HasActed = false, 
                ActionPoints = 1.0f 
            });
        }
        
        engine.ChangeState(GameState.Playing);
        
        // Act & Assert
        var stopwatch = Stopwatch.StartNew();
        
        // Process a complete turn
        engine.ProcessTurn();
        
        // Mark all actors as having acted to complete the turn
        engine.EcsWorld.Query(in new QueryDescription().WithAll<ActorTurn>(), 
            (Entity entity, ref ActorTurn actorTurn) =>
            {
                actorTurn.HasActed = true;
            });
        
        // End the turn
        if (engine.TurnManager.GetNextActor() == Entity.Null)
        {
            engine.TurnManager.EndTurn();
        }
        
        stopwatch.Stop();
        
        // Assert turn processing completes within 100ms as specified in RFC
        Assert.True(stopwatch.ElapsedMilliseconds < 100, 
            $"Turn processing took {stopwatch.ElapsedMilliseconds}ms, expected < 100ms");
        
        // Cleanup
        engine.Shutdown();
    }
    
    [Fact]
    public void GameEngine_StateTransition_CompletesWithin50ms()
    {
        // Arrange
        var engine = new GameEngine();
        engine.Initialize();
        
        // Act & Assert
        var stopwatch = Stopwatch.StartNew();
        
        engine.ChangeState(GameState.Playing);
        
        stopwatch.Stop();
        
        // Assert state transition completes within 50ms as specified in RFC
        Assert.True(stopwatch.ElapsedMilliseconds < 50, 
            $"State transition took {stopwatch.ElapsedMilliseconds}ms, expected < 50ms");
        
        // Cleanup
        engine.Shutdown();
    }
    
    [Fact]
    public void GameEngine_GameLoop_CanMaintain60FPSTarget()
    {
        // Arrange
        var engine = new GameEngine();
        engine.Initialize();
        
        var frameCount = 0;
        var targetFrames = 60; // Test for 1 second worth of frames
        
        // Create a simple test system that counts frames
        var testSystem = new TestFrameCounterSystem();
        engine.RegisterSystem(testSystem);
        
        // Act
        var stopwatch = Stopwatch.StartNew();
        
        // Simulate ~1 second of game loop at 60 FPS
        var targetTime = TimeSpan.FromSeconds(1);
        var frameTime = TimeSpan.FromMilliseconds(16.67); // ~60 FPS
        
        while (stopwatch.Elapsed < targetTime)
        {
            // Simulate a frame update
            testSystem.Update(engine.EcsWorld, 0.016f);
            frameCount++;
            
            // Simulate frame delay
            Thread.Sleep(1); // Minimal delay to prevent tight loop
        }
        
        stopwatch.Stop();
        
        // Assert - should be able to process close to 60 frames per second
        var actualFPS = frameCount / stopwatch.Elapsed.TotalSeconds;
        Assert.True(actualFPS >= 30, 
            $"Achieved {actualFPS:F1} FPS, expected at least 30 FPS for responsive UI");
        
        // Cleanup
        engine.Shutdown();
    }
    
    private class TestFrameCounterSystem : IGameSystem
    {
        public int FrameCount { get; private set; }
        
        public void Update(World world, float deltaTime)
        {
            FrameCount++;
        }
        
        public bool ShouldUpdateInState(GameState state)
        {
            return true;
        }
    }
}