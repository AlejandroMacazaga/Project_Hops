using Player.States;
using UnityEngine;
using Utils.StateMachine;
using Utils.Timers;

namespace Player.Classes.Reaper
{
    public abstract class ReaperState : IState
    {
        protected readonly ReaperClass Owner;

        protected ReaperState(ReaperClass owner)
        {
            Owner = owner;
        }
        public abstract void OnEnter();
        public abstract void Update();
        public abstract void FixedUpdate();
        public abstract void OnExit();
    }

    public class ReaperNoAttackState : ReaperState
    {
        public ReaperNoAttackState(ReaperClass owner) : base(owner)
        {
        }

        public override void OnEnter()
        {
            Debug.Log("Current State: " + ToString());
        }

        public override void Update()
        {
            // noop
        }

        public override void FixedUpdate()
        {
            // noop
        }

        public override void OnExit()
        {
            // noop
        }
    }
    public class ReaperPrimaryAttackChargingState : ReaperState
    {
        public CountdownTimer CanBeCanceledTimer;
        private IState _currentMovementState;
        public ReaperPrimaryAttackChargingState(ReaperClass owner) : base(owner)
        {
            CanBeCanceledTimer = new CountdownTimer(0.1f);
        }

        public override void OnEnter()
        {
            Owner.isLeftAttack = !Owner.isLeftAttack;
            Owner.isAttacking = true;
            CanBeCanceledTimer.Start();
            Owner.GetClassData().AddModifier(ClassStat.Speed, Owner.ChargeSlowdown);
            _currentMovementState = Owner.mover.MovementStateMachine.CurrentState;
        }

        public override void Update()
        {
        }

        public override void FixedUpdate()
        {
            // noop
        }

        public override void OnExit()
        {
            
            if(!Owner.HoldAttackTimer.IsFinished()) Owner.HoldAttackTimer.Stop();
            if(!CanBeCanceledTimer.IsFinished()) CanBeCanceledTimer.Stop();
            Owner.GetClassData().RemoveModifier(ClassStat.Speed, Owner.ChargeSlowdown);
        }

        public bool CanBeCanceled()
        {
            return CanBeCanceledTimer.IsFinished();
        }
    }

    public class ReaperFastPrimaryAttackState : ReaperState
    {

        private ReaperAttack _attack;
        private readonly CountdownTimer _hitboxDuration;
        public ReaperFastPrimaryAttackState(ReaperClass owner) : base(owner)
        {
            Debug.Log("Current State: " + ToString());
            _hitboxDuration = new CountdownTimer(Owner.attacks[ReaperAction.FastPrimaryAttackLeft].duration);
            _hitboxDuration.OnTimerStop += () =>  {
                _attack.hitbox.Deactivate();
                Owner.isAttacking = !Owner.isAttacking;
            };
            _hitboxDuration.OnTimerStart += () => _attack.hitbox.Activate();
        }

        public override void OnEnter()
        {
            Debug.Log("Current State: " + ToString());
            _attack = Owner.isLeftAttack
                ? Owner.attacks[ReaperAction.FastPrimaryAttackLeft]
                : Owner.attacks[ReaperAction.FastPrimaryAttackRight];
            _hitboxDuration.Start();
            Owner.AnimationSystem.PlayOneShot(_attack.animation);
        }

        public override void Update()
        {
            // noop
        }

        public override void FixedUpdate()
        {
            // noop
        }

        public override void OnExit()
        {
            Owner.isAttacking = !Owner.isAttacking;
        }
    }

    public class ReaperChargedPrimaryAttackState : ReaperState
    {
        private ReaperAttack _attack;
        private readonly CountdownTimer _hitboxDuration;
        public ReaperChargedPrimaryAttackState(ReaperClass owner) : base(owner)
        {
            _hitboxDuration = new CountdownTimer(Owner.attacks[ReaperAction.FastPrimaryAttackLeft].duration);
            _hitboxDuration.OnTimerStop += () =>
            {
                _attack.hitbox.Deactivate();
                Owner.isAttacking = !Owner.isAttacking;
            };
            
            _hitboxDuration.OnTimerStart += () => _attack.hitbox.Activate();
        }

        public override void OnEnter()
        {
            Debug.Log("Current State: " + ToString());
            _attack = Owner.isLeftAttack
                ? Owner.attacks[ReaperAction.ChargedPrimaryAttackLeft]
                : Owner.attacks[ReaperAction.ChargedPrimaryAttackRight];
            _hitboxDuration.Start();
            Owner.AnimationSystem.PlayOneShot(_attack.animation);
        }

        public override void Update()
        {
            // noop
        }

        public override void FixedUpdate()
        {
            // noop
        }

        public override void OnExit()
        {
        }
    }

    public class ReaperSecondaryAttackState : ReaperState
    {
        public ReaperSecondaryAttackState(ReaperClass owner) : base(owner)
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
}