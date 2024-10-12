using System;
using UnityEngine;
using UnityEngine.Serialization;
using Utils.Extensions;
using Utils.Flyweight;

namespace Items
{
    [CreateAssetMenu(menuName = "Flyweight/Pickup Settings")]
    public class PickupSettings : FlyweightSettings
    {
        public BaseItem item;
        public int min, max;
        public override Flyweight Create()
        {
            var go = Instantiate(prefab);
            go.SetActive(false);
            go.name = prefab.name + " (pool " + type +")";
            var flyweight = go.GetOrAdd<Pickup>();
            flyweight.settings = this;
            
            return flyweight;
        }
    }
}
