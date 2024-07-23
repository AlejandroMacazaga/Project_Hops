using UnityEngine;
using Utils.StateMachine;
using Utils.Timers;

namespace Player.States
{
    public interface IControllable {}
    public interface IAffectedByGravity {}
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
            Debug.Log("We on the idle state");
        }

        public override void OnExit()
        {
            Debug.Log("We off the idle state");
        }

        public override void Update()
        {
            
        }

    }

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

    public class PlayerOnAirState : PlayerBaseState
    {
        private readonly StatModifier _gravityModifier;
        private const float GravityMultiplier = 3f;
        private AnimationClip _animation;
        public PlayerOnAirState(PlayerController controller, AnimationClip animation) : base(controller)
        {
            this._animation = animation;
            _gravityModifier = new StatModifier(StatModifier.ModifierType.Percent, GravityMultiplier);
        }

        public override void FixedUpdate()
        {
        }

        public override void OnEnter()
        {
            Debug.Log("We on the air");
            Controller.PlayerStats.AddModifier("Gravity", _gravityModifier);
            //controller.AnimationSystem.PlayOneShot(animation, true);
        }

        public override void OnExit()
        {
            Debug.Log("We off the air");
            Controller.PlayerStats.RemoveModifier("Gravity", _gravityModifier);
        }

        public override void Update()
        {

        }
    }
}
