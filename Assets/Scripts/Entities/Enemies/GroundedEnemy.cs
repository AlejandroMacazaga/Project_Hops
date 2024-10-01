using KBCore.Refs;
using UnityEngine;
using UnityEngine.Serialization;

namespace Entities.Enemies
{
    [RequireComponent(typeof(HealthComponent))]
    [RequireComponent(typeof(EnemyMovement))]
    public class ChaseEnemy : ValidatedMonoBehaviour, IEnemy
    {
        [SerializeField, HideInInspector, Self]
        private Transform tr;
        [SerializeField, HideInInspector, Self]
        public HealthComponent healthComponent;
        [SerializeField, HideInInspector, Self]
        public EnemyMovement enemyMovement;
    }
}