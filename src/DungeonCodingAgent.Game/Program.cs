using DungeonCodingAgent.Game.Core;

Console.WriteLine("=== Dungeon Coding Agent - RFC008 Save Game System Demo ===");
Console.WriteLine();

// Create a mock game engine to demonstrate save/load functionality
using var gameEngine = new GameEngineMock();

Console.WriteLine("Game initialized. Commands:");
Console.WriteLine("  [Space] - Advance turn");
Console.WriteLine("  [Ctrl+S] - Save game");
Console.WriteLine("  [Ctrl+L] - Load game");
Console.WriteLine("  [Q] - Quit");
Console.WriteLine("  [H] - Show this help");
Console.WriteLine();

if (gameEngine.HasSaveFile())
{
    Console.WriteLine("📁 Save file found! Press Ctrl+L to load previous game.");
}
else
{
    Console.WriteLine("🆕 Starting new game...");
}

Console.WriteLine();
Console.WriteLine("Game Status: " + gameEngine.GetGameStatus());
Console.WriteLine();

bool running = true;
while (running)
{
    Console.Write("Input (H for help): ");
    
    var keyInfo = Console.ReadKey(true);
    Console.WriteLine();
    
    switch (keyInfo.Key)
    {
        case ConsoleKey.Spacebar:
            gameEngine.AdvanceGame();
            Console.WriteLine("Game Status: " + gameEngine.GetGameStatus());
            break;
            
        case ConsoleKey.S when keyInfo.Modifiers.HasFlag(ConsoleModifiers.Control):
            Console.WriteLine("💾 Saving game...");
            await gameEngine.HandleInput(ConsoleKey.S, true);
            break;
            
        case ConsoleKey.L when keyInfo.Modifiers.HasFlag(ConsoleModifiers.Control):
            Console.WriteLine("📂 Loading game...");
            await gameEngine.HandleInput(ConsoleKey.L, true);
            Console.WriteLine("Game Status: " + gameEngine.GetGameStatus());
            break;
            
        case ConsoleKey.Q:
            Console.WriteLine("Goodbye!");
            running = false;
            break;
            
        case ConsoleKey.H:
            Console.WriteLine();
            Console.WriteLine("Commands:");
            Console.WriteLine("  [Space] - Advance turn");
            Console.WriteLine("  [Ctrl+S] - Save game");
            Console.WriteLine("  [Ctrl+L] - Load game");
            Console.WriteLine("  [Q] - Quit");
            Console.WriteLine("  [H] - Show this help");
            break;
            
        default:
            Console.WriteLine("Unknown command. Press H for help.");
            break;
    }
    
    Console.WriteLine();
}
