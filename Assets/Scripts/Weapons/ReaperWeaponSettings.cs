using AYellowpaper.SerializedCollections;
using Entities;
using UnityEngine;
using UnityEngine.Serialization;

namespace Weapons
{
    [CreateAssetMenu(menuName = "Weapons/Reaper")]
    public class ReaperWeaponSettings :  ScriptableObject, IWeaponSettings
    {
        [Range(0.01f, 10f)] public float timeForChargedPrimaryAttack;
        public AnimationClip idleAnimation;
        public AnimationClip walkAnimation;
        public SerializedDictionary<ReaperAction, AnimationClip> animations;
        public SerializedDictionary<ReaperAction, DamageComponent> damages;
        public SerializedDictionary<ReaperAction, Vector3> damageRange;
    }

    public enum ReaperAction
    {
        FastPrimaryAttackLeft,
        FastPrimaryAttackRight,
        ChargedPrimaryAttackLeft,
        ChargedPrimaryAttackRight,
        SecondaryAttack,
        ReloadAttack,
    }
}