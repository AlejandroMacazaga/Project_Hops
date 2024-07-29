using UnityEngine;
using Utils.Extensions;
using Utils.Flyweight;

namespace Projectiles
{
    [CreateAssetMenu(menuName = "Player/Projectile")]
    public class ProjectileSettings : FlyweightSettings
    {
        public float despawnDelay = 5f;
        public float speed = 10f;
        
        public override Flyweight Create() {
            var go = Instantiate(prefab);
            go.SetActive(false);
            go.name = prefab.name;
            
            var flyweight = go.GetOrAdd<Projectile>();
            flyweight.settings = this;
            
            return flyweight;
        }        
    }
}