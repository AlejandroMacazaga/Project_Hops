using KBCore.Refs;
using UnityEngine;
using UnityEngine.Serialization;

namespace Entities.Enemies
{
    [RequireComponent(typeof(HealthComponent))]
    [RequireComponent(typeof(PushComponent))]
    [RequireComponent(typeof(StunComponent))]
    [RequireComponent(typeof(EnemyMovement))]
    public class ChaseEnemy : ValidatedMonoBehaviour, IEnemy, IStunned, IPushed, IEntity
    {
        [SerializeField, HideInInspector, Self]
        private Transform tr;
        [SerializeField, HideInInspector, Self]
        public HealthComponent healthComponent;
        [SerializeField, HideInInspector, Self]
        public EnemyMovement enemyMovement;

        public EntityTeam team;

        public void Stun(float time)
        {
            enemyMovement.Stun(time);
        }

        public bool CanBeStunned()
        {
            return true;
        }

        public bool IsStunned()
        {
            return enemyMovement.MovementStateMachine.CurrentState is StunnedState;
        }

        public void Push(Vector3 force)
        {
            enemyMovement.Push(force);
        }

        public bool CanBePushed()
        {
            return true;
        }

        public float GetStat(EnemyStat stat)
        {
            return 1f;
        }

        public EntityTeam GetTeam()
        {
            return team;
        }
    }
}