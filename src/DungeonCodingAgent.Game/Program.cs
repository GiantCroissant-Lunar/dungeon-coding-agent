using DungeonCodingAgent.Game.Core;

namespace DungeonCodingAgent.Game;

/// <summary>
/// Main entry point for the Dungeon Coding Agent game.
/// </summary>
internal class Program
{
    private static async Task Main(string[] args)
    {
        Console.WriteLine("Dungeon Coding Agent - Starting Game Engine...");
        
        var gameEngine = new GameEngine();
        
        try
        {
            // Initialize the game engine
            gameEngine.Initialize();
            Console.WriteLine("Game Engine initialized successfully.");
            
            // Subscribe to state changes for demonstration
            gameEngine.StateChanged += (state) => 
                Console.WriteLine($"Game state changed to: {state}");
            
            // Demonstrate state transitions
            Console.WriteLine("Demonstrating state transitions...");
            gameEngine.ChangeState(GameState.Playing);
            await Task.Delay(100);
            
            gameEngine.ChangeState(GameState.Paused);
            await Task.Delay(100);
            
            gameEngine.ChangeState(GameState.Playing);
            await Task.Delay(100);
            
            gameEngine.ChangeState(GameState.MainMenu);
            await Task.Delay(100);
            
            Console.WriteLine("State transitions completed. Engine is ready for integration.");
            
            // Shutdown gracefully
            gameEngine.ChangeState(GameState.Exiting);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error running game engine: {ex.Message}");
        }
        finally
        {
            gameEngine.Shutdown();
            Console.WriteLine("Game Engine shut down.");
        }
    }
}
