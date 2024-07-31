using UnityEngine;

namespace Entities
{
    
    public class DamageArea : MonoBehaviour
    {
        [SerializeField] private DamageComponent damageComponent;
        
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