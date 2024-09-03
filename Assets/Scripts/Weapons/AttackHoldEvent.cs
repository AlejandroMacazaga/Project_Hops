using Utils.EventBus;

namespace Weapons
{
    public struct AttackHoldEvent : IEvent
    {
        public float Progress;
    }
}