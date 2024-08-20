using System.Collections.Generic;
using UnityEngine;

namespace Items
{
    public class ItemDatabase : ScriptableObject, ISerializationCallbackReceiver
    {
        public BaseItem[] items;
        public Dictionary<BaseItem, int> GetId = new ();
        public void OnBeforeSerialize()
        {
            GetId = new Dictionary<BaseItem, int>();
            for (var i = 0; i < items.Length; i++) GetId.Add(items[i], i);
        }

        public void OnAfterDeserialize()
        {
            
        }
    }
}