using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Player
{
    [CreateAssetMenu(menuName = "Player/PlayerData")]
    public class PlayerData : Entities.EntityData
    {
        public float MaxSpeed;
        public float Acceleration;
        public float JumpForce;
        public float Gravity;
        public float RotationSpeed;
        public float Damage;
        public float AttackSpeed;
        public float Defense;

        
        
    }
}