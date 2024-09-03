using System;
using UnityEngine;
using Utils.Timers;

namespace Entities
{
    public class StaminaComponent : MonoBehaviour, IVisitable
    {
        [SerializeField] public float staminaPoints;

        [SerializeField] public float maxStaminaPoints;

        [SerializeField] public float staminaRegeneration;

        void Start()
        {
            
          
        }
        
        public void SetValues(StaminaData data)
        {
            staminaPoints = data.staminaPoints;
            maxStaminaPoints = data.maxStaminaPoints;
            staminaRegeneration = data.staminaRegeneration;
        }
        
        public void Accept(IVisitor visitor)
        {
            throw new System.NotImplementedException();
        }
    }
    
    [Serializable]
    public struct StaminaData
    {
        public float staminaPoints;

        public float maxStaminaPoints;
        public float staminaRegeneration;
    }
}