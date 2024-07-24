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
            Debug.Log("We Jumping");
            
            Controller.PlayerStats.AddModifier("Gravity", _gravityModifier);
            _timer.Start();
            Controller.SetVelocity(Controller.PlayerStats.GetStat("JumpForce"));

        }

        public override void OnExit()
        {
            Debug.Log("We no jumping no more");
            Controller.PlayerStats.RemoveModifier("Gravity", _gravityModifier);
            _timer.Stop();
        }

        public override void Update()
        {

        }
    }
}