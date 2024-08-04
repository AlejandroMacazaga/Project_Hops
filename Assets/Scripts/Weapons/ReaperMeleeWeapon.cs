using System.Collections.Generic;
using Entities;
using Player;
using UnityEngine;

namespace Weapons
{
    public class ReaperMeleeWeapon : IWeapon
    {
        private readonly RaycastWeaponSettings _settings;
        private readonly Camera _spawnPoint;
        private readonly PlayerController _owner;
        private List<DamageComponent> _damageList;
        
        public void PrimaryAttack()
        {
            Debug.Log(FindByRaycast());
        }

        public void SecondaryAttack()
        {
            throw new System.NotImplementedException();
        }

        public void Reload()
        {
            throw new System.NotImplementedException();
        }

        public ReaperMeleeWeapon(PlayerController owner, RaycastWeaponSettings settings)
        {
            _owner = owner;
            _spawnPoint = Camera.main;
            Debug.Log(_spawnPoint);
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
    }
}