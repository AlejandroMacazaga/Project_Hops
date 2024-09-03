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
        }

        public override void Update()
        {
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
            _timer.OnTimerStart += () => IsGracePeriodOver = false;
            _timer.OnTimerStop += () => IsGracePeriodOver = true;
            _timer.Start();
            CharClass.GetClassData().AddModifier(ClassStat.Gravity, StatModifier.Zero);
            CharClass.mover.currentVelocity = CharClass.GetCurrentStat(ClassStat.JumpForce);
        }

        public override void Update()
        {
            CharClass.mover.HandleAirMovement();
        }

        public override void FixedUpdate()
        {
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
            
        }

        public override void Update()
        {
            CharClass.mover.HandleAirMovement();
        }

        public override void FixedUpdate()
        {

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
        private readonly PlayerCooldownTimer _timer;
        public bool IsFinished = true;
        public override void OnEnter()
        {
            _timer.Start();
            CharClass.fpCamera.IsBodyLocked = true;
            _direction =  CharClass.mover.currentDirection;
            CharClass.mover.currentVelocity = 0f;
            CharClass.GetClassData().AddModifier(ClassStat.Gravity, StatModifier.Zero);
            if (_direction is { x: 0, y: 0 }) _direction.y = 1f;
            EventBus<PlayerIsGoingFast>.Raise(new PlayerIsGoingFast()
            {
                IsGoingFast = true
            });
        }

        public override void Update()
        {
            var move = new Vector3(_direction.x, 0 , _direction.y);
            move = CharClass.mover.transform.TransformDirection(move);
            move *= CharClass.GetCurrentStat(ClassStat.Speed);
            move *= CharClass.GetCurrentStat(ClassStat.DashSpeed);
            move.y = CharClass.mover.currentVelocity;
            
            CharClass.mover.characterController.Move(move * Time.deltaTime);
        }

        public override void FixedUpdate()
        {
        }

        public override void OnExit()
        {
            
            CharClass.fpCamera.IsBodyLocked = false;
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
            _timer = new PlayerCooldownTimer(CharClass.GetClassData(), ClassStat.DashDuration);
            _timer.OnTimerStart += () => IsFinished = false;
            _timer.OnTimerStop += () => IsFinished = true;
        }
    }
}