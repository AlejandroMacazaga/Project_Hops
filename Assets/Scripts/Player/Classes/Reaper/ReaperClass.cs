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
        
        private bool _isPressingDash, _hasStartedPrimaryAttack;
        private CountdownTimer _holdAttackTimer;
        private AttackHoldEvent _holdEvent;

        [SerializeField] private int attackChainNumber = 0;
        
        [SerializeField] private SerializedDictionary<ReaperAction, ReaperAttack> attacks;
        public override void Start()
        {
            base.Start();
            #region StateMachine
            DashingState = new DashingState(this);
            DashCooldown = new PlayerCooldownTimer(data, ClassStat.DashCooldown);
            mover.MovementStateMachine.AddTransition(mover.GroundedState, DashingState, new FuncPredicate(() => _isPressingDash && DashCooldown.IsFinished() && stamina.UseStamina(50)));
            mover.MovementStateMachine.AddTransition(mover.AirborneState, DashingState, new FuncPredicate(() => _isPressingDash && DashCooldown.IsFinished()));

            mover.MovementStateMachine.AddTransition(DashingState, mover.GroundedState,
                new FuncPredicate(() => DashingState.IsFinished && mover.characterController.isGrounded), () => DashCooldown.Start());
            mover.MovementStateMachine.AddTransition(DashingState, mover.AirborneState, new FuncPredicate(() => DashingState.IsFinished && !mover.characterController.isGrounded), () => DashCooldown.Start());
            #endregion
            
            inputReader.Dash += OnDash;
            inputReader.PrimaryAttack += OnPrimaryAttack;
            inputReader.EnablePlayerActions();
            
            _holdAttackTimer = new CountdownTimer(1f);
            _holdAttackTimer.OnTimerTick += OnHoldTick;
        }
        
        private void OnHoldTick()
        {
            _holdEvent.Progress = _holdAttackTimer.Progress;
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
            Debug.Log(action);
            Debug.Log(interaction);
            switch (action)
            {
                case ActionState.Press:
                    StartPrimaryAttack();
                    break;
                case ActionState.Hold:
                    break;
                case ActionState.Release:
                    ReleasePrimaryAttack();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(action), action, null);
            }
        }
        
        private void StartPrimaryAttack()
        {
            if (_hasStartedPrimaryAttack) return;
            _hasStartedPrimaryAttack = true;    
            _holdAttackTimer.Start();
            attackChainNumber++;

        }

        private void ReleasePrimaryAttack()
        {
            if (_holdAttackTimer.IsFinished())
            {
                ChargedPrimaryAttack();
            }
            else
            {
                FastPrimaryAttack();
            }

            _holdAttackTimer.Stop();
            _holdEvent.Progress = 0f;
            EventBus<AttackHoldEvent>.Raise(_holdEvent);
            _hasStartedPrimaryAttack = false;
        }


        private void FastPrimaryAttack()
        {
            var attack = attackChainNumber % 2 == 0
                ? attacks[ReaperAction.FastPrimaryAttackLeft]
                : attacks[ReaperAction.FastPrimaryAttackRight];
            attack.hitbox.Activate();
            // play the animation
            var timer = new CountdownTimer(attack.duration);
            timer.Start();
            timer.OnTimerStop += attack.hitbox.Deactivate;
        }

        private void ChargedPrimaryAttack()
        {
            var attack = attackChainNumber % 2 == 0
                ? attacks[ReaperAction.ChargedPrimaryAttackLeft]
                : attacks[ReaperAction.ChargedPrimaryAttackRight];
            attack.hitbox.Activate();
            // play the animation
            var timer = new CountdownTimer(attack.duration);
            timer.Start();
            timer.OnTimerStop += attack.hitbox.Deactivate;
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