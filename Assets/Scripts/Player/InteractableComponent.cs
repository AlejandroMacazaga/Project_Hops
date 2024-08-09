using Entities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace Player
{
    public class InteractableComponent : MonoBehaviour, IInteractable
    {
        [SerializeField] private bool isBeingHovered = false;
        public void IsHovered()
        {
            
        }

        public void IsNotHovered()
        {
            
        }

        public void Interact()
        {
            Destroy(gameObject);
        }

    }
    
}
