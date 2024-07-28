using Entities;

namespace Utils.EventBus
{
    public interface IEvent
    {

    }

    public struct TestEvent : IEvent
    {

    }

    public struct PlayerDamageEvent : IEvent
    {
        public EntityController DamageDealer;
        public int Damage;
    }
}