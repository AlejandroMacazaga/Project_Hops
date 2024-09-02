using System;
using System.Collections.Generic;
using Entities;
using Player;
using UnityEngine;

namespace Weapons
{
    public class RaycastWeapon : IWeapon
    {
        private readonly RaycastWeaponSettings _settings;
        private readonly Camera _spawnPoint;
        private readonly PlayerController _owner;
        private List<DamageComponent> _damageList;
        
        public void PrimaryAttack()
        {
            Debug.Log(FindByRaycast());
        }

        

        public RaycastWeapon(PlayerController owner, RaycastWeaponSettings settings)
        {
            _owner = owner;
            _spawnPoint = Camera.main;
            _settings = settings;
        }

        private IVisitable FindByRaycast()
        {
            Debug.DrawRay(_spawnPoint.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f)), _spawnPoint.transform.forward * _settings.range, Color.red, 10f);
            return Physics.Raycast(
                _spawnPoint.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f)),
                 _spawnPoint.transform.forward,
                out var hit,
                _settings.range) ? hit.transform.GetComponent<IVisitable>() : default;
        }

        public void Action(WeaponAction action)
        {
            switch (action)
            {
                case WeaponAction.StartPrimaryAttack:
                    PrimaryAttack();
                    break;
                case WeaponAction.StartSecondaryAttack:
                    break;
                case WeaponAction.StartReload:
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

        public void DoNothing()
        {
            Debug.Log("Doing Nothing");
        }
    }
}