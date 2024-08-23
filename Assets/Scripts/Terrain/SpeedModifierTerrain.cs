using Player;
using UnityEngine;

namespace Terrain
{
    public class SpeedModifierTerrain : MonoBehaviour
    {
        [SerializeField] [Range(0.0f, 10.0f)] private float speedModifier = 1f;
        [SerializeField] [Range(0.0f, 120.0f)] private float duration = 0f;
        private StatModifier _mod;
        private StatModifier _modWithTime;

        private void Awake()
        {
            _mod ??= new StatModifier(StatModifier.ModifierType.Percent, speedModifier);
            if (duration != 0)
                _modWithTime ??= new StatModifier(StatModifier.ModifierType.Percent, speedModifier, duration);
        }
        private void OnTriggerEnter(Collider other)
        {
            var controller = other.GetComponent<PlayerController>();
            if (!controller) return;
            controller.PlayerStats.AddModifier(PlayerStat.Speed, _mod);
        }

        private void OnTriggerExit(Collider other)
        {
            var controller = other.GetComponent<PlayerController>();
            if (!controller) return;
            controller.PlayerStats.RemoveModifier(PlayerStat.Speed,_mod);
            if (duration != 0) controller.PlayerStats.AddModifier(PlayerStat.Speed, _modWithTime);
        }
    }
}