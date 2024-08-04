using Player;
using Projectiles;
using UnityEngine;
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
            _camera = Camera.current;
            _projectileSettings = projectileSettings;
            _reloadTimer = new CountdownTimer(_weaponSettings.reloadSpeed * _owner.PlayerStats.GetStat("ReloadSpeed"));
            _currentBullets = weaponSettings.magazineSize;
            _reloadTimer.OnTimerStop += () => _currentBullets = weaponSettings.magazineSize;
        }

        public void PrimaryAttack()
        {
            if (!_reloadTimer.IsFinished()) return;
            if (_currentBullets == 0) Reload();
            var flyweight = (Projectile)FlyweightManager.Spawn(_projectileSettings);
            flyweight.transform.position = _camera.transform.localPosition;
            flyweight.transform.rotation = _camera.transform.localRotation;
            _currentBullets -= 1;
        }

        public void SecondaryAttack()
        {
            throw new System.NotImplementedException();
        }

        public void Reload()
        {
            if (_reloadTimer.IsRunning) return;
            _reloadTimer.InitialTime = _weaponSettings.reloadSpeed * _owner.PlayerStats.GetStat("ReloadSpeed");
            _reloadTimer.Start();
        }
        
    }
}
