using System;
using UnityEngine;
using Utils.EventBus;

namespace Utils.DialogueSystem
{
    public class PruebaClick : MonoBehaviour
    {
        [SerializeField] private InitiateConversationEvent @event;
        private void Start()
        {
            RaiseEvent();
        }

        public void RaiseEvent()
        {
            EventBus<InitiateConversationEvent>.Raise(@event);
        }

    
    }
}
