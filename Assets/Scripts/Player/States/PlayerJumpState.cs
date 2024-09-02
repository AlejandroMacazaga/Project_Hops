using Player.Classes;
using UnityEngine;
using Utils.Timers;

namespace Player.States
{
    public class PlayerJumpState : PlayerBaseState
    {
        private AnimationClip _animation;
        private readonly StatModifier _gravityModifier;
        private readonly CountdownTimer _timer = new(0.1f);
        public bool IsGracePeriodOver = false;
        public PlayerJumpState(PlayerController controller, AnimationClip animation, StatModifier gravityModifier ) : base(controller)
        {
            _animation = animation;
            _gravityModifier = gravityModifier;
            _timer.OnTimerStart += () => IsGracePeriodOver = false;
            _timer.OnTimerStop += () => IsGracePeriodOver = true;
        }

        public override void FixedUpdate()
        {
        }

        public override void OnEnter()
        {
            Debug.Log("Jump State");
            Controller.classData.AddModifier(ClassStat.Gravity, _gravityModifier);
            _timer.Start();
            Controller.SetVelocity(Controller.classData.GetStat(ClassStat.JumpForce));

        }

        public override void OnExit()
        {
            Controller.classData.RemoveModifier(ClassStat.Gravity, _gravityModifier);
            Controller.IsPressingJump = false;
            _timer.Stop();
        }

        public override void Update()
        {
            Controller.HandleGravity();
            Controller.HandleAirMovement();
        }
    }
}