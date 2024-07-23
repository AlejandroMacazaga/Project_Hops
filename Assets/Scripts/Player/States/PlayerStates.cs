using UnityEngine;
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

    public class PlayerIdleState : PlayerBaseState
    {
        public PlayerIdleState(PlayerController controller) : base(controller)
        {
        }

        public override void FixedUpdate()
        {
        }

        public override void OnEnter()
        {
            
        }

        public override void OnExit()
        {
            
        }

        public override void Update()
        {
            
        }

    }

    public class PlayerJumpState : PlayerBaseState
    {
        private AnimationClip _animation;
        public PlayerJumpState(PlayerController controller, AnimationClip animation) : base(controller)
        {
            this._animation = animation;
        }

        public override void FixedUpdate()
        {
        }

        public override void OnEnter()
        {
            //controller.AnimationSystem.PlayOneShot(animation, true);
        }

        public override void OnExit()
        {
            
        }

        public override void Update()
        {

        }
    }
}
