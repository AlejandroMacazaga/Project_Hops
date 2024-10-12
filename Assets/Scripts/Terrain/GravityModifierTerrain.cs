using Player;
using Player.Classes;
using UnityEngine;

namespace Terrain
{
    public class GravityModifierTerrain : MonoBehaviour
    {
        [SerializeField] [Range(0.0f, 10.0f)] private float gravityMultiplier = 1f;

        private StatModifier _mod;
        private void Awake()
        {
            _mod ??= new StatModifier(ModifierType.Multiplier, gravityMultiplier);
        }
        private void OnTriggerEnter(Collider other)
        {
            var controller = other.GetComponent<CharacterClass>();
            if (!controller) return;
            controller.data.AddModifier(ClassStat.Gravity, _mod);
        }

        private void OnTriggerExit(Collider other)
        {
            Debug.Log("Agur");
            var controller = other.GetComponent<CharacterClass>();
            if (!controller) return;
            controller.data.RemoveModifier(ClassStat.Gravity, _mod);
        }
    }
}