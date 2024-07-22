using System;
using UnityEngine;
using UnityEngine.Events;
namespace Scripts.Entities
{
    public class HealthComponent : MonoBehaviour
    {
        [SerializeField]
        float healthPoints;

        [SerializeField]
        float maxHealthPoints;

        public event UnityAction OnDeath;

        public event UnityAction<float> OnOverheal;
        public void DamageReceived(float damage)
        {
            healthPoints = damage;
            if (healthPoints <= 0) OnDeath.Invoke();
        }

        public void HealReceived(float heal)
        {
            healthPoints += heal;
            if (healthPoints > maxHealthPoints) OnOverheal.Invoke(healthPoints - maxHealthPoints);
        }

        public void SetValues(HealthData data)
        {
            healthPoints = data.HealthPoints;
            maxHealthPoints = data.MaxHealthPoints;
        }


    }

    [Serializable]
    public struct HealthData
    {
        public float HealthPoints,
            MaxHealthPoints;
    }
}

