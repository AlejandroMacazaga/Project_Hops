using Utils.EventBus;

namespace Player.Events
{
    public struct PlayerIsGoingFast : IEvent
    {
        public bool IsGoingFast;
    }
}
