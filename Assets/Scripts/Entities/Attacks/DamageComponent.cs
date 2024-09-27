using System;
using System.Collections.Generic;
using System.Reflection;
using Items;
using Player;
using UnityEngine;
using UnityEngine.Serialization;

namespace Entities.Attacks
{
    [CreateAssetMenu(menuName = "Entities/DamageComponent")]
    public class DamageComponent : ScriptableObject, IVisitor
    {
        public float damageAmount;
        public List<AttackEffect> effects = new();
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
            if (effects.Contains(AttackEffect.Bleed) && drop.energy.item.type == EnergyType.Blood) drop.Drop();
        }
        
        

        public bool CanDamage(EntityTeam other)
        {
            return true;
            // TODO : Add checks for what can damage what
        }
    }

    public enum AttackEffect
    {
        Stun,
        Bleed,
    }
}