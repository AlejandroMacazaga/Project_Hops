using UnityEngine;

namespace Player.States
{
    public class PlayerOnAirState : PlayerBaseState
    {
        private AnimationClip _animation;
        public PlayerOnAirState(PlayerController controller, AnimationClip animation) : base(controller)
        {
            this._animation = animation;
        }

        public override void FixedUpdate()
        {
        }

        public override void OnEnter()
        {
            //Controller.PlayerStats.AddModifier("Gravity", _gravityModifier);
            //controller.AnimationSystem.PlayOneShot(animation, true);
        }

        public override void OnExit()
        {
            //Controller.PlayerStats.RemoveModifier("Gravity", _gravityModifier);
        }

        public override void Update()
        {
            Controller.HandleGravity();
            Controller.HandleMovement();
        }
    }
}