using KBCore.Refs;
using UnityEngine;
using UnityEngine.Serialization;

namespace Entities.Enemies
{
    [RequireComponent(typeof(HealthComponent))]
    [RequireComponent(typeof(PushComponent))]
    [RequireComponent(typeof(StunComponent))]
    [RequireComponent(typeof(EnemyMovement))]
    public class ChaseEnemy : ValidatedMonoBehaviour, IEnemy, IStunned, IPushed
    {
        [SerializeField, HideInInspector, Self]
        private Transform tr;
        [SerializeField, HideInInspector, Self]
        public HealthComponent healthComponent;
        [SerializeField, HideInInspector, Self]
        public EnemyMovement enemyMovement;


        public void Stun(float time)
        {
            enemyMovement.Stun(time);
        }

        public bool CanBeStunned()
        {
            return true;
        }

        public void Push(Vector3 force)
        {
            Debug.Log("Lets push");
            enemyMovement.Push(force);
        }

        public bool CanBePushed()
        {
            return true;
        }
    }
}