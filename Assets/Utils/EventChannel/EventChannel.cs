using System.Collections.Generic;
using UnityEngine;

namespace Utils.EventChannel
{
    public abstract class EventChannel<T> : ScriptableObject
    {
        public readonly HashSet<EventListener<T>> Observers = new();

        public void Invoke(T instance)
        {
            foreach (var observer in Observers)
            {
                observer.Raise(instance);
            }
        }

        public void Register(EventListener<T> observer) => Observers.Add(observer);
        public void Deregister(EventListener<T> observer) => Observers.Remove(observer);
    }
    public readonly struct Empty
    {

    }

    [CreateAssetMenu(menuName = "Events/EventChannel")]
    public class EventChannel : EventChannel<Empty> { }
}