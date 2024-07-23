using UnityEngine;

namespace Player
{
    [CreateAssetMenu(menuName = "Player/PlayerData")]
    public class PlayerData : Entities.EntityData
    {
        public float maxSpeed;
        public float acceleration;
        public float jumpForce;
        public float gravity;
        public float rotationSpeed;
        public float damage;
        public float attackSpeed;
        public float defense;

        
        
    }
}