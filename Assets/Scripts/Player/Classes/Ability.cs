using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Player.Classes
{
    [CreateAssetMenu(menuName = "Classes/Ability")]
    public class Ability : ScriptableObject
    {
        public readonly List<Ability> Prerequisits = new();
        public string abilityName;
        public bool isUnlocked = false;
        public int cost;
        

        public void AddPrerequisits(Ability a)
        {
            Prerequisits.Add(a);
        }

        public bool CanUnlock()
        {
            return Prerequisits.All(ability => ability.isUnlocked);
        }

        public bool UnlockAbility()
        {
            return isUnlocked = CanUnlock();
        }

    }

}