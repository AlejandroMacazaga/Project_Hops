using UnityEngine;
using Utils.Timers;

namespace Player.States
{
    public class PlayerDashState: PlayerBaseState
    {
        private AnimationClip _animation;
        private readonly StatModifier _gravityModifier;
        private Vector2 _direction;
        private readonly CountdownTimer _timer;
        public bool IsFinished = true;
        
        public PlayerDashState(PlayerController controller, AnimationClip animation, StatModifier gravityModifier) : base(controller)
        {
            this._animation = animation;
            _direction = new Vector2();
            _gravityModifier = gravityModifier;
            _timer = new CountdownTimer(Controller.PlayerStats.GetStat(PlayerStat.DashDuration));
            _timer.OnTimerStart += () => IsFinished = false;
            _timer.OnTimerStop += () => IsFinished = true;
        }

        public override void FixedUpdate()
        {
        }

        public override void OnEnter()
        {
            _timer.Start();
            Controller.SetVelocity(0f);
            Controller.PlayerStats.AddModifier(PlayerStat.Gravity, _gravityModifier);
            _direction = Controller.currentDirection;
            if (_direction is { x: 0, y: 0 }) _direction.y = 1f;
            Controller.fpScript.IsBodyLocked = true;
            //controller.AnimationSystem.PlayOneShot(animation, true);
        }

        public override void OnExit()
        {
            Controller.PlayerStats.RemoveModifier(PlayerStat.Gravity, _gravityModifier);
            _timer.Stop();
            if(Controller.Character.isGrounded) Controller.DashCooldown.Start();
            else Controller.DashCooldown.Start(); // TODO: Add here logic for when you dash in the air
            Controller.fpScript.IsBodyLocked = false;
            Controller.IsPressingDash = false;
        }

        public override void Update()
        {
            var move = new Vector3(_direction.x, 0 , _direction.y);
            move = Controller.transform.TransformDirection(move);
            move *= Controller.PlayerStats.GetStat(PlayerStat.Speed);
            move *= Controller.PlayerStats.GetStat(PlayerStat.DashMultiplier);
            Controller.Character.Move(move * Time.deltaTime);
        }
    }
} 
