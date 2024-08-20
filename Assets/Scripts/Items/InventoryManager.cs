using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using Utils.EventBus;
using Utils.Singletons;

namespace Items
{
    public class InventoryManager : PersistentSingleton<InventoryManager>
    {
        public SerializedDictionary<ResourceItem, int> resourceInventory = new();
        public SerializedDictionary<ConsumableItem, int> consumableInventory = new();
        public SerializedDictionary<KeyItem, int> keyInventory = new();
        
        public void Load(SerializedDictionary<ResourceItem, int> loadedInventory)
        {
            resourceInventory = loadedInventory;
        }
        
        
        public void Load(SerializedDictionary<ConsumableItem, int> loadedInventory)
        {
            consumableInventory = loadedInventory;
        }
        
        
        public void Add(ResourceItem item, int quantity)
        {
            if (resourceInventory.ContainsKey(item))
            {
                var currentQuantity = resourceInventory[item];
                resourceInventory[item] = quantity + currentQuantity;
                EventBus<ResourceInventoryUpdateEvent>.Raise(new ResourceInventoryUpdateEvent() {Item = item, Quantity = quantity + currentQuantity});
            }
            else
            {
                resourceInventory.Add(item, quantity);
                EventBus<ResourceInventoryUpdateEvent>.Raise(new ResourceInventoryUpdateEvent() {Item = item, Quantity = quantity});
            }
        }

        public bool Remove(ResourceItem item, int quantity)
        {
            if (!resourceInventory.ContainsKey(item)) return false;
            var currentQuantity = resourceInventory[item];
            if (quantity > currentQuantity) return false;
            resourceInventory[item] = quantity - currentQuantity;
            EventBus<ResourceInventoryUpdateEvent>.Raise(new ResourceInventoryUpdateEvent() {Item = item, Quantity = quantity - currentQuantity});
            return true;
        }
        
        public void Add(ConsumableItem item, int quantity)
        {
            if (consumableInventory.ContainsKey(item))
            {
                var currentQuantity = consumableInventory[item];
                consumableInventory[item] = quantity + currentQuantity;
                EventBus<ConsumableInventoryUpdateEvent>.Raise(new ConsumableInventoryUpdateEvent() {Item = item, Quantity = quantity + currentQuantity});
            }
            else
            {
                consumableInventory.Add(item, quantity);
                EventBus<ConsumableInventoryUpdateEvent>.Raise(new ConsumableInventoryUpdateEvent() {Item = item, Quantity = quantity});
            }
        }

        public bool Remove(ConsumableItem item, int quantity)
        {
            if (!consumableInventory.ContainsKey(item)) return false;
            var currentQuantity = consumableInventory[item];
            if (quantity > currentQuantity) return false;
            consumableInventory[item] = quantity - currentQuantity;
            EventBus<ConsumableInventoryUpdateEvent>.Raise(new ConsumableInventoryUpdateEvent() {Item = item, Quantity = quantity - currentQuantity});
            return true;
        }
    }
    
    
    public struct ConsumableInventoryUpdateEvent : IEvent
    {
        public ConsumableItem Item;
        public int Quantity;
    }

    public struct ResourceInventoryUpdateEvent : IEvent
    {
        public ResourceItem Item;
        public int Quantity;
    }
}
