using UnityEngine;

namespace Entities
{
    
    public class DamageArea : MonoBehaviour
    {
        [SerializeField] private DamageComponent damageComponent;
        
        void OnTriggerEnter(Collider collision)
        {
            if (collision.TryGetComponent<IVisitable>(out var visitable))
            {
                visitable.Accept(damageComponent);
            }
        }
        
        
    }
}