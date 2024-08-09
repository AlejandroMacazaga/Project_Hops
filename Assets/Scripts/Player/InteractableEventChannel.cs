using UnityEngine;
using Utils.EventChannel;

namespace Player
{
    [CreateAssetMenu(menuName = "IntEventChannel")]
    public class InteractableEventChannel : EventChannel<InteractableComponent>
    {
        
    }
}
