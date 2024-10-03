using UnityEngine;
using Utils.StateMachine;

namespace Entities.Enemies
{
    public class BaseEnemyState : IState
    {
        public EnemyMovement Owner;
        public BaseEnemyState(EnemyMovement owner)
        {
            Owner = owner;
        }

    public virtual void OnEnter()
        {
            // NOOP
        }

        public virtual void Update()
        {
            // NOOP
        }

        public virtual void FixedUpdate()
        {
            // NOOP
        }

        public virtual void OnExit()
        {
            // NOOP
        }
    }

    public class GroundedState : BaseEnemyState
    {
        public GroundedState(EnemyMovement owner) : base(owner)
        {
        }
    }
    
    public class AirborneState : BaseEnemyState
    {
        public AirborneState(EnemyMovement owner) : base(owner)
        {
        }
    }

    public class StunnedState : BaseEnemyState
    {
        public StunnedState(EnemyMovement owner) : base(owner)
        {
        }
    }
}