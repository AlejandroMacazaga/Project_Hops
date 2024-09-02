using System;
using Player.Classes;
using Player.Events;
using UnityEngine;
using Utils.EventBus;
using Utils.StateMachine;
using Utils.Timers;

namespace Player.States
{

    public abstract class GenericMovementState : IState
    {
        protected readonly CharacterClass CharClass;

        protected GenericMovementState(CharacterClass cc)
        {
            CharClass = cc;
        }
        public abstract void OnEnter();
        public abstract void Update();
        public abstract void FixedUpdate();
        public abstract void OnExit();
    }
    public class GroundedState : GenericMovementState
    {
        public override void OnEnter()
        {
            Debug.Log("Entering Grounded State");
        }

        public override void Update()
        {
            CharClass.mover.HandleGravity();
            CharClass.mover.HandleMovement();
        }

        public override void FixedUpdate()
        {
            // noop
        }

        public override void OnExit()
        {
            // noop
        }

        public GroundedState(CharacterClass cc) : base(cc)
        {
            
        }
    }
    
    public class JumpingState : GenericMovementState
    {
        public bool IsGracePeriodOver = false;
        private readonly CountdownTimer _timer = new(0.1f);

        public JumpingState(CharacterClass cc) : base(cc)
        {
        }
        public override void OnEnter()
        {
            Debug.Log("Entering Jumping State");
            _timer.OnTimerStart += () => IsGracePeriodOver = false;
            _timer.OnTimerStop += () => IsGracePeriodOver = true;
            _timer.Start();
            CharClass.GetClassData().AddModifier(ClassStat.Gravity, StatModifier.Zero);
            CharClass.mover.currentVelocity = CharClass.GetCurrentStat(ClassStat.JumpForce);
        }

        public override void Update()
        {
            
        }

        public override void FixedUpdate()
        {
            CharClass.mover.HandleGravity();
            CharClass.mover.HandleAirMovement();
        }


        public override void OnExit()
        {
            CharClass.GetClassData().RemoveModifier(ClassStat.Gravity, StatModifier.Zero);
            _timer.Stop();
        }
    }
    
    public class AirborneState : GenericMovementState {
        public override void OnEnter()
        {
            Debug.Log("Entering OnAir State");
        }

        public override void Update()
        {
           
        }

        public override void FixedUpdate()
        {
            CharClass.mover.HandleGravity();
            CharClass.mover.HandleAirMovement();
        }

        public override void OnExit()
        {
            // noop
        }

        public AirborneState(CharacterClass cc) : base(cc)
        {
            
        }
    }

    public class DashingState : GenericMovementState
    {
        private Vector2 _direction;
        private readonly CountdownTimer _timer;
        public bool IsFinished = true;
        public override void OnEnter()
        {
            _timer.Start();
            
            CharClass.GetClassData().AddModifier(ClassStat.Gravity, StatModifier.Zero);
            _direction = CharClass.mover.currentDirection;
            if (_direction is { x: 0, y: 0 }) _direction.y = 1f;
            EventBus<PlayerIsGoingFast>.Raise(new PlayerIsGoingFast()
            {
                IsGoingFast = true
            });
            CharClass.mover.isPressingDash = false;
        }

        public override void Update()
        {
           
        }

        public override void FixedUpdate()
        {
            CharClass.mover.HandleGravity();
            CharClass.mover.HandleAirMovement();
        }

        public override void OnExit()
        {
            CharClass.mover.currentSpeed = _direction;
            CharClass.GetClassData().RemoveModifier(ClassStat.Gravity, StatModifier.Zero);
            _timer.Stop();
            EventBus<PlayerIsGoingFast>.Raise(new PlayerIsGoingFast()
            {
                IsGoingFast = false
            });
        }

        public DashingState(CharacterClass cc) : base(cc)
        {
            _direction = new Vector2();
            _timer = new CountdownTimer(CharClass.GetCurrentStat(ClassStat.DashDuration));
            _timer.OnTimerStart += () => IsFinished = false;
            _timer.OnTimerStop += () => IsFinished = true;
        }
    }
}