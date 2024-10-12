using UnityEngine;
using UnityEngine.Serialization;

namespace Entities {
    [RequireComponent(typeof(HealthComponent), typeof(Animator))]

    public class EntityController : MonoBehaviour, IEntityController
    {
        [Header("Entity configuration")]
        [SerializeField] protected EntityData data; 
        protected HealthComponent Health;
        public virtual void Awake()
        {
            PrepareHealth();
        }

        public virtual void OnDeath()
        {
            // noop
        }

        protected virtual void PrepareHealth()
        {
            if (!Health) Health = GetComponent<HealthComponent>();
            Health.SetValues(data.healthData);
        }
    }
}
