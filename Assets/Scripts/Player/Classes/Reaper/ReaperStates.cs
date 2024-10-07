using Player.Events;
using UnityEngine;
using UnityUtils;
using Utils.EventBus;
using Utils.StateMachine;
using Utils.Timers;

namespace Player.Classes.Reaper
{
    public abstract class ReaperState : IState
    {
        protected readonly ReaperClass Owner;
        public ReaperAttack AttackData;
        protected abstract void ChangeData(ReaperAttack data);
        protected ReaperState(ReaperClass owner, ReaperAttack attack = default)
        {
            Owner = owner;
            AttackData = attack;
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

        protected override void ChangeData(ReaperAttack data)
        {
            // noop
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
        public readonly CountdownTimer CanBeCanceledTimer;
        public ReaperPrimaryAttackChargingState(ReaperClass owner) : base(owner)
        {
            CanBeCanceledTimer = new CountdownTimer(0.1f);
        }

        protected override void ChangeData(ReaperAttack data)
        {
            // noop
        }

        public override void OnEnter()
        {
            Owner.HoldAttackTimer.Start();
            Owner.isLeftAttack = !Owner.isLeftAttack;
            Owner.isAttacking = true;
            CanBeCanceledTimer.Start();
            Owner.GetClassData().AddModifier(ClassStat.Speed, Owner.ChargeSlowdown);
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
        
        private readonly CountdownTimer _hitboxDuration;
        public ReaperFastPrimaryAttackState(ReaperClass owner, ReaperAttack attack) : base(owner)
        {
            AttackData = attack;
            AttackData.hitbox.ActiveChange += OnActiveChange;
            attack.damage.SetOwnerTransform(owner.transform);
        }
        
        private void OnActiveChange(bool enabled)
        {
            Owner.isAttacking = enabled;
        }

        protected override void ChangeData(ReaperAttack data)
        {
            AttackData = data;
        }

        public override void OnEnter()
        {
            Owner.AnimationSystem.PlayOneShot(Owner.isLeftAttack ? AttackData.animations[0] : AttackData.animations[1]);
            AttackData.hitbox.damage = AttackData.damage;
            AttackData.hitbox.ActivateHitbox(AttackData.duration);
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
        public ReaperChargedPrimaryAttackState(ReaperClass owner, ReaperAttack attack) : base(owner, attack)
        {
            AttackData = attack;
            AttackData.hitbox.ActiveChange += OnActiveChange;
            attack.damage.SetOwnerTransform(owner.transform);
        }

        protected override void ChangeData(ReaperAttack data) => AttackData = data;

        public override void OnEnter()
        {
            AttackData.hitbox.damage = AttackData.damage;
            Owner.AnimationSystem.PlayOneShot(Owner.isLeftAttack ? AttackData.animations[0] : AttackData.animations[1]);
            AttackData.hitbox.ActivateHitbox(AttackData.duration);
        }

        private void OnActiveChange(bool enabled)
        {
            Owner.isAttacking = enabled;
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
        private ReaperAttack _attack;
        private readonly CountdownTimer _canAct;
        public ReaperSecondaryAttackState(ReaperClass owner) : base(owner)
        {
            _canAct = new CountdownTimer(Owner.attacks[ReaperAction.SecondaryAttack].recovery);
            _canAct.OnTimerStop += () =>
            {
                Owner.isAttacking = false;
            };
            
        }

        protected override void ChangeData(ReaperAttack data)
        {
            _attack = data;
        }

        public override void OnEnter()
        {
            var direction = -Owner.fpCamera.virtualCamera.transform.forward;
            Owner.mover.ApplyForce(new Vector3(0f, direction.y,  direction.x), 20f);
            Owner.fpCamera.KickScreen(5f);
            Owner.isAttacking = true;
            _canAct.Start();
            Owner.HandleSecondaryAttack();
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

    public class ReaperReloadAttackState : ReaperState
    {
        private readonly CountdownTimer _startupTimer, _activeTimer;
        public ReaperReloadAttackState(ReaperClass owner, ReaperAttack data) : base(owner, data)
        {
            _startupTimer = new CountdownTimer(data.startup);
            _activeTimer = new CountdownTimer(data.duration);
            AttackData.damage.SetOwnerTransform(owner.transform);   
            _startupTimer.OnTimerStop += () =>
            {
                _activeTimer?.Start();
                AttackData.hitbox.ActivateHitbox(AttackData.duration);
            };
            _activeTimer.OnTimerStop += () => Owner.isAttacking = false;
        }

        protected override void ChangeData(ReaperAttack data)
        {
            
        }

        public override void OnEnter()
        {
            _startupTimer.Start();
            Owner.isAttacking = true;
            Owner.AnimationSystem.PlayOneShot(AttackData.animations[0]);
            AttackData.hitbox.damage = AttackData.damage;
        }

        public override void Update()
        {
            
        }

        public override void FixedUpdate()
        {
            
        }

        public override void OnExit()
        {
            _startupTimer.Stop();
            _activeTimer.Stop();
        }
    }
}