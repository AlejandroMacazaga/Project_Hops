using UnityEngine;
using UnityEngine.Serialization;

namespace Entities {
    [RequireComponent(typeof(HealthComponent), typeof(Animator))]

    public class EntityController : MonoBehaviour, IEntityController
    {
        [FormerlySerializedAs("Data")]
        [Header("Entity configuration")]
        [SerializeField] protected EntityData data; 
        [SerializeField] protected Animator animator;
        protected HealthComponent Health;
        public virtual void Awake()
        {
            PrepareHealth();
            animator = GetComponent<Animator>();
        }

        protected virtual void OnDeath()
        {
            // noop
        }

        protected virtual void PrepareHealth()
        {
            if (Health == null) Health = GetComponent<HealthComponent>();
            Health.SetValues(data.healthData);
            Health.OnDeath += OnDeath;
        }

      
    }
}
