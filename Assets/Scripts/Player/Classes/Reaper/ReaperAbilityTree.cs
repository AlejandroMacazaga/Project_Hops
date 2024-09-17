using System;
using System.Collections.Generic;
using System.Linq;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player.Classes.Reaper
{
    public class ReaperAbilityTree : ValidatedMonoBehaviour
    {
        public int unlockPoints = 0;
        public List<Ability> abilityTree;

        void UnlockAbilitiesFromSaveData()
        {
            // Get the ability tree from the current save data


        }
        
    }
}