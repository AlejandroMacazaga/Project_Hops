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
            Percent,
            Zero
        }
        public readonly ModifierType Type;
        public readonly float Value;
        public Action<StatModifier> ModifierEnd;

        public StatModifier(ModifierType type, float value = 0, float duration = 0)
        {
            Type = type;
            Value = value;
            if (duration == 0) return;
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
        private readonly Dictionary<PlayerStat, List<StatModifier>> _modifiers = new();
        private readonly Dictionary<PlayerStat, ModifiedStat> _cachedModifiedValues = new();


        public PlayerStats(PlayerData stats)
        {
            _baseStats = stats;
            InitializeCache();
        }

        private void InitializeCache()
        {
            _cachedModifiedValues[PlayerStat.MaxHealth] = new ModifiedStat(_baseStats.healthData.maxHealthPoints);
            _cachedModifiedValues[ PlayerStat.Speed] = new ModifiedStat(_baseStats.speed);
            _cachedModifiedValues[PlayerStat.Acceleration] = new ModifiedStat(_baseStats.acceleration);
            _cachedModifiedValues[ PlayerStat.JumpForce] = new ModifiedStat(_baseStats.jumpForce);
            _cachedModifiedValues[PlayerStat.Gravity] = new ModifiedStat(_baseStats.gravity);
            _cachedModifiedValues[ PlayerStat.Damage] = new ModifiedStat(_baseStats.damage);
            _cachedModifiedValues[PlayerStat.AttackSpeed] = new ModifiedStat(_baseStats.attackSpeed);
            _cachedModifiedValues[PlayerStat.Defense] = new ModifiedStat(_baseStats.defense);
            _cachedModifiedValues[PlayerStat.ReloadSpeed] = new ModifiedStat(_baseStats.reloadSpeed);
            _cachedModifiedValues[ PlayerStat.DashMultiplier] = new ModifiedStat(_baseStats.dashMultiplier);
            _cachedModifiedValues[PlayerStat.DashDuration] = new ModifiedStat(_baseStats.dashDuration);
            _cachedModifiedValues[PlayerStat.DashCooldown] = new ModifiedStat(_baseStats.dashCooldown);
            // Add new values here depending of the new values the player has
        }

        public void AddModifier(PlayerStat statName, StatModifier modifier)
        {
            if (!_modifiers.ContainsKey(statName))
            {
                _modifiers[statName] = new List<StatModifier>();
            }

            modifier.ModifierEnd = m => RemoveModifier(statName, m);
            _modifiers[statName].Add(modifier);
            _cachedModifiedValues[statName].NeedsUpdate = true;


        }

        public void RemoveModifier(PlayerStat statName, StatModifier modifier)
        {
            if (!_modifiers.TryGetValue(statName, out var modifiers)) return;
            modifiers.Remove(modifier);
            _cachedModifiedValues[statName].NeedsUpdate = true;
        }

        public float GetStat(PlayerStat statName)
        {
            var stat = _cachedModifiedValues[statName];
            if (!stat.NeedsUpdate) return stat.Value;
            stat.Value = UpdateStat(statName);
            stat.NeedsUpdate = false;
            return stat.Value;
        }

        private float UpdateStat(PlayerStat statName)
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
                    case StatModifier.ModifierType.Zero:
                        return 0;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                modifiedValue += flatSum;
                modifiedValue *= percentSum;
            }
            return modifiedValue;
        }

        private float GetBaseStat(PlayerStat statName)
        {
            return statName switch
            {
                PlayerStat.MaxHealth => _baseStats.healthData.maxHealthPoints,
                PlayerStat.Speed => _baseStats.speed,
                PlayerStat.Acceleration => _baseStats.acceleration,
                PlayerStat.JumpForce => _baseStats.jumpForce,
                PlayerStat.Gravity => _baseStats.gravity,
                PlayerStat.Damage => _baseStats.damage,
                PlayerStat.AttackSpeed => _baseStats.attackSpeed,
                PlayerStat.Defense => _baseStats.defense,
                PlayerStat.ReloadSpeed => _baseStats.reloadSpeed,
                PlayerStat.DashMultiplier => _baseStats.dashMultiplier,
                PlayerStat.DashDuration => _baseStats.dashDuration,
                PlayerStat.DashCooldown => _baseStats.dashCooldown,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }

    public enum PlayerStat
    {
        MaxHealth,
        Speed,
        Acceleration,
        JumpForce,
        Gravity,
        Damage,
        AttackSpeed,
        Defense,
        ReloadSpeed,
        DashMultiplier,
        DashDuration,
        DashCooldown
    }
}