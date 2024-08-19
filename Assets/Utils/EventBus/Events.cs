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

    public struct AmmoChangeEvent : IEvent
    {
        public readonly int CurrentAmmo;

        public AmmoChangeEvent(int currentAmmo)
        {
            CurrentAmmo = currentAmmo;
        }
    }
}