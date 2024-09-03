/* using System;
using MEC;
using Player;
using UnityEngine;
using Utils.EventBus;
using Utils.Timers;

namespace Weapons
{
    public class ReaperWeapon : IWeapon
    {

        private PlayerController _owner;
        private ReaperWeaponSettings _settings;
        private bool _hasStartedPrimaryAttack = false;
        private readonly CountdownTimer _holdTimer;
        private AttackHoldEvent _holdEvent;
        
        public ReaperWeapon(PlayerController owner, ReaperWeaponSettings settings)
        {
            _owner = owner;
            _settings = settings;
            _holdTimer = new CountdownTimer(settings.timeForChargedPrimaryAttack);
            _holdTimer.OnTimerTick += OnHoldTick;
        }

        private void OnHoldTick()
        {
            _holdEvent.Progress = _holdTimer.Progress;
            EventBus<AttackHoldEvent>.Raise(_holdEvent);
        }
        
        public void Action(WeaponAction action)
        {
            switch (action)
            {
                case WeaponAction.StartPrimaryAttack:
                    StartPrimaryAttack();
                    break;
                case WeaponAction.StartSecondaryAttack:
                    break;
                case WeaponAction.StartReload:
                    break;
                case WeaponAction.HoldPrimaryAttack:
                    HoldPrimaryAttack();
                    break;
                case WeaponAction.HoldSecondaryAttack:
                    break;
                case WeaponAction.HoldReload:
                    break;
                case WeaponAction.ReleasePrimaryAttack:
                    ReleasePrimaryAttack();
                    break;
                case WeaponAction.ReleaseSecondaryAttack:
                    break;
                case WeaponAction.ReleaseReload:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(action), action, null);
            }
        }

        private void StartPrimaryAttack()
        {
            if (_hasStartedPrimaryAttack) return;
            _hasStartedPrimaryAttack = true;    
            _holdTimer.Start();

        }

        private void ReleasePrimaryAttack()
        {
            if (_holdTimer.IsFinished())
            {
                
            }
            else
            {
                
            }

            _holdTimer.Stop();
            _holdEvent.Progress = 0f;
            EventBus<AttackHoldEvent>.Raise(_holdEvent);
            _hasStartedPrimaryAttack = false;
        }

        private void ChargedPrimaryAttack()
        {
            
        }

        private void QuickPrimaryAttack()
        {
            
        }

        private void HoldPrimaryAttack()
        {

        }
    }
} */