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
            Debug.Log("Hello");
            var controller = other.GetComponent<PlayerController>();
            if (!controller) return;
            controller.classData.AddModifier(ClassStat.Gravity, _mod);
        }

        private void OnTriggerExit(Collider other)
        {
            Debug.Log("Agur");
            var controller = other.GetComponent<PlayerController>();
            if (!controller) return;
            controller.classData.RemoveModifier(ClassStat.Gravity, _mod);
        }
    }
}