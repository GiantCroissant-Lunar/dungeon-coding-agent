using Terminal.Gui;

namespace DungeonCodingAgent.Game.UI.Views;

/// <summary>
/// View component that renders the game map
/// </summary>
public class GameMapView : View
{
    private readonly string[,] _mapData;
    private int _cameraX;
    private int _cameraY;
    private int _mapWidth;
    private int _mapHeight;

    public GameMapView()
    {
        ColorScheme = DungeonColorSchemes.Game;
        CanFocus = true;
        
        // Initialize with placeholder dungeon
        _mapWidth = 40;
        _mapHeight = 20;
        _mapData = new string[_mapHeight, _mapWidth];
        InitializePlaceholderMap();
    }

    private void InitializePlaceholderMap()
    {
        // Create a simple placeholder dungeon for testing
        for (int y = 0; y < _mapHeight; y++)
        {
            for (int x = 0; x < _mapWidth; x++)
            {
                if (x == 0 || x == _mapWidth - 1 || y == 0 || y == _mapHeight - 1)
                {
                    _mapData[y, x] = "#"; // Walls
                }
                else if (x == _mapWidth / 2 && y == _mapHeight / 2)
                {
                    _mapData[y, x] = "@"; // Player
                }
                else if ((x + y) % 7 == 0)
                {
                    _mapData[y, x] = "#"; // Scattered walls
                }
                else if ((x + y) % 13 == 0)
                {
                    _mapData[y, x] = "g"; // Goblins
                }
                else if ((x + y) % 11 == 0)
                {
                    _mapData[y, x] = "."; // Items
                }
                else
                {
                    _mapData[y, x] = " "; // Empty floor
                }
            }
        }

        // Add some rooms
        for (int roomY = 3; roomY < 8; roomY++)
        {
            for (int roomX = 3; roomX < 12; roomX++)
            {
                _mapData[roomY, roomX] = ".";
            }
        }

        for (int roomY = 12; roomY < 17; roomY++)
        {
            for (int roomX = 25; roomX < 35; roomX++)
            {
                _mapData[roomY, roomX] = ".";
            }
        }

        // Add corridors
        for (int corridorX = 12; corridorX < 25; corridorX++)
        {
            _mapData[5, corridorX] = ".";
            _mapData[14, corridorX] = ".";
        }
        
        for (int corridorY = 5; corridorY < 14; corridorY++)
        {
            _mapData[corridorY, 12] = ".";
            _mapData[corridorY, 25] = ".";
        }
    }

    public void RefreshDisplay()
    {
        DrawContent();
        SetNeedsDisplay();
    }

    private void DrawContent()
    {
        Clear();

        var viewport = Bounds;
        int startX = Math.Max(0, _cameraX);
        int startY = Math.Max(0, _cameraY);
        int endX = Math.Min(_mapWidth, _cameraX + viewport.Width);
        int endY = Math.Min(_mapHeight, _cameraY + viewport.Height);

        for (int y = startY; y < endY; y++)
        {
            for (int x = startX; x < endX; x++)
            {
                if (y < _mapHeight && x < _mapWidth)
                {
                    var symbol = _mapData[y, x];
                    var screenX = x - _cameraX;
                    var screenY = y - _cameraY;

                    if (screenX >= 0 && screenX < viewport.Width && screenY >= 0 && screenY < viewport.Height)
                    {
                        // Set color based on symbol
                        var attr = GetSymbolAttribute(symbol);
                        Move(screenX, screenY);
                        Driver.SetAttribute(attr);
                        Driver.AddRune(symbol[0]);
                    }
                }
            }
        }
    }

    private Terminal.Gui.Attribute GetSymbolAttribute(string symbol)
    {
        return symbol switch
        {
            "#" => new Terminal.Gui.Attribute(Color.BrightYellow, Color.Black), // Walls
            "@" => new Terminal.Gui.Attribute(Color.BrightWhite, Color.Black), // Player
            "g" => new Terminal.Gui.Attribute(Color.BrightRed, Color.Black),   // Goblins
            "." => new Terminal.Gui.Attribute(Color.Green, Color.Black),       // Floor/Items
            _ => new Terminal.Gui.Attribute(Color.Gray, Color.Black)           // Default
        };
    }

    public void UpdateMap(string[,] newMapData)
    {
        if (newMapData != null)
        {
            _mapHeight = newMapData.GetLength(0);
            _mapWidth = newMapData.GetLength(1);
            
            for (int y = 0; y < _mapHeight; y++)
            {
                for (int x = 0; x < _mapWidth; x++)
                {
                    _mapData[y, x] = newMapData[y, x];
                }
            }
        }
        SetNeedsDisplay();
    }

    public void CenterCamera(int centerX, int centerY)
    {
        var viewWidth = Bounds.Width;
        var viewHeight = Bounds.Height;

        _cameraX = Math.Max(0, Math.Min(_mapWidth - viewWidth, centerX - viewWidth / 2));
        _cameraY = Math.Max(0, Math.Min(_mapHeight - viewHeight, centerY - viewHeight / 2));

        SetNeedsDisplay();
    }

    public void MoveCamera(int deltaX, int deltaY)
    {
        _cameraX = Math.Max(0, Math.Min(_mapWidth - Bounds.Width, _cameraX + deltaX));
        _cameraY = Math.Max(0, Math.Min(_mapHeight - Bounds.Height, _cameraY + deltaY));
        SetNeedsDisplay();
    }
}