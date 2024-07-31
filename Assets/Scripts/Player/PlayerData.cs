using UnityEngine;

namespace Player
{
    [CreateAssetMenu(menuName = "Entities/Player/PlayerData")]
    public class PlayerData : Entities.EntityData
    {
        [Header("Ground Movement Data")]
        [Tooltip("Flat")]public float speed;
        [Tooltip("Flat")]public float acceleration;
        
        [Header("Air Movement Data")]
        [Tooltip("Flat")]public float jumpForce;
        [Tooltip("Flat")]public float gravity;
        
        [Header("Combat Data")]
        [Tooltip("Multiplier")][Range(0.1f, 10f)] public float damage = 1;
        [Tooltip("Multiplier")][Range(0.1f, 10f)] public float attackSpeed;
        [Tooltip("Flat")] public float defense;
        [Tooltip("Multiplier")][Range(0.1f, 10f)] public float reloadSpeed;

        [Header("Dash Data")]
        [Tooltip("Multiplier")][Range(0.1f, 10f)] public float dashMultiplier;
        [Tooltip("Seconds")] public float dashDuration;
        [Tooltip("Seconds")] public float dashCooldown;


    }
}