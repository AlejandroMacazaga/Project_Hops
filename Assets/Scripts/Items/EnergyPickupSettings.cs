using Items;
using UnityEngine;
using Utils.Extensions;
using Utils.Flyweight;

namespace Player.Classes.Reaper
{
    [CreateAssetMenu(menuName = "Flyweight/EnergyPickup")]
    public class EnergyPickupSettings : FlyweightSettings
    {
        public EnergyItem item;
        public int amountOfParticles;
        public override Flyweight Create()
        {
            var go = Instantiate(prefab);
            go.SetActive(false);
            go.name = prefab.name + " (pool " + type +")";
            var flyweight = go.GetOrAdd<EnergyPickup>();
            flyweight.settings = this;
            
            return flyweight;
        }
    }
}