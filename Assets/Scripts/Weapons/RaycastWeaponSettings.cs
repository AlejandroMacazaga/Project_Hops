using Entities;
using UnityEngine;

namespace Weapons
{
    [CreateAssetMenu(menuName = "Weapons/RaycastWeapon")]
    public class RaycastWeaponSettings : WeaponSettings
    {
        [Range(1f, 999f)] public float range;
        public DamageComponent damage;
    }
}