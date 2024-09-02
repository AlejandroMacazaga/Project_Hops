using Player;
using Player.Classes;
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
            _mod ??= new StatModifier(ModifierType.Percent, speedModifier);
            if (duration != 0)
                _modWithTime ??= new StatModifier(ModifierType.Percent, speedModifier);
        }
        private void OnTriggerEnter(Collider other)
        {
            var controller = other.GetComponent<PlayerController>();
            if (!controller) return;
            controller.classData.AddModifier(ClassStat.Speed, _mod);
        }

        private void OnTriggerExit(Collider other)
        {
            var controller = other.GetComponent<PlayerController>();
            if (!controller) return;
            controller.classData.RemoveModifier(ClassStat.Speed,_mod);
            if (duration != 0) controller.classData.AddModifier(ClassStat.Speed, _modWithTime);
        }
    }
}