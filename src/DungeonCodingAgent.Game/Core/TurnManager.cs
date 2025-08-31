using Arch.Core;
using Arch.Core.Utils;
using DungeonCodingAgent.Game.Components;

namespace DungeonCodingAgent.Game.Core;

/// <summary>
/// Manages turn-based game logic, tracking current turn and actor initiative order.
/// </summary>
public class TurnManager
{
    private readonly World _world;
    private readonly List<Entity> _actorQueue = new();
    
    /// <summary>
    /// Current turn number.
    /// </summary>
    public int CurrentTurn { get; private set; } = 1;
    
    /// <summary>
    /// Current actor in the turn order.
    /// </summary>
    public Entity CurrentActor { get; private set; }
    
    /// <summary>
    /// Whether a turn is currently being processed.
    /// </summary>
    public bool IsTurnInProgress { get; private set; }
    
    /// <summary>
    /// Initializes a new TurnManager.
    /// </summary>
    /// <param name="world">The ECS world to manage turns for.</param>
    public TurnManager(World world)
    {
        _world = world ?? throw new ArgumentNullException(nameof(world));
    }
    
    /// <summary>
    /// Begins a new turn, determining turn order and setting up actor queue.
    /// </summary>
    public void BeginTurn()
    {
        if (IsTurnInProgress)
        {
            return; // Turn already in progress
        }
        
        IsTurnInProgress = true;
        BuildActorQueue();
        
        // Reset all actors' HasActed flag
        _world.Query(in new QueryDescription().WithAll<ActorTurn>(), (Entity entity, ref ActorTurn actorTurn) =>
        {
            actorTurn.HasActed = false;
        });
        
        GameEvents.RaiseTurnStarted(CurrentTurn);
        
        // Set first actor if any exist
        if (_actorQueue.Count > 0)
        {
            CurrentActor = _actorQueue[0];
        }
    }
    
    /// <summary>
    /// Ends the current turn and advances to the next turn.
    /// </summary>
    public void EndTurn()
    {
        if (!IsTurnInProgress)
        {
            return;
        }
        
        GameEvents.RaiseTurnEnded(CurrentTurn);
        
        CurrentTurn++;
        IsTurnInProgress = false;
        _actorQueue.Clear();
        CurrentActor = Entity.Null;
    }
    
    /// <summary>
    /// Gets the next actor in the turn order.
    /// </summary>
    /// <returns>The next actor entity, or Entity.Null if no more actors.</returns>
    public Entity GetNextActor()
    {
        if (_actorQueue.Count == 0)
        {
            return Entity.Null;
        }
        
        // Find next actor that hasn't acted
        for (int i = 0; i < _actorQueue.Count; i++)
        {
            var actor = _actorQueue[i];
            if (_world.TryGet<ActorTurn>(actor, out var actorTurn) && !actorTurn.HasActed)
            {
                CurrentActor = actor;
                return actor;
            }
        }
        
        // All actors have acted, end turn
        return Entity.Null;
    }
    
    /// <summary>
    /// Checks if the specified actor can act in the current turn.
    /// </summary>
    /// <param name="actor">The actor entity to check.</param>
    /// <returns>True if the actor can act, false otherwise.</returns>
    public bool CanActorAct(Entity actor)
    {
        if (!_world.TryGet<ActorTurn>(actor, out var actorTurn))
        {
            return false;
        }
        
        return !actorTurn.HasActed && actorTurn.ActionPoints > 0;
    }
    
    /// <summary>
    /// Marks an actor as having acted in the current turn.
    /// </summary>
    /// <param name="actor">The actor that has acted.</param>
    public void ActorHasActed(Entity actor)
    {
        if (_world.TryGet<ActorTurn>(actor, out var actorTurn))
        {
            actorTurn.HasActed = true;
            _world.Set(actor, actorTurn);
        }
    }
    
    /// <summary>
    /// Builds the actor queue for the current turn, ordered by initiative.
    /// </summary>
    private void BuildActorQueue()
    {
        _actorQueue.Clear();
        
        var actors = new List<(Entity entity, int initiative)>();
        
        _world.Query(in new QueryDescription().WithAll<ActorTurn>(), (Entity entity, ref ActorTurn actorTurn) =>
        {
            actors.Add((entity, actorTurn.Initiative));
        });
        
        // Sort by initiative (highest first)
        actors.Sort((a, b) => b.initiative.CompareTo(a.initiative));
        
        foreach (var (entity, _) in actors)
        {
            _actorQueue.Add(entity);
        }
    }
}