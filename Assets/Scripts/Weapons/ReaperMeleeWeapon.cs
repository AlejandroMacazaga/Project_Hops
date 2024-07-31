using System.Collections.Generic;
using Entities;
using Player;
using UnityEngine;

namespace Weapons
{
    public class ReaperMeleeWeapon : IWeapon
    {
        private readonly RaycastWeaponSettings _settings;
        private readonly Camera _camera;
        private readonly PlayerController _owner;
        private List<DamageComponent> _damageList;
        
        public void PrimaryAttack()
        {
            
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
            _camera = _owner?.fpCamera?.gameObject.GetComponent<Camera>();
            _settings = settings;
        }

        private IVisitable FindByRaycast()
        {
            return Physics.Raycast(
                _camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f)),
                _owner.fpCamera.transform.forward,
                out var hit,
                _settings.range) ? hit.transform.GetComponent<IVisitable>() : default;
        }
    }
}