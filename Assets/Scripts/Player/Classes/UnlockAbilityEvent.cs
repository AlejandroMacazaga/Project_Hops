using System;
using UnityEngine.Serialization;
using Utils.EventBus;

namespace Player.Classes
{
    public struct UnlockAbilityEvent : IEvent
    {
        public Ability Ability;
    }
    public struct LockAbilityEvent : IEvent
    {
        public Ability Ability;
    }
}