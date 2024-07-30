using Projectiles;
using UnityEngine;

namespace Weapons
{
    [CreateAssetMenu(menuName = "Weapons/WeaponSettings")]
    public class WeaponSettings : ScriptableObject
    {
        public ProjectileSettings projectileSettings;
        [Range(1, 999)] public int magazineSize = 0;
        public float rateOfFire = 0f;
        [Range(0f, 5f)] public float reloadSpeed = 0f;
        public GameObject prefab;
    }
}
