namespace DungeonCodingAgent.Game.Core;

/// <summary>
/// Helper class for managing weak event subscriptions to prevent memory leaks.
/// </summary>
public static class WeakEventManager
{
    /// <summary>
    /// Subscribe to an event with a weak reference to prevent memory leaks.
    /// </summary>
    /// <typeparam name="T">Event argument type.</typeparam>
    /// <param name="target">The target object containing the handler method.</param>
    /// <param name="handler">The handler method.</param>
    /// <param name="subscribe">Action to subscribe to the event.</param>
    /// <param name="unsubscribe">Action to unsubscribe from the event.</param>
    public static void Subscribe<T>(object target, Action<T> handler, 
        Action<Action<T>> subscribe, Action<Action<T>> unsubscribe)
    {
        var weakRef = new WeakReference(target);
        Action<T>? weakHandler = null;
        
        weakHandler = (arg) =>
        {
            if (weakRef.Target is object actualTarget)
            {
                handler(arg);
            }
            else if (weakHandler != null)
            {
                // Target has been collected, unsubscribe
                unsubscribe(weakHandler);
            }
        };
        
        subscribe(weakHandler);
    }
    
    /// <summary>
    /// Subscribe to an event with no arguments using weak reference.
    /// </summary>
    /// <param name="target">The target object containing the handler method.</param>
    /// <param name="handler">The handler method.</param>
    /// <param name="subscribe">Action to subscribe to the event.</param>
    /// <param name="unsubscribe">Action to unsubscribe from the event.</param>
    public static void Subscribe(object target, Action handler, 
        Action<Action> subscribe, Action<Action> unsubscribe)
    {
        var weakRef = new WeakReference(target);
        Action? weakHandler = null;
        
        weakHandler = () =>
        {
            if (weakRef.Target is object actualTarget)
            {
                handler();
            }
            else if (weakHandler != null)
            {
                // Target has been collected, unsubscribe
                unsubscribe(weakHandler);
            }
        };
        
        subscribe(weakHandler);
    }
}