using UnityEngine;

namespace Player
{
    [CreateAssetMenu(menuName = "Player/PlayerData")]
    public class PlayerData : Entities.EntityData
    {
        [Tooltip("Flat")]public float speed;
        [Tooltip("Flat")]public float acceleration;
        [Tooltip("Flat")]public float jumpForce;
        [Tooltip("Flat")]public float gravity;
        [Tooltip("Multiplier")][Range(0.1f, 10f)] public float rotationSpeed;
        [Tooltip("Multiplier")][Range(0.1f, 10f)] public float damage = 1;
        [Tooltip("Multiplier")][Range(0.1f, 10f)] public float attackSpeed;
        [Tooltip("Flat")] public float defense;
        [Tooltip("Multiplier")][Range(0.1f, 10f)] public float reloadSpeed;

        [Header("Dash data")]
        [Tooltip("Multiplier")][Range(0.1f, 10f)] public float dashMultiplier;
        [Tooltip("Seconds")] public float dashDuration;
        [Tooltip("Seconds")] public float dashCooldown;


    }
}