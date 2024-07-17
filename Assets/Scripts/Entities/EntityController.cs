using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

namespace Assets.Scripts.Entities {
    [RequireComponent(typeof(HealthComponent))]

    public class EntityController : MonoBehaviour, IEntityController
    {
        [Header("Entity configuration")]
        [SerializeField] protected EntityData Data;
        [SerializeField] protected AnimatorController Animator;
        protected HealthComponent Health;
        public virtual void Awake()
        {
            PrepareHealth();
        }

        protected virtual void OnDeath()
        {
            // noop
        }

        protected virtual void PrepareHealth()
        {
            if (Health == null) Health = GetComponent<HealthComponent>();
            Health.SetValues(Data.healthData);
            Health.OnDeath += OnDeath;
        }

    }
}
