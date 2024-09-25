using Entities;
using Items;
using Player.Classes.Reaper;
using UnityEngine;
using UnityEngine.Serialization;

namespace Enemies
{
    [CreateAssetMenu(menuName = "Entities/Enemies/BasicEnemyData")]
    public class BasicEnemyData : EntityData
    {
        public PickupSettings pickupSettings;
        public EnergyPickupSettings energyPickupSettings;
    }
}