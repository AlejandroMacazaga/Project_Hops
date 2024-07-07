using Utils.Flyweight;

namespace Projectiles
{
    public class ProjectileSettings : FlyweightSettings
    {
        public float despawnDelay = 5f;
        public float speed = 10f;
        
        public override Flyweight Create()
        {
            var go = Instantiate(this.prefab);
            go.SetActive(false);
            go.name = prefab.name;
            
            var projectile = prefab.AddComponent<Projectile>();
            projectile.settings = this;
            return projectile;
        }
    }
}