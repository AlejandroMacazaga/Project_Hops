using Utils.StateMachine;

namespace Entities
{
    public abstract class BaseEntityState : IState
    {
        public readonly EntityController Controller;

        public BaseEntityState(EntityController controller)
        {
            Controller = controller;
        }

        public virtual void FixedUpdate()
        {

        }

        public virtual void OnEnter()
        {

        }

        public virtual void OnExit()
        {

        }

        public virtual void Update()
        {
        }
    }
}

