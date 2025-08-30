using DungeonCodingAgent.Game.UI;

namespace DungeonCodingAgent.Game;

/// <summary>
/// Main entry point for the Dungeon Coding Agent game
/// </summary>
public static class Program
{
    public static void Main(string[] args)
    {
        try
        {
            DungeonApplication.Initialize();
            DungeonApplication.Run();
        }
        catch (Exception ex)
        {
            HandleCriticalError(ex);
        }
        finally
        {
            DungeonApplication.Shutdown();
        }
    }

    private static void HandleCriticalError(Exception ex)
    {
        // Log error and show user-friendly message
        Console.WriteLine($"Fatal error: {ex.Message}");
        Console.WriteLine($"Stack trace: {ex.StackTrace}");
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}
