using Utils.EventBus;

namespace Utils.MultiSceneSystem
{
    public struct ZoneChangeEvent : IEvent
    {
        public int ToLoad;
    }
}