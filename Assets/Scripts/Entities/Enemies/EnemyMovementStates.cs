using UnityEngine;
using Utils.StateMachine;

namespace Entities.Enemies
{
    public class BaseEnemyState : IState
    {
        
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
        
    }
    
    public class AirborneState : BaseEnemyState
    {
        
    }

    public class StunnedState : BaseEnemyState
    {
        
    }
}