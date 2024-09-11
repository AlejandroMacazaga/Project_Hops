using UnityEngine;

namespace Entities.Attacks
{
    
    public class DamageArea : MonoBehaviour
    {
        public DamageComponent damageComponent;
        
        void OnTriggerEnter (Collider other)
        {
            Debug.Log("Just collided");
            if (other.TryGetComponent<IVisitable>(out var visitable))
            {
                visitable.Accept(damageComponent);
            }
        }
        
        
    }
}