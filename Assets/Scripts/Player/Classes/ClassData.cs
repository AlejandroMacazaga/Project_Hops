using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.Serialization;
using Utils.Timers;

namespace Player.Classes
{
    [CreateAssetMenu(menuName = "Classes/ClassData")]
    public class ClassData : ScriptableObject
    {
        public SerializedDictionary<ClassStat, float> baseStats = new();
        public SerializedDictionary<ClassStat, List<StatModifier>> modifiers;
        public SerializedDictionary<ClassStat, ModifiedStat> cachedModifiedValues;
        
        private void OnValidate()
        {
            cachedModifiedValues = new SerializedDictionary<ClassStat, ModifiedStat>();
            modifiers = new SerializedDictionary<ClassStat, List<StatModifier>>();
            var values = Enum.GetValues(typeof(ClassStat));
            foreach(ClassStat stat in values)
            {
                Debug.Log(stat);
                cachedModifiedValues.Add(stat, new ModifiedStat(baseStats[stat])
                {
                    needsUpdate = false
                });
                modifiers.Add(stat, new List<StatModifier>());
            }
            
        }
        private void UpdateStat(ClassStat stat)
        {
            var baseValue = baseStats[stat];
            var modifiedValue = baseValue;
            foreach (var modifier in modifiers[stat])
            {
                var flatSum = 0f;
                var percentSum = 1f;
                var multiplier = new List<float>();
                switch (modifier.Type)
                {
                    case ModifierType.Flat:
                        flatSum += modifier.Value;
                        break;
                    case ModifierType.Percent:
                        percentSum += modifier.Value;
                        break;
                    case ModifierType.Multiplier:
                        multiplier.Add(modifier.Value);
                        break;
                    case ModifierType.Zero:
                        cachedModifiedValues[stat].value = 0;
                        return;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                modifiedValue += flatSum;
                modifiedValue *= percentSum;
                foreach (var t in multiplier)
                {
                    modifiedValue *= t;
                }
            }
            cachedModifiedValues[stat].value = modifiedValue;
            cachedModifiedValues[stat].needsUpdate = false;
        }
        
        public void AddModifier(ClassStat stat, StatModifier modifier)
        {
            if (!modifiers.ContainsKey(stat))
            {
                modifiers[stat] = new List<StatModifier>();
            }
            modifiers[stat].Add(modifier);
            cachedModifiedValues[stat].needsUpdate = true;
        }

        public void RemoveModifier(ClassStat stat, StatModifier modifier)
        {
            if (modifiers.TryGetValue(stat, out var list))
            {
                list.Remove(modifier);
            }
            cachedModifiedValues[stat].needsUpdate = true;
        }

        public float GetStat(ClassStat statName)
        {
            var stat = cachedModifiedValues[statName];
            if (!stat.needsUpdate) return stat.value;
            UpdateStat(statName);
            return stat.value;
        }
    }
    
     public enum ClassStat
        {
            MaxHealth,
            MaxStamina,
            StaminaRegen,
            Speed,
            AirAcceleration,
            MaxAirSpeed,
            JumpForce,
            Gravity,
            MaxVelocity,
            AttackDamage,
            AbilityDamage,
            ChargeSpeed,
            Defense,
            ReloadSpeed,
            DashSpeed,
            DashDuration,
            DashCooldown
        }
     
        [Serializable]
        public class StatModifier
        {
            public readonly ModifierType Type;
            public readonly float Value;

            private static readonly StatModifier StatNegator = new StatModifier(ModifierType.Zero, 0f);
            public static StatModifier Zero { get; } = StatNegator;

            public StatModifier(ModifierType type, float value)
            {
                Type = type;
                Value = value;
            }
            
        }

        [Serializable]
        public class ModifiedStat
        { 
            public bool needsUpdate = false; 
            public float value;

            public ModifiedStat(float value)
            {
                this.value = value;
            }
        }
        
        public enum ModifierType
        {
            Flat,
            Percent,
            Multiplier,
            Zero
        }
}