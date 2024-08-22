using UnityEngine;

namespace Items
{
    [CreateAssetMenu(menuName = "Items/Consumable", fileName = "New Consumable", order = 0)]
    public abstract class ConsumableItem : BaseItem
    {


        public abstract void Consume();
    }
    
}