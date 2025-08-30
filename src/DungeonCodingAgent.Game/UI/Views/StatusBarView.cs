using Terminal.Gui;

namespace DungeonCodingAgent.Game.UI.Views;

/// <summary>
/// Displays player health, mana, level, and experience in a status bar
/// </summary>
public class StatusBarView : View
{
    private ProgressBar? _healthBar;
    private ProgressBar? _manaBar;
    private Label? _levelLabel;
    private Label? _experienceLabel;
    private Label? _turnLabel;

    // Placeholder stats
    private int _currentHealth = 40;
    private int _maxHealth = 50;
    private int _currentMana = 15;
    private int _maxMana = 30;
    private int _level = 2;
    private int _experience = 150;
    private int _experienceToNext = 400;
    private int _turn = 15;

    public StatusBarView()
    {
        ColorScheme = DungeonColorSchemes.Status;
        Height = 2; // Two lines for status information
        InitializeControls();
    }

    private void InitializeControls()
    {
        // Health bar (top line, left side)
        _healthBar = new ProgressBar()
        {
            X = 8,
            Y = 0,
            Width = 20,
            Height = 1,
            ColorScheme = new ColorScheme
            {
                Normal = new Terminal.Gui.Attribute(Color.BrightRed, Color.Black),
                Focus = new Terminal.Gui.Attribute(Color.BrightRed, Color.Black),
                HotNormal = new Terminal.Gui.Attribute(Color.BrightRed, Color.Black),
                HotFocus = new Terminal.Gui.Attribute(Color.BrightRed, Color.Black),
                Disabled = new Terminal.Gui.Attribute(Color.Gray, Color.Black)
            }
        };
        Add(_healthBar);

        // Mana bar (top line, right side)
        _manaBar = new ProgressBar()
        {
            X = Pos.Right(_healthBar) + 9, // 9 for " | Mana: "
            Y = 0,
            Width = 20,
            Height = 1,
            ColorScheme = new ColorScheme
            {
                Normal = new Terminal.Gui.Attribute(Color.BrightBlue, Color.Black),
                Focus = new Terminal.Gui.Attribute(Color.BrightBlue, Color.Black),
                HotNormal = new Terminal.Gui.Attribute(Color.BrightBlue, Color.Black),
                HotFocus = new Terminal.Gui.Attribute(Color.BrightBlue, Color.Black),
                Disabled = new Terminal.Gui.Attribute(Color.Gray, Color.Black)
            }
        };
        Add(_manaBar);

        // Level label (bottom line, left side)
        _levelLabel = new Label()
        {
            X = 0,
            Y = 1,
            Width = 15,
            Height = 1,
            Text = $"Level: {_level}",
            ColorScheme = DungeonColorSchemes.Status
        };
        Add(_levelLabel);

        // Experience label (bottom line, middle)
        _experienceLabel = new Label()
        {
            X = Pos.Right(_levelLabel) + 3,
            Y = 1,
            Width = 25,
            Height = 1,
            Text = $"Exp: {_experience}/{_experienceToNext}",
            ColorScheme = DungeonColorSchemes.Status
        };
        Add(_experienceLabel);

        // Turn label (bottom line, right side)
        _turnLabel = new Label()
        {
            X = Pos.Right(_experienceLabel) + 3,
            Y = 1,
            Width = 15,
            Height = 1,
            Text = $"Turn: {_turn}",
            ColorScheme = DungeonColorSchemes.Status
        };
        Add(_turnLabel);

        UpdateDisplay();
    }

    public void RefreshDisplay()
    {
        DrawStatusBar();
        SetNeedsDisplay();
    }

    private void DrawStatusBar()
    {
        Clear();

        // Draw health text and bar
        Move(0, 0);
        Driver.SetAttribute(ColorScheme.Normal);
        Driver.AddStr($"Health: ");

        // Draw mana text
        var manaX = _healthBar!.X + _healthBar.Width + 1;
        Move(manaX, 0);
        Driver.AddStr($" | Mana: ");

        // Draw health/mana values
        Move(_healthBar.X + _healthBar.Width + 1, 0);
        Driver.AddStr($" {_currentHealth}/{_maxHealth}");

        Move(_manaBar!.X + _manaBar.Width + 1, 0);
        Driver.AddStr($" {_currentMana}/{_maxMana}");
    }

    public void UpdatePlayerStats(int health, int maxHealth, int mana, int maxMana, 
                                  int level, int experience, int experienceToNext, int turn)
    {
        _currentHealth = health;
        _maxHealth = maxHealth;
        _currentMana = mana;
        _maxMana = maxMana;
        _level = level;
        _experience = experience;
        _experienceToNext = experienceToNext;
        _turn = turn;

        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (_healthBar != null)
        {
            _healthBar.Fraction = _maxHealth > 0 ? (float)_currentHealth / _maxHealth : 0f;
        }

        if (_manaBar != null)
        {
            _manaBar.Fraction = _maxMana > 0 ? (float)_currentMana / _maxMana : 0f;
        }

        if (_levelLabel != null)
        {
            _levelLabel.Text = $"Level: {_level}";
        }

        if (_experienceLabel != null)
        {
            _experienceLabel.Text = $"Exp: {_experience}/{_experienceToNext}";
        }

        if (_turnLabel != null)
        {
            _turnLabel.Text = $"Turn: {_turn}";
        }

        SetNeedsDisplay();
    }

    public void UpdateHealth(int current, int max)
    {
        _currentHealth = current;
        _maxHealth = max;
        UpdateDisplay();
    }

    public void UpdateMana(int current, int max)
    {
        _currentMana = current;
        _maxMana = max;
        UpdateDisplay();
    }

    public void UpdateLevel(int level)
    {
        _level = level;
        UpdateDisplay();
    }

    public void UpdateExperience(int current, int toNext)
    {
        _experience = current;
        _experienceToNext = toNext;
        UpdateDisplay();
    }

    public void UpdateTurn(int turn)
    {
        _turn = turn;
        UpdateDisplay();
    }
}