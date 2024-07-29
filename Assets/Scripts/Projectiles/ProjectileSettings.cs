using System;
using System.Reflection;
using Entities;
using UnityEditor.Profiling;
using UnityEngine;
using Utils.Extensions;
using Utils.Flyweight;

namespace Projectiles
{
    [CreateAssetMenu(menuName = "Player/Projectile")]
    public class ProjectileSettings : FlyweightSettings, IDamager
    {
        public float despawnDelay = 5f;
        public float speed = 10f;
        public float damage = 5f;
        public override Flyweight Create() {
            var go = Instantiate(prefab);
            go.SetActive(false);
            go.name = prefab.name + " (pool " + type +")";
            
            var flyweight = go.GetOrAdd<Projectile>();
            flyweight.settings = this;
            
            return flyweight;
        }

        public void Damage(object o)
        {
            MethodInfo damageMethod = GetType().GetMethod("Damage", new Type[] { o.GetType() });
            if (damageMethod != null && damageMethod != GetType().GetMethod("Damage", new Type[] { typeof(object) }))
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

        public void Damage(HealthComponent healthComponent)
        {
            healthComponent.DamageReceived(damage);
        }
        
        
    }
}