namespace DungeonCodingAgent.Game.Components;

/// <summary>
/// Player identity and progression component
/// </summary>
public struct Player
{
    public string Name { get; set; }
    public int Level { get; set; }
    public int Experience { get; set; }
    public int ExperienceToNext { get; set; }
    
    public Player(string name)
    {
        Name = name;
        Level = 1;
        Experience = 0;
        ExperienceToNext = 100;
    }
}

/// <summary>
/// Health management component  
/// </summary>
public struct Health
{
    public int Current { get; set; }
    public int Maximum { get; set; }
    public bool IsDead => Current <= 0;
    
    public Health(int maximum)
    {
        Current = maximum;
        Maximum = maximum;
    }
}

/// <summary>
/// Mana management component
/// </summary>
public struct Mana
{
    public int Current { get; set; }
    public int Maximum { get; set; }
    
    public Mana(int maximum)
    {
        Current = maximum;
        Maximum = maximum;
    }
}

/// <summary>
/// Player-specific attributes component
/// </summary>
public struct PlayerStats
{
    public int Strength { get; set; }
    public int Dexterity { get; set; }
    public int Intelligence { get; set; }
    public int Constitution { get; set; }
    public int AttackPower { get; set; }
    public int Defense { get; set; }
    
    public PlayerStats(int baseValue = 10)
    {
        Strength = baseValue;
        Dexterity = baseValue;
        Intelligence = baseValue;
        Constitution = baseValue;
        AttackPower = baseValue;
        Defense = baseValue;
    }
}

/// <summary>
/// Renderable component for visual representation
/// </summary>
public struct Renderable
{
    public char Character { get; set; }
    public ConsoleColor ForegroundColor { get; set; }
    public ConsoleColor BackgroundColor { get; set; }
    
    public Renderable(char character, ConsoleColor foreground = ConsoleColor.White, ConsoleColor background = ConsoleColor.Black)
    {
        Character = character;
        ForegroundColor = foreground;
        BackgroundColor = background;
    }
}

/// <summary>
/// Turn order component for turn-based gameplay
/// </summary>
public struct TurnOrder
{
    public int Initiative { get; set; }
    public bool HasActed { get; set; }
    
    public TurnOrder(int initiative)
    {
        Initiative = initiative;
        HasActed = false;
    }
}