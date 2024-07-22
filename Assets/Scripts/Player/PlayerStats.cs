using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils.Timers;

namespace Scripts.Player
{
    public class StatModifier
    {
        public enum ModifierType
        {
            Flat,
            Percent
        }
        public ModifierType type;
        public float value;
        public string name;
        private CountdownTimer durationTimer;
        public Action<StatModifier> ModifierEnd;

        public StatModifier(ModifierType type, float value, float duration = 0)
        {
            this.type = type;
            this.value = value;
            if (duration > 0)
            {

                durationTimer = new CountdownTimer(duration);
                durationTimer.Start();
                durationTimer.OnTimerStop += OnModifierEnd;
            }
        }

        private void OnModifierEnd()
        {
            ModifierEnd?.Invoke(this);
        }
    }

    public class ModifiedStat
    {
        public bool needsUpdate;
        public float value;
    
        public ModifiedStat(float value)
        {
            needsUpdate = false;
            this.value = value;
        }

        
    }

    public class PlayerStats
    {
        readonly PlayerData baseStats;
        private Dictionary<string, List<StatModifier>> modifiers = new();
        private Dictionary<string, ModifiedStat> cachedModifiedValues = new();


        public PlayerStats(PlayerData stats)
        {
            baseStats = stats;
            InitializeCache();
        }

        private void InitializeCache()
        {
            cachedModifiedValues["MaxHealth"] = new ModifiedStat(baseStats.HealthData.MaxHealthPoints);
            cachedModifiedValues["MaxSpeed"] = new ModifiedStat(baseStats.MaxSpeed);
            cachedModifiedValues["Acceleration"] = new ModifiedStat(baseStats.Acceleration);
            cachedModifiedValues["JumpForce"] = new ModifiedStat(baseStats.JumpForce);
            cachedModifiedValues["Gravity"] = new ModifiedStat(baseStats.Gravity);
            cachedModifiedValues["RotationSpeed"] = new ModifiedStat(baseStats.RotationSpeed);
            cachedModifiedValues["Damage"] = new ModifiedStat(baseStats.Damage);
            cachedModifiedValues["AttackSpeed"] = new ModifiedStat(baseStats.AttackSpeed);
            cachedModifiedValues["Defense"] = new ModifiedStat(baseStats.Defense);
            // Add new values here depending of the new values the player has
        }

        public void AddModifier(string statName, StatModifier modifier)
        {
            if (!modifiers.ContainsKey(statName))
            {
                modifiers[statName] = new List<StatModifier>();
            }

            modifier.ModifierEnd = m => RemoveModifier(statName, m);
            modifiers[statName].Add(modifier);
            modifiers[statName].OrderByDescending(mod => mod.type);
            cachedModifiedValues[statName].needsUpdate = true;


        }

        public void RemoveModifier(string statName, StatModifier modifier)
        {
            if (modifiers.ContainsKey(statName))
            {
                modifiers[statName].Remove(modifier);
                cachedModifiedValues[statName].needsUpdate = true;

            }
        }

        public float GetStat(string statName)
        {
            var stat = cachedModifiedValues[statName];
            if (stat.needsUpdate)
            {
                stat.value = UpdateStat(statName);
                stat.needsUpdate = false;
            }
            return stat.value;
        }

        private float UpdateStat(string statName)
        {
            var baseValue = GetBaseStat(statName);
            var modifiedValue = baseValue;

            foreach (var modifier in modifiers[statName])
            {
                var flatSum = 0f;
                var percentSum = 0f;
                switch (modifier.type)
                {
                    case StatModifier.ModifierType.Flat:
                        flatSum += modifier.value;
                        break;
                    case StatModifier.ModifierType.Percent:
                        percentSum += modifier.value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                modifiedValue = flatSum;
                modifiedValue *= (1 * percentSum / 100f);

            }

            return modifiedValue;
        }

        private float GetBaseStat(string statName)
        {
            return (float)baseStats.GetType().GetProperty(statName).GetValue(baseStats);
        }
    }

}