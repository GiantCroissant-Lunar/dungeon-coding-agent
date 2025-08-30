using System.Text;
using Terminal.Gui;

namespace DungeonCodingAgent.Game.UI.Views;

/// <summary>
/// Message types for different kinds of game messages
/// </summary>
public enum MessageType
{
    Normal,
    Combat,
    System,
    Error,
    Success
}

/// <summary>
/// Represents a game message with type and timestamp
/// </summary>
public class GameMessage
{
    public string Text { get; set; } = string.Empty;
    public MessageType Type { get; set; } = MessageType.Normal;
    public DateTime Timestamp { get; set; } = DateTime.Now;

    public GameMessage(string text, MessageType type = MessageType.Normal)
    {
        Text = text;
        Type = type;
        Timestamp = DateTime.Now;
    }
}

/// <summary>
/// Scrollable view that displays game messages
/// </summary>
public class MessageLogView : View
{
    private readonly List<GameMessage> _messages;
    private readonly int _maxMessages;
    private TextView? _textView;

    public MessageLogView()
    {
        ColorScheme = DungeonColorSchemes.Default;
        _messages = new List<GameMessage>();
        _maxMessages = 1000; // Keep last 1000 messages

        InitializeControls();
        AddInitialMessages();
    }

    private void InitializeControls()
    {
        _textView = new TextView()
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            ReadOnly = true,
            WordWrap = true,
            ColorScheme = DungeonColorSchemes.Default
        };

        Add(_textView);
    }

    private void AddInitialMessages()
    {
        // Add some placeholder messages
        AddMessage("Welcome to the Dungeon Coding Agent!", MessageType.System);
        AddMessage("You entered the dungeon", MessageType.Normal);
        AddMessage("A goblin appears!", MessageType.Combat);
        AddMessage("You attack the goblin for 5 damage", MessageType.Combat);
        AddMessage("The goblin dies", MessageType.Success);
        AddMessage("You found a health potion", MessageType.Success);
        AddMessage("Type 'h' for help", MessageType.System);
    }

    public void AddMessage(string message, MessageType type = MessageType.Normal)
    {
        var gameMessage = new GameMessage(message, type);
        AddMessage(gameMessage);
    }

    public void AddMessage(GameMessage message)
    {
        _messages.Add(message);

        // Remove old messages if we exceed the limit
        while (_messages.Count > _maxMessages)
        {
            _messages.RemoveAt(0);
        }

        RefreshDisplay();
        ScrollToBottom();
    }

    public void ClearLog()
    {
        _messages.Clear();
        RefreshDisplay();
    }

    private void RefreshDisplay()
    {
        if (_textView == null) return;

        var sb = new StringBuilder();
        
        foreach (var message in _messages)
        {
            var prefix = GetMessagePrefix(message.Type);
            sb.AppendLine($"{prefix} {message.Text}");
        }

        _textView.Text = sb.ToString();
        _textView.SetNeedsDisplay();
    }

    private string GetMessagePrefix(MessageType type)
    {
        return type switch
        {
            MessageType.Combat => ">",
            MessageType.System => "*",
            MessageType.Error => "!",
            MessageType.Success => "+",
            _ => ">"
        };
    }

    private void ScrollToBottom()
    {
        if (_textView == null) return;

        Application.MainLoop.Invoke(() =>
        {
            var lines = _textView.Lines;
            if (lines > 0)
            {
                // For now, just set the text view to show the latest content
                _textView.SetNeedsDisplay();
            }
        });
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _textView?.Dispose();
        }
        base.Dispose(disposing);
    }

    // Method to get recent messages for testing
    public IReadOnlyList<GameMessage> GetRecentMessages(int count = 10)
    {
        var recentCount = Math.Min(count, _messages.Count);
        return _messages.GetRange(_messages.Count - recentCount, recentCount).AsReadOnly();
    }

    // Method to get message by type for testing
    public IReadOnlyList<GameMessage> GetMessagesByType(MessageType type)
    {
        return _messages.Where(m => m.Type == type).ToList().AsReadOnly();
    }

    // Method to search messages
    public IReadOnlyList<GameMessage> SearchMessages(string searchTerm)
    {
        return _messages
            .Where(m => m.Text.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
            .ToList()
            .AsReadOnly();
    }
}