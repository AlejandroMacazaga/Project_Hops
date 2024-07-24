using Utils.StateMachine;

namespace Player.States
{
    public abstract class PlayerBaseState : IState
    {

        protected readonly PlayerController Controller;

        protected PlayerBaseState(PlayerController controller)
        {
            this.Controller = controller;
        }

        public virtual void FixedUpdate()
        {
            // noop
        }

        public virtual void OnEnter()
        {
            // noop
        }

        public virtual void OnExit()
        {
            // noop
        }

        public virtual void Update()
        {
            // noop
        }
    }
}
