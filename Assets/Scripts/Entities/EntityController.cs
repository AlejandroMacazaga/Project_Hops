using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Entities {
    [RequireComponent(typeof(HealthComponent))]
    public class EntityController : MonoBehaviour
    {
        [SerializeField] protected EntityData Data;
        [SerializeField] protected HealthComponent Health;
        private void Awake()
        {
            Health = gameObject.AddComponent<HealthComponent>();
            Health.SetValues(Data.healthData);
            Health.OnDeath += OnDeath;
        }

        protected virtual void OnDeath()
        {
            // noop
        }

    }
}
