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
        public int force = 0;
        public PushDirection direction = PushDirection.Forward;
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
            if (!CanDamage(healthComponent.team)) return;
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
            if (!effects.Contains(AttackEffect.Stun)) return;
            if (!CanDamage(stun.team)) return;
            stun.Stun(stunAmount);
        }

        public void Visit(PushComponent push)
        {
            if (force <= 0) return;
            if (!CanDamage(push.team)) return;
            var where = GetDirection(direction);
            where.Normalize();
            push.Push(force * where);
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

        private Vector3 GetDirection(PushDirection d)
        {
            return d switch
            {
                PushDirection.Forward => _tr.forward,
                PushDirection.Backward => -_tr.forward,
                PushDirection.Left => -_tr.right,
                PushDirection.Right => _tr.right,
                _ => throw new ArgumentOutOfRangeException(nameof(d), d, null)
            };
        }
    }

    public enum AttackEffect
    {
        Stun,
        Bleed,
        Cut,
        
    }

    public enum PushDirection
    {
        Forward,
        Backward,
        Left,
        Right
    }
}