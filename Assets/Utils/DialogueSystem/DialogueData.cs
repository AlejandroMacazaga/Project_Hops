using UnityEngine;

namespace Utils.DialogueSystem
{
    public class DialogueData : ScriptableObject
    {
        [System.Serializable]
        public class ItemText
        {
            public string idText;
            public string character;
            public string position;
            public string langEs;
            public string langEn;
            public string langCat;
            public string langEus;
            public string langChi;
        }

        public ItemText[] items;
    }
}