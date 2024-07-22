
using UnityEngine;
using Utils.StateMachine;

namespace Scripts.Player
{
    public abstract class PlayerBaseState : IState
    {

        readonly PlayerController controller;

        protected PlayerBaseState(PlayerController controller)
        {
            this.controller = controller;
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
        AnimationClip animation;
        public PlayerJumpState(PlayerController controller, AnimationClip animation) : base(controller)
        {
            this.animation = animation;
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
}
