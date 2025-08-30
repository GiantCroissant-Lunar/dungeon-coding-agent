using Terminal.Gui;

namespace DungeonCodingAgent.Game.UI;

/// <summary>
/// Provides color schemes for different UI contexts in the dungeon crawler
/// </summary>
public static class DungeonColorSchemes
{
    public static ColorScheme Default { get; }
    public static ColorScheme Game { get; }
    public static ColorScheme Menu { get; }
    public static ColorScheme Status { get; }
    public static ColorScheme Combat { get; }

    static DungeonColorSchemes()
    {
        // Default scheme - green on black for classic terminal feel
        Default = new ColorScheme
        {
            Normal = new Terminal.Gui.Attribute(Color.BrightGreen, Color.Black),
            Focus = new Terminal.Gui.Attribute(Color.BrightYellow, Color.DarkGray),
            HotNormal = new Terminal.Gui.Attribute(Color.BrightCyan, Color.Black),
            HotFocus = new Terminal.Gui.Attribute(Color.BrightWhite, Color.DarkGray),
            Disabled = new Terminal.Gui.Attribute(Color.Gray, Color.Black)
        };

        // Game scheme - for the main game area
        Game = new ColorScheme
        {
            Normal = new Terminal.Gui.Attribute(Color.White, Color.Black),
            Focus = new Terminal.Gui.Attribute(Color.BrightYellow, Color.DarkGray),
            HotNormal = new Terminal.Gui.Attribute(Color.BrightGreen, Color.Black),
            HotFocus = new Terminal.Gui.Attribute(Color.BrightWhite, Color.DarkGray),
            Disabled = new Terminal.Gui.Attribute(Color.Gray, Color.Black)
        };

        // Menu scheme - for menu bars and dialogs
        Menu = new ColorScheme
        {
            Normal = new Terminal.Gui.Attribute(Color.Black, Color.Gray),
            Focus = new Terminal.Gui.Attribute(Color.White, Color.Blue),
            HotNormal = new Terminal.Gui.Attribute(Color.BrightYellow, Color.Gray),
            HotFocus = new Terminal.Gui.Attribute(Color.BrightYellow, Color.Blue),
            Disabled = new Terminal.Gui.Attribute(Color.DarkGray, Color.Gray)
        };

        // Status scheme - for status bars and progress indicators
        Status = new ColorScheme
        {
            Normal = new Terminal.Gui.Attribute(Color.BrightWhite, Color.DarkGray),
            Focus = new Terminal.Gui.Attribute(Color.BrightYellow, Color.Blue),
            HotNormal = new Terminal.Gui.Attribute(Color.BrightGreen, Color.DarkGray),
            HotFocus = new Terminal.Gui.Attribute(Color.BrightWhite, Color.Blue),
            Disabled = new Terminal.Gui.Attribute(Color.Gray, Color.DarkGray)
        };

        // Combat scheme - for combat messages and alerts
        Combat = new ColorScheme
        {
            Normal = new Terminal.Gui.Attribute(Color.BrightRed, Color.Black),
            Focus = new Terminal.Gui.Attribute(Color.BrightYellow, Color.Red),
            HotNormal = new Terminal.Gui.Attribute(Color.BrightMagenta, Color.Black),
            HotFocus = new Terminal.Gui.Attribute(Color.BrightWhite, Color.Red),
            Disabled = new Terminal.Gui.Attribute(Color.Gray, Color.Black)
        };
    }

    /// <summary>
    /// Gets the appropriate color scheme based on terminal capabilities
    /// </summary>
    public static ColorScheme GetColorScheme(bool supportsColor, string context = "default")
    {
        if (!supportsColor)
        {
            // Monochrome fallback
            return new ColorScheme
            {
                Normal = new Terminal.Gui.Attribute(Color.White, Color.Black),
                Focus = new Terminal.Gui.Attribute(Color.Black, Color.White),
                HotNormal = new Terminal.Gui.Attribute(Color.White, Color.Black),
                HotFocus = new Terminal.Gui.Attribute(Color.Black, Color.White),
                Disabled = new Terminal.Gui.Attribute(Color.Gray, Color.Black)
            };
        }

        return context.ToLower() switch
        {
            "game" => Game,
            "menu" => Menu,
            "status" => Status,
            "combat" => Combat,
            _ => Default
        };
    }
}