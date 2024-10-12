using Entities;
using Items;
using UnityEngine;
using Utils.Flyweight;

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
            var flyweight = (Pickup)FlyweightManager.Spawn(data.pickupSettings);
            flyweight.transform.position = transform.position;
            Destroy(gameObject);
        }
    }
}