using Terminal.Gui;
using DungeonCodingAgent.Game.Core;

namespace DungeonCodingAgent.Game.UI;

/// <summary>
/// Main application class that manages Terminal.Gui lifecycle and coordinates with game engine
/// </summary>
public static class DungeonApplication
{
    private static GameEngine? _gameEngine;
    private static MainWindow? _mainWindow;
    private static bool _isInitialized;

    public static void Initialize()
    {
        if (_isInitialized) return;

        try
        {
            InitializeApplication();
        }
        catch (Exception ex)
        {
            HandleGlobalException(ex);
            throw;
        }
    }

    public static void Run()
    {
        if (!_isInitialized)
        {
            throw new InvalidOperationException("Application not initialized. Call Initialize() first.");
        }

        try
        {
            RunApplication();
        }
        catch (Exception ex)
        {
            HandleGlobalException(ex);
            throw;
        }
    }

    public static void Shutdown()
    {
        if (!_isInitialized) return;

        try
        {
            ShutdownApplication();
        }
        catch (Exception ex)
        {
            HandleGlobalException(ex);
        }
    }

    private static void InitializeApplication()
    {
        if (_isInitialized) return;

        // Initialize Terminal.Gui
        InitializeTerminal();

        // Set up color schemes
        SetupColorSchemes();

        // Initialize game engine
        _gameEngine = new GameEngine();
        _gameEngine.Initialize();

        // Create main window
        _mainWindow = new MainWindow();

        // Set up exception handling
        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

        _isInitialized = true;
    }

    private static void RunApplication()
    {
        if (!_isInitialized)
        {
            throw new InvalidOperationException("Application not initialized");
        }

        if (_mainWindow == null)
        {
            throw new InvalidOperationException("Main window not created");
        }

        // Set the main window as the top-level
        Application.Top = _mainWindow;

        // Show initial game state
        _gameEngine?.ChangeState(GameState.MainMenu);

        // Run the application
        Application.Run(_mainWindow);
    }

    private static void ShutdownApplication()
    {
        if (!_isInitialized) return;

        // Shutdown game engine
        _gameEngine?.Shutdown();

        // Dispose main window
        _mainWindow?.Dispose();

        // Remove exception handlers
        AppDomain.CurrentDomain.UnhandledException -= OnUnhandledException;

        // Shutdown Terminal.Gui
        Application.Shutdown();

        _isInitialized = false;
    }

    private static void InitializeTerminal()
    {
        // Initialize Terminal.Gui application
        Application.Init();

        // Configure console encoding for proper Unicode support
        try
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
        }
        catch
        {
            // Fallback if UTF8 is not supported
            Console.OutputEncoding = System.Text.Encoding.ASCII;
        }

        // Check terminal capabilities
        var supportsColor = DetectColorSupport();
        var minWidth = 80;
        var minHeight = 24;

        // Verify minimum terminal size
        if (Console.WindowWidth < minWidth || Console.WindowHeight < minHeight)
        {
            Console.WriteLine($"Warning: Terminal size ({Console.WindowWidth}x{Console.WindowHeight}) " +
                            $"is smaller than recommended minimum ({minWidth}x{minHeight})");
            Console.WriteLine("Some UI elements may not display correctly.");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }
    }

    private static bool DetectColorSupport()
    {
        try
        {
            // Try to detect if the terminal supports colors
            return Application.Driver.SupportsTrueColor || Application.Driver.Colors.HasColors;
        }
        catch
        {
            // Conservative fallback
            return false;
        }
    }

    private static void SetupColorSchemes()
    {
        var supportsColor = DetectColorSupport();

        // Set default color scheme based on terminal capabilities
        var defaultScheme = DungeonColorSchemes.GetColorScheme(supportsColor, "default");
        Colors.Base = defaultScheme;

        // Set color scheme for different contexts
        Colors.Menu = DungeonColorSchemes.GetColorScheme(supportsColor, "menu");
        Colors.Dialog = DungeonColorSchemes.GetColorScheme(supportsColor, "default");
    }

    private static void HandleGlobalException(Exception ex)
    {
        try
        {
            // Try to log the error gracefully
            var errorMessage = $"Fatal error occurred: {ex.Message}";
            
            if (_mainWindow?.MessageLog != null)
            {
                _mainWindow.MessageLog.AddMessage(errorMessage, MessageType.Error);
            }
            else
            {
                // Fallback to console output
                Console.WriteLine(errorMessage);
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }
        catch
        {
            // Last resort - write to console
            Console.WriteLine($"Critical error: {ex.Message}");
        }
    }

    private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception ex)
        {
            HandleGlobalException(ex);
        }

        // If it's terminating, ensure clean shutdown
        if (e.IsTerminating)
        {
            try
            {
                ShutdownApplication();
            }
            catch
            {
                // Ignore errors during emergency shutdown
            }
        }
    }

    public static GameEngine? GetGameEngine()
    {
        return _gameEngine;
    }

    public static MainWindow? GetMainWindow()
    {
        return _mainWindow;
    }

    public static bool IsInitialized()
    {
        return _isInitialized;
    }
}