using Utils.StateMachine;

namespace Entities.Enemies
{
    public class BaseEnemyState : IState
    {
        
        public void OnEnter()
        {
            // NOOP
        }

        public void Update()
        {
            // NOOP
        }

        public void FixedUpdate()
        {
            // NOOP
        }

        public void OnExit()
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