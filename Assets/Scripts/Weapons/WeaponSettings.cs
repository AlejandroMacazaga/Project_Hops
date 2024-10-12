using Projectiles;
using UnityEngine;

namespace Weapons
{
    [CreateAssetMenu(menuName = "Weapons/ProjectileWeapon")]
    public class WeaponSettings : ScriptableObject, IWeaponSettings
    {
        [Range(1, 999)] public int magazineSize = 0;
        public float rateOfFire = 0f;
        [Range(0f, 5f)] public float reloadSpeed = 0f;
        public GameObject prefab;
        public ProjectileSettings primaryAttack;
        public ProjectileSettings secondaryAttack;
        public int damage;
    }

    public interface IWeaponSettings
    {
    }
}
