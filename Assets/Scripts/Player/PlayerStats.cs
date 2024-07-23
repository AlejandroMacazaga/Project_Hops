using System;
using System.Collections.Generic;
using Utils.Timers;

namespace Player
{
    public class StatModifier
    {
        public enum ModifierType
        {
            Flat,
            Percent
        }
        public readonly ModifierType Type;
        public readonly float Value;
        public Action<StatModifier> ModifierEnd;

        public StatModifier(ModifierType type, float value, float duration = 0)
        {
            Type = type;
            Value = value;
            if (!(duration > 0)) return;
            var durationTimer = new CountdownTimer(duration);
            durationTimer.Start();
            durationTimer.OnTimerStop += OnModifierEnd;
        }

        private void OnModifierEnd()
        {
            ModifierEnd?.Invoke(this);
        }
    }

    public class ModifiedStat
    {
        public bool NeedsUpdate;
        public float Value;
    
        public ModifiedStat(float value)
        {
            NeedsUpdate = false;
            this.Value = value;
        }

        
    }

    public class PlayerStats
    {
        private readonly PlayerData _baseStats;
        private readonly Dictionary<string, List<StatModifier>> _modifiers = new();
        private readonly Dictionary<string, ModifiedStat> _cachedModifiedValues = new();


        public PlayerStats(PlayerData stats)
        {
            _baseStats = stats;
            InitializeCache();
        }

        private void InitializeCache()
        {
            _cachedModifiedValues["MaxHealth"] = new ModifiedStat(_baseStats.healthData.maxHealthPoints);
            _cachedModifiedValues["MaxSpeed"] = new ModifiedStat(_baseStats.maxSpeed);
            _cachedModifiedValues["Acceleration"] = new ModifiedStat(_baseStats.acceleration);
            _cachedModifiedValues["JumpForce"] = new ModifiedStat(_baseStats.jumpForce);
            _cachedModifiedValues["Gravity"] = new ModifiedStat(_baseStats.gravity);
            _cachedModifiedValues["RotationSpeed"] = new ModifiedStat(_baseStats.rotationSpeed);
            _cachedModifiedValues["Damage"] = new ModifiedStat(_baseStats.damage);
            _cachedModifiedValues["AttackSpeed"] = new ModifiedStat(_baseStats.attackSpeed);
            _cachedModifiedValues["Defense"] = new ModifiedStat(_baseStats.defense);
            // Add new values here depending of the new values the player has
        }

        public void AddModifier(string statName, StatModifier modifier)
        {
            if (!_modifiers.ContainsKey(statName))
            {
                _modifiers[statName] = new List<StatModifier>();
            }

            modifier.ModifierEnd = m => RemoveModifier(statName, m);
            _modifiers[statName].Add(modifier);
            _cachedModifiedValues[statName].NeedsUpdate = true;


        }

        public void RemoveModifier(string statName, StatModifier modifier)
        {
            if (!_modifiers.ContainsKey(statName)) return;
            _modifiers[statName].Remove(modifier);
            _cachedModifiedValues[statName].NeedsUpdate = true;
        }

        public float GetStat(string statName)
        {
            var stat = _cachedModifiedValues[statName];
            if (!stat.NeedsUpdate) return stat.Value;
            stat.Value = UpdateStat(statName);
            stat.NeedsUpdate = false;
            return stat.Value;
        }

        private float UpdateStat(string statName)
        {
            var baseValue = GetBaseStat(statName);
            var modifiedValue = baseValue;
            foreach (var modifier in _modifiers[statName])
            {
                var flatSum = 0f;
                var percentSum = 0f;
                switch (modifier.Type)
                {
                    case StatModifier.ModifierType.Flat:
                        flatSum += modifier.Value;
                        break;
                    case StatModifier.ModifierType.Percent:
                        percentSum += modifier.Value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                modifiedValue += flatSum;
                modifiedValue *= percentSum;
            }
            return modifiedValue;
        }

        private float GetBaseStat(string statName)
        {
            return statName switch
            {
                "MaxHealth" => _baseStats.healthData.maxHealthPoints,
                "MaxSpeed" => _baseStats.maxSpeed,
                "Acceleration" => _baseStats.acceleration,
                "JumpForce" => _baseStats.jumpForce,
                "Gravity" => _baseStats.gravity,
                "RotationSpeed" => _baseStats.rotationSpeed,
                "Damage" => _baseStats.damage,
                "AttackSpeed" => _baseStats.attackSpeed,
                "Defense" => _baseStats.defense,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }

}