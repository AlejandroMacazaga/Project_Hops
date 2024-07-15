using System.Collections.Generic;
using UnityEngine;

namespace Utils.EventChannel
{
    public abstract class EventChannel<T> : ScriptableObject
    {
        public HashSet<EventListener<T>> observers = new();

        public void Invoke(T instance)
        {
            foreach (var observer in observers)
            {
                observer.Raise(instance);
            }
        }

        public void Register(EventListener<T> observer) => observers.Add(observer);
        public void Deregister(EventListener<T> observer) => observers.Remove(observer);
    }
    public readonly struct Empty
    {

    }

    [CreateAssetMenu(menuName = "Events/EventChannel")]
    public class EventChannel : EventChannel<Empty> { }
}