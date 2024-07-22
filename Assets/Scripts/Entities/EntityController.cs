using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

namespace Scripts.Entities {
    [RequireComponent(typeof(HealthComponent), typeof(Animator))]

    public class EntityController : MonoBehaviour, IEntityController
    {
        [Header("Entity configuration")]
        [SerializeField] protected EntityData Data;
        [SerializeField] protected Animator Animator;
        protected HealthComponent Health;
        public virtual void Awake()
        {
            PrepareHealth();
            Animator = GetComponent<Animator>();
        }

        protected virtual void OnDeath()
        {
            // noop
        }

        protected virtual void PrepareHealth()
        {
            if (Health == null) Health = GetComponent<HealthComponent>();
            Health.SetValues(Data.HealthData);
            Health.OnDeath += OnDeath;
        }

      
    }
}
