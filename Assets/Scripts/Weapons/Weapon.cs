using Projectiles;
using UnityEngine;
using Utils.Flyweight;

namespace Weapons
{
    public class Weapon : IWeapon
    {
        private readonly ProjectileSettings _settings;
        private readonly GameObject _owner;

        public Weapon(ProjectileSettings settings, GameObject owner)
        {
            _settings = settings;
            _owner = owner;
        }
        public void Shoot()
        {
            var flyweight = FlyweightManager.Spawn(_settings);
            flyweight.transform.position = _owner.transform.position;
            flyweight.transform.rotation = _owner.transform.rotation;
        }
    }
}
