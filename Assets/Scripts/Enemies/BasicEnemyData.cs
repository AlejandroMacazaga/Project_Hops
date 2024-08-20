using Entities;
using Items;
using UnityEngine;
using UnityEngine.Serialization;

namespace Enemies
{
    [CreateAssetMenu(menuName = "Entities/Enemies/BasicEnemyData")]
    public class BasicEnemyData : EntityData
    {
        public PickupSettings pickupSettings;
    }
}