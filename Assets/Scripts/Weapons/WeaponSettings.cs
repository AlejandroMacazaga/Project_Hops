using Projectiles;
using UnityEngine;

namespace Weapons
{
    [CreateAssetMenu(menuName = "Weapons/WeaponSettings")]
    public class WeaponSettings : ScriptableObject
    {
        public ProjectileSettings projectileSettings;
        public int magazineSize = 0;
        public float rateOfFire = 0f;
        public float reloadSpeed = 0f;
        public GameObject prefab;
    }
}
