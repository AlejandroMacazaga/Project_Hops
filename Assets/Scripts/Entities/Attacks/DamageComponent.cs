using System;
using System.Reflection;
using Player;
using UnityEngine;

namespace Entities.Attacks
{
    [CreateAssetMenu(menuName = "Entities/DamageComponent")]
    public class DamageComponent : ScriptableObject, IVisitor
    {
        public float damageAmount;
        
        public void Visit(object o)
        {
            MethodInfo damageMethod = GetType().GetMethod("Visit", new Type[] { o.GetType() });
            if (damageMethod != null && damageMethod != GetType().GetMethod("Visit", new Type[] { typeof(object) }))
            {
                damageMethod.Invoke(this, new object[] { o });
            }
            else
            {
                DefaultDamage();
            }
        }

        private void DefaultDamage()
        {
            Debug.Log("Default damage method");
        }

        public void Visit(HealthComponent healthComponent)
        {
            Debug.Log("Visit health");
            healthComponent.DamageReceived(damageAmount);
        }

        public void Visit(OnHitDropComponent drop)
        {
            Debug.Log("Visit drop");
            drop.Drop();
        }
        
        

        public bool CanDamage(EntityTeam other)
        {
            return true;
            // TODO : Add checks for what can damage what
        }
    }
}