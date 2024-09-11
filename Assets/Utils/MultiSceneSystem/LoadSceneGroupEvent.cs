using Utils.EventBus;

namespace Utils.MultiSceneSystem
{
    public struct LoadSceneGroupEvent : IEvent
    {
        public int[] ToLoad;
        
    }
}