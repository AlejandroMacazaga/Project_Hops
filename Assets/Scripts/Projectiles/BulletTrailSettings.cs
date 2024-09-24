using UnityEngine;
using Utils.Extensions;
using Utils.Flyweight;

namespace Projectiles
{
    [CreateAssetMenu(menuName = "Effects/BulletTrail")]
    public class BulletTrailSettings : FlyweightSettings
    {
        public float despawnTime = 0.5f;
        
        public override Flyweight Create()
        {
            var go = Instantiate(prefab);
            go.SetActive(false);
            go.name = prefab.name + " (pool " + type +")";
            
            var flyweight = go.GetOrAdd<BulletTrail>();
            flyweight.settings = this;
            
            return flyweight;
        }
    }
}
