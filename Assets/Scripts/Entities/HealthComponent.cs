using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Entities
{
    public class HealthComponent : MonoBehaviour, IVisitable
    {
        [SerializeField]
        float healthPoints;

        [SerializeField]
        float maxHealthPoints;

        public bool IsDead => healthPoints <= 0;
        public event UnityAction OnDeath;

        public event UnityAction<float> OnOverheal;
        public void DamageReceived(float damage)
        {
            healthPoints = damage;
            if (healthPoints <= 0) OnDeath?.Invoke();
        }

        public void HealReceived(float heal)
        {
            healthPoints += heal;
            if (healthPoints > maxHealthPoints) OnOverheal?.Invoke(healthPoints - maxHealthPoints);
        }

        public void SetValues(HealthData data)
        {
            healthPoints = data.healthPoints;
            maxHealthPoints = data.maxHealthPoints;
        }


        public void Accept(IVisitor damage)
        {
            damage.Visit(this);
        }
    }

    [Serializable]
    public struct HealthData
    {
        public float healthPoints;

        public float maxHealthPoints;
    }
}

