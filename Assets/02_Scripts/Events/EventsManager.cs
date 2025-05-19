using System.Collections.Generic;
using UnityEngine.Events;
using System;

public class EventManager
{
    private class InternalEvent<T> : UnityEvent<T> { }

    private static readonly Dictionary<Type, object> events = new Dictionary<Type, object>();

    public static void Subscribe<T>(UnityAction<T> listener)
    {
        object thisEvent;
        if (events.TryGetValue(typeof(T), out thisEvent))
        {
            ((UnityEvent<T>)thisEvent).AddListener(listener);
        }
        else
        {
            var ev = new InternalEvent<T>();
            ev.AddListener(listener);
            events.Add(typeof(T), ev);
        }
    }

    public static void Unsubscribe<T>(UnityAction<T> listener)
    {
        object thisEvent;
        if (events.TryGetValue(typeof(T), out thisEvent))
        {
            ((UnityEvent<T>)thisEvent).RemoveListener(listener);
        }
    }

    public static void Broadcast<T>(T context) where T : struct
    {
        object thisEvent;
        if (events.TryGetValue(typeof(T), out thisEvent))
        {
            ((UnityEvent<T>)thisEvent).Invoke(context);
        }
    }

    public static void SubscribeOnce<T>(UnityAction<T> listener) where T : struct
    {
        object thisEvent;

        UnityAction<T> internalListener = null;
        internalListener = (T context) =>
        {
            Unsubscribe(internalListener);
            listener.Invoke(context);
        };

        if (events.TryGetValue(typeof(T), out thisEvent))
        {
            ((UnityEvent<T>)thisEvent).AddListener(internalListener);
        }
        else
        {
            var ev = new InternalEvent<T>();
            ev.AddListener(internalListener);
            events.Add(typeof(T), ev);
        }
    }
}
