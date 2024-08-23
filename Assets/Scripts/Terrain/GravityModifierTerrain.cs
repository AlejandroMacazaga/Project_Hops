using Player;
using UnityEngine;

namespace Terrain
{
    public class GravityModifierTerrain : MonoBehaviour
    {
        [SerializeField] [Range(0.0f, 10.0f)] private float gravityMultiplier = 1f;

        private StatModifier _mod;
        private void Awake()
        {
            _mod ??= new StatModifier(StatModifier.ModifierType.Percent, gravityMultiplier);
        }
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("Hello");
            var controller = other.GetComponent<PlayerController>();
            if (!controller) return;
            controller.PlayerStats.AddModifier(PlayerStat.Gravity, _mod);
        }

        private void OnTriggerExit(Collider other)
        {
            Debug.Log("Agur");
            var controller = other.GetComponent<PlayerController>();
            if (!controller) return;
            controller.PlayerStats.RemoveModifier(PlayerStat.Gravity, _mod);
        }
    }
}