using UnityEngine;
using UnityEngine.Events;

namespace Utils.EventChannel
{
    public abstract class EventListener<T> : MonoBehaviour
    {
        [SerializeField]
        EventChannel<T> channel;
        [SerializeField]
        UnityEvent<T> unityEvent;

        protected void Awake()
        {
            channel.Register(this);
        }

        protected void OnDestroy()
        {
            channel.Deregister(this);
        }

        public void Raise(T instance)
        {
            unityEvent?.Invoke(instance);
        }
    }

    public class EventListener : EventListener<Empty> { }

}
