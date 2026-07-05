using System;
using System.Collections.Generic;
using UnityEngine;

public static class EventBus
{
    private static readonly Dictionary<Type, List<Delegate>> _events = new();

    public static void Subscribe<T>(Action<T> callback)
    {
        Type type = typeof(T);

        if (!_events.TryGetValue(type, out var list))
        {
            list = new List<Delegate>();
            _events[type] = list;
        }

        list.Add(callback);
    }

    public static void Unsubscribe<T>(Action<T> callback)
    {
        if (_events.TryGetValue(typeof(T), out var list))
        {
            list.Remove(callback);

            if (list.Count == 0)
                _events.Remove(typeof(T));
        }
    }

    public static void Publish<T>(T message)
    {
        if (!_events.TryGetValue(typeof(T), out var list))
            return;

        foreach (var callback in list)
            ((Action<T>)callback)?.Invoke(message);
    }
}
