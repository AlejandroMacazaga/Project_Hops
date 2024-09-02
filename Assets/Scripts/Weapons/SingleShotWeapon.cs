using System;
using Player;
using Player.Classes;
using Projectiles;
using UnityEngine;
using Utils.EventBus;
using Utils.Flyweight;
using Utils.Timers;

namespace Weapons
{
    public class SingleShotWeapon : IWeapon
    {
        private readonly WeaponSettings _weaponSettings;
        private readonly PlayerController _owner;
        private readonly Camera _camera;
        private readonly CountdownTimer _reloadTimer;
        private int _currentBullets;

        public SingleShotWeapon(WeaponSettings weaponSettings, PlayerController owner)
        {
            _weaponSettings = weaponSettings;
            _owner = owner;
            _camera = Camera.main;
            _reloadTimer = new CountdownTimer(_weaponSettings.reloadSpeed * _owner.classData.GetStat(ClassStat.ReloadSpeed));
            _currentBullets = weaponSettings.magazineSize;
            EventBus<AmmoChangeEvent>.Raise(new AmmoChangeEvent(_currentBullets));
            _reloadTimer.OnTimerStop += () =>
            {
                _currentBullets = weaponSettings.magazineSize;
                EventBus<AmmoChangeEvent>.Raise(new AmmoChangeEvent(_currentBullets));
            };
            
        }

        public void PrimaryAttack()
        {
            if (!_reloadTimer.IsFinished()) return;
            if (_currentBullets == 0)
            {
                Reload();
                return;
            }
            var flyweight = (Projectile)FlyweightManager.Spawn(_weaponSettings.primaryAttack);
            flyweight.currentDamage = _weaponSettings.damage * _owner.classData.GetStat(ClassStat.AttackDamage);
            flyweight.transform.position = _camera.transform.localPosition;
            flyweight.transform.rotation = _camera.transform.localRotation;
            _currentBullets -= 1;
            EventBus<AmmoChangeEvent>.Raise(new AmmoChangeEvent(_currentBullets));
        }
        

        public void SecondaryAttack()
        {
            if (!_reloadTimer.IsFinished()) return;
            if (_currentBullets == 0)
            {
                Reload();
                return;
            }
            var flyweight = (Projectile)FlyweightManager.Spawn(_weaponSettings.secondaryAttack);
            flyweight.currentDamage = _weaponSettings.damage * (_currentBullets / 2) * _owner.classData.GetStat(ClassStat.AttackDamage);
            flyweight.transform.position = _camera.transform.localPosition;
            flyweight.transform.rotation = _camera.transform.localRotation;
            _currentBullets = 0;
            EventBus<AmmoChangeEvent>.Raise(new AmmoChangeEvent(_currentBullets));
        }

        public void Reload()
        {
            if (_reloadTimer.IsRunning) return;
            _reloadTimer.InitialTime = _weaponSettings.reloadSpeed * _owner.classData.GetStat(ClassStat.ReloadSpeed);
            _reloadTimer.Start();
        }
        public void Action(WeaponAction action)
        {
            switch (action)
            {
                case WeaponAction.StartPrimaryAttack:
                    Debug.Log("Pog");
                    PrimaryAttack();
                    break;
                case WeaponAction.StartSecondaryAttack:
                    SecondaryAttack();
                    break;
                case WeaponAction.StartReload:
                    Reload();
                    break;
                case WeaponAction.HoldPrimaryAttack:
                    break;
                case WeaponAction.HoldSecondaryAttack:
                    break;
                case WeaponAction.HoldReload:
                    break;
                case WeaponAction.ReleasePrimaryAttack:
                    break;
                case WeaponAction.ReleaseSecondaryAttack:
                    break;
                case WeaponAction.ReleaseReload:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(action), action, null);
            }
        } 
    }
}
