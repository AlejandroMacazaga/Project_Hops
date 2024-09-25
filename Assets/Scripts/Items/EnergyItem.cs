using UnityEngine;

namespace Items
{
    [CreateAssetMenu(menuName = "Items/Energy")]
    public class EnergyItem : BaseItem
    {
        public EnergyType type;
        public int amount;
    }

    public enum EnergyType
    {
        Health,
        Stamina,
        Blood
    }
}