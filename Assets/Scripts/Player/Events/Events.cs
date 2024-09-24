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
}