using UnityEngine;
using UnityEngine.Serialization;

namespace Entities.Attacks
{
    
    public class DamageArea : MonoBehaviour
    {
        [FormerlySerializedAs("damageComponent")] public Damage damage;
        
        void OnTriggerEnter (Collider other)
        {
            Debug.Log("Just collided");
            if (other.TryGetComponent<IVisitable>(out var visitable))
            {
                visitable.Accept(damage);
            }
        }
        
        
    }
}