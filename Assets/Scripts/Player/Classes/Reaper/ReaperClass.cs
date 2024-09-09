using System;
using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Entities.Attacks;
using Input;
using KBCore.Refs;
using Player.States;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Utils.EventBus;
using Utils.StateMachine;
using Utils.Timers;
using Weapons;

namespace Player.Classes.Reaper
{
    public class ReaperClass : CharacterClass
    {
        public DashingState DashingState;
        public PlayerCooldownTimer DashCooldown;

        public ReaperNoAttackState NoAttackState;
        public ReaperPrimaryAttackChargingState ChargingState;
        public ReaperFastPrimaryAttackState FastPrimaryAttackState;
        public ReaperChargedPrimaryAttackState ChargedPrimaryAttackState;
        
        public readonly StateMachine CombatStateMachine = new();
        
        private bool _isPressingDash, _hasStartedPrimaryAttack;
        public bool isAttacking = false;
        public bool isPressingPrimaryAttack = false;
        public bool isLeftAttack = false;
        public CountdownTimer HoldAttackTimer;
        private AttackHoldEvent _holdEvent;

        public readonly StatModifier ChargeSlowdown = new StatModifier(ModifierType.Multiplier, 0.5f);
        
        [SerializeField] public SerializedDictionary<ReaperAction, ReaperAttack> attacks;

        private Action _primaryAttackStart;
        public override void Start()
        {
            base.Start();
            #region MovementStateMachine
            DashingState = new(this);
            DashCooldown = new PlayerCooldownTimer(data, ClassStat.DashCooldown);
            mover.MovementStateMachine.AddTransition(mover.GroundedState, DashingState, new FuncPredicate(() => _isPressingDash && DashCooldown.IsFinished() && stamina.UseStamina(50)));
            mover.MovementStateMachine.AddTransition(mover.AirborneState, DashingState, new FuncPredicate(() => _isPressingDash && DashCooldown.IsFinished()));

            mover.MovementStateMachine.AddTransition(DashingState, mover.GroundedState,
                new FuncPredicate(() => DashingState.IsFinished && mover.characterController.isGrounded), () => DashCooldown.Start());
            mover.MovementStateMachine.AddTransition(DashingState, mover.AirborneState, new FuncPredicate(() => DashingState.IsFinished && !mover.characterController.isGrounded), () => DashCooldown.Start());
            #endregion
            
            #region ReaperStateMachine
            
            NoAttackState = new ReaperNoAttackState(this);
            ChargingState = new ReaperPrimaryAttackChargingState(this);
            FastPrimaryAttackState = new ReaperFastPrimaryAttackState(this);
            ChargedPrimaryAttackState = new ReaperChargedPrimaryAttackState(this);
            
            CombatStateMachine.AddAnyTransition(NoAttackState, new FuncPredicate(() => !isAttacking));
            CombatStateMachine.AddTransition(NoAttackState, ChargingState, new ActionPredicate(ref _primaryAttackStart));
            CombatStateMachine.AddTransition(ChargingState, FastPrimaryAttackState,
                new FuncPredicate(() => !isPressingPrimaryAttack && HoldAttackTimer.IsRunning && ChargingState.CanBeCanceled()));
            CombatStateMachine.AddTransition(ChargingState, ChargedPrimaryAttackState, 
                new FuncPredicate(() => !isPressingPrimaryAttack && !HoldAttackTimer.IsRunning && ChargingState.CanBeCanceled()));
            CombatStateMachine.SetState(NoAttackState);
            #endregion

            inputReader.PrimaryAttack += OnPrimaryAttack;
            inputReader.EnablePlayerActions();
            
            HoldAttackTimer = new CountdownTimer(1f);
            HoldAttackTimer.OnTimerTick += OnHoldTick;
            
            
        }
        

        private void OnHoldTick()
        {
            Debug.Log("Tick");
            _holdEvent.Progress = HoldAttackTimer.Progress;
            EventBus<AttackHoldEvent>.Raise(_holdEvent);
        }

        
        private void OnDash(ActionState action, IInputInteraction interaction)
        {
            switch (action)
            {
                case ActionState.Press:
                    _isPressingDash = true;
                    break;
                case ActionState.Hold:
                    break;
                case ActionState.Release:
                    _isPressingDash = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(action), action, null);
            }
        }

        private void OnPrimaryAttack(ActionState action, IInputInteraction interaction)
        {
            switch (action)
            {
                case ActionState.Press:
                    isPressingPrimaryAttack = true;
                    _primaryAttackStart?.Invoke();
                    HoldAttackTimer.Start();
                    break;
                case ActionState.Hold:
                    break;
                case ActionState.Release:
                    isPressingPrimaryAttack = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(action), action, null);
            }
        }

        void Update()
        {
            CombatStateMachine.Update();
        }
        
        void FixedUpdate()
        {
            CombatStateMachine.FixedUpdate();
        }
    }
    
    
    public enum ReaperAction
    {
        FastPrimaryAttackLeft,
        FastPrimaryAttackRight,
        ChargedPrimaryAttackLeft,
        ChargedPrimaryAttackRight,
        SecondaryAttack,
        ReloadAttack,
    }

    [System.Serializable]
    public struct ReaperAttack
    {
        public AnimationClip animation;
        public Hitbox hitbox;
        public float duration;
    }
}