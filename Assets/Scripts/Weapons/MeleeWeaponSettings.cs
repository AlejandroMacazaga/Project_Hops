using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.Serialization;

namespace Weapons
{
    [CreateAssetMenu(menuName = "Weapons/MeleeWeapon")]
    public class MeleeWeaponSettings : ScriptableObject, IWeaponSettings
    {
        public GameObject prefab;
        public SerializedDictionary<WeaponAction, AnimationClip> animations;
        
    }
}