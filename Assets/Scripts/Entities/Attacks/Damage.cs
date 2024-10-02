using System;
using System.Collections.Generic;
using System.Reflection;
using Entities.Enemies;
using Items;
using Player;
using UnityEngine;

namespace Entities.Attacks
{
    [CreateAssetMenu(menuName = "Visitors/Damage")]
    public class Damage : ScriptableObject, IVisitor
    {
        public float damageAmount;
        public float stunAmount = 0f;
        public Vector3 force = Vector3.zero;

        private Transform _tr;
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
            healthComponent.DamageReceived(damageAmount);
        }

        public void Visit(OnHitDropComponent drop)
        {
            if (effects.Contains(AttackEffect.Bleed) && drop.energy.item.type == EnergyType.Blood) drop.Drop(damageAmount);
        }

        public void Visit(RecollectComponent collect)
        {
            if (effects.Contains(collect.vulnerability)) collect.Collect();
        }

        public void Visit(StunComponent stun)
        {
            stun.Stun(stunAmount);
        }

        public void Visit(PushComponent push)
        {
            var forward = _tr.forward;
            forward.Normalize();
            push.Push(force.magnitude * forward);
        }
        
        

        public bool CanDamage(EntityTeam other)
        {
            return true;
            // TODO : Add checks for what can damage what
        }

        public void SetOwnerTransform(Transform tr)
        {
            _tr = tr;
        }
    }

    public enum AttackEffect
    {
        Stun,
        Bleed,
        Cut,
        
    }
}