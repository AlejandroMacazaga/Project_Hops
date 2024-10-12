using UnityEngine;

namespace Items
{
    [CreateAssetMenu(menuName = "Items/BaseItem", fileName = "ItemObject", order = 0)]
    public class BaseItem : ScriptableObject
    {
        public string itemName;
        public Sprite sprite;
    }
}