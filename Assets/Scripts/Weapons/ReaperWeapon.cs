using System;
using Player;
using UnityEngine;

namespace Weapons
{
    public class ReaperWeapon : IWeapon
    {

        private CharacterController _owner;
        private MeleeWeaponSettings _settings;
        
        public void Action(WeaponAction action)
        {
            switch (action)
            {
                case WeaponAction.TapPrimaryAttack:
                    break;
                case WeaponAction.TapSecondaryAttack:
                    break;
                case WeaponAction.TapReload:
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