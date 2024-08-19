using Player;
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
        private readonly ProjectileSettings _projectileSettings;
        private readonly PlayerController _owner;
        private readonly Camera _camera;
        private readonly CountdownTimer _reloadTimer;
        private int _currentBullets;
        public SingleShotWeapon(WeaponSettings weaponSettings, PlayerController owner, ProjectileSettings projectileSettings)
        {
            _weaponSettings = weaponSettings;
            _owner = owner;
            _camera = Camera.main;
            _projectileSettings = projectileSettings;
            _reloadTimer = new CountdownTimer(_weaponSettings.reloadSpeed * _owner.PlayerStats.GetStat("ReloadSpeed"));
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
            var flyweight = (Projectile)FlyweightManager.Spawn(_projectileSettings);
            flyweight.transform.position = _camera.transform.localPosition;
            flyweight.transform.rotation = _camera.transform.localRotation;
            _currentBullets -= 1;
            EventBus<AmmoChangeEvent>.Raise(new AmmoChangeEvent(_currentBullets));
        }

        public void SecondaryAttack()
        
        {
            
        }

        public void Reload()
        {
            if (_reloadTimer.IsRunning) return;
            _reloadTimer.InitialTime = _weaponSettings.reloadSpeed * _owner.PlayerStats.GetStat("ReloadSpeed");
            _reloadTimer.Start();
        }
        
    }
}
