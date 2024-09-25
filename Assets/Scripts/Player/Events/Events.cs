using Items;
using Utils.EventBus;

namespace Player.Events
{
    public struct PlayerHealthChange : IEvent
    {
        
    }

    public struct PlayerStaminaChange : IEvent
    {
        public float Current;
    }
    
    public struct PlayerIsGoingFast : IEvent
    {
        public bool IsGoingFast;
    }

    public struct ReaperBloodChange : IEvent
    {
        public int Current;
    }

    public struct KickScreen : IEvent
    {
        public float Amount;
    }

    public struct ItemPickupEvent : IEvent
    {
        public BaseItem Item;
    }

    public struct EnergyPickupEvent : IEvent
    {
        public EnergyItem Item;
    }
    
    
    
}