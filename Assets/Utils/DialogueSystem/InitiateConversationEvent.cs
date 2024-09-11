using System;
using Utils.EventBus;

namespace Utils.DialogueSystem
{
    [Serializable] public struct InitiateConversationEvent : IEvent
    {
        public string textId;
        public DialogueData dialogueData;
    }
}
