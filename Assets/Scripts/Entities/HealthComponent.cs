using System;
using UnityEngine;
using UnityEngine.Events;
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
        healthPoints = data.healthPoints;
        maxHealthPoints = data.maxHealthPoints;
    }


}

[Serializable]
public struct HealthData
{
    public float healthPoints,
        maxHealthPoints;
}
