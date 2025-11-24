using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// EventBus estático, tipado. Permite suscribir, desuscribir y publicar eventos de tipo T.
/// Uso: EventBus.Subscribe&lt;MyEvent&gt;(OnMyEvent); EventBus.Publish(new MyEvent{...});
/// </summary>

namespace Project.Core
{
    public static class EventBus
    {
        private static readonly Dictionary<Type, Delegate> subscribers = new Dictionary<Type, Delegate>();

        public static void Subscribe<T>(Action<T> handler)
        {
            var type = typeof(T);
            if (subscribers.TryGetValue(type, out var existing))
                subscribers[type] = Delegate.Combine(existing, handler);
            else
                subscribers[type] = handler;
        }

        public static void Unsubscribe<T>(Action<T> handler)
        {
            var type = typeof(T);
            if (!subscribers.TryGetValue(type, out var existing)) return;
            var updated = Delegate.Remove(existing, handler);
            if (updated == null) subscribers.Remove(type);
            else subscribers[type] = updated;
        }

        public static void Publish<T>(T payload)
        {
            var type = typeof(T);
            if (subscribers.TryGetValue(type, out var d))
            {

                var handlers = d as Action<T>;
                try
                {
                    handlers?.Invoke(payload);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"EventBus handler for {type.Name} threw: {ex}");
                }
            }
        }


        public static void ClearAll()
        {
            subscribers.Clear();
        }
    }
}