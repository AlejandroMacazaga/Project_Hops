using Entities;

namespace Enemies
{
    public class EnemyController : EntityController
    {
        private new BasicEnemyData data => (BasicEnemyData)base.data;

        
        public override void Awake() 
        {
            base.Awake();
            Health.OnDeath += OnDeath;
        }

        public void OnDestroy()
        {
            Health.OnDeath -= OnDeath;
        }
        
        public override void OnDeath()
        {
            Destroy(gameObject);
        }
    }
}