using Player;
using UnityEngine;

namespace Weapons
{
    public class ReaperMeleeWeapon : IWeapon
    {
        
        private readonly WeaponSettings _weaponSettings;
        private readonly PlayerController _owner;
        private readonly Camera _camera;
        public void PrimaryAttack()
        {
            throw new System.NotImplementedException();
        }

        public void SecondaryAttack()
        {
            throw new System.NotImplementedException();
        }

        public void Reload()
        {
            throw new System.NotImplementedException();
        }
    }
}