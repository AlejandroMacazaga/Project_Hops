using System;
using System.Collections;
using System.Collections.Generic;
using Input;
using KBCore.Refs;
using Player.States;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils.StateMachine;
using Utils.Timers;

namespace Player.Classes.Reaper
{
    public class ReaperClass : CharacterClass
    {
        public DashingState DashingState;
        public PlayerCooldownTimer DashCooldown;
        
        private bool _isPressingDash;
        
        public override void Start()
        {
            base.Start();
            DashingState = new DashingState(this);
            DashCooldown = new PlayerCooldownTimer(data, ClassStat.DashCooldown);
            mover.MovementStateMachine.AddTransition(mover.GroundedState, DashingState, new FuncPredicate(() => _isPressingDash && DashCooldown.IsFinished() && stamina.UseStamina(50)));
            mover.MovementStateMachine.AddTransition(mover.AirborneState, DashingState, new FuncPredicate(() => _isPressingDash && DashCooldown.IsFinished()));

            mover.MovementStateMachine.AddTransition(DashingState, mover.GroundedState,
                new FuncPredicate(() => DashingState.IsFinished && mover.characterController.isGrounded), () => DashCooldown.Start());
            mover.MovementStateMachine.AddTransition(DashingState, mover.AirborneState, new FuncPredicate(() => DashingState.IsFinished && !mover.characterController.isGrounded), () => DashCooldown.Start());
            
            inputReader.Dash += OnDash;
            
            inputReader.EnablePlayerActions();
        }

        void Update()
        {
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
    }
}