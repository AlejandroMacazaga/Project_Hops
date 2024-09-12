using Player.Classes;
using Player.Events;
using UnityEngine;
using Utils.EventBus;
using Utils.StateMachine;
using Utils.Timers;

namespace Player
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
    public class GroundedState : GenericMovementState, IJumpCancelable
    {
        public override void OnEnter()
        {
            CharClass.mover.currentAmountOfAirJumps = (int) CharClass.GetCurrentStat(ClassStat.MaxAirJumps);
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
    
    public class AirborneState : GenericMovementState, IJumpCancelable
    {

        private readonly CountdownTimer _coyoteTime;
        public override void OnEnter()
        {
            _coyoteTime.Start();
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

        public AirborneState(CharacterClass cc, CountdownTimer coyoteTime) : base(cc)
        {
            _coyoteTime = coyoteTime;
        }
    }

    public class DashingState : GenericMovementState, IJumpCancelable
    {
        private Vector2 _direction;
        public readonly PlayerCooldownTimer Timer;
        public bool IsFinished = true;
        public bool AirborneDash = false;
        public override void OnEnter()
        {
            Timer.Start();
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
            CharClass.mover.currentSpeed = new Vector2(_direction.x, _direction.y) * 1.5f;
            CharClass.GetClassData().RemoveModifier(ClassStat.Gravity, StatModifier.Zero);
            Timer.Stop();
            EventBus<PlayerIsGoingFast>.Raise(new PlayerIsGoingFast()
            {
                IsGoingFast = false
            });
        }

        public DashingState(CharacterClass cc) : base(cc)
        {
            _direction = new Vector2();
            Timer = new PlayerCooldownTimer(CharClass.GetClassData(), ClassStat.DashDuration);
            Timer.OnTimerStart += () => IsFinished = false;
            Timer.OnTimerStop += () => IsFinished = true;
        }
    }
    
    public class DashedAirborneState : GenericMovementState
    {
        public DashedAirborneState(CharacterClass cc) : base(cc)
        {
        }

        public override void OnEnter()
        {
            throw new System.NotImplementedException();
        }

        public override void Update()
        {
            throw new System.NotImplementedException();
        }

        public override void FixedUpdate()
        {
            throw new System.NotImplementedException();
        }

        public override void OnExit()
        {
            throw new System.NotImplementedException();
        }
    }

    public interface IJumpCancelable
    {
        
    }
}
