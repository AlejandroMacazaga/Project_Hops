using System;
using System.Collections.Generic;
using MEC;
using Player.Events;
using UnityEngine;
using Utils.EventBus;

namespace Entities
{
    public class StaminaComponent : MonoBehaviour, IVisitable
    {
        [SerializeField] public float staminaPoints;

        [SerializeField] public float maxStaminaPoints;

        [SerializeField] public float staminaRegeneration;

        private CoroutineHandle _regenHandler;

        void Start()
        {
            
        }
        public void SetValues(StaminaData data)
        {
            staminaPoints = data.staminaPoints;
            maxStaminaPoints = data.maxStaminaPoints;
            staminaRegeneration = data.staminaRegeneration;
        }

        public bool UseStamina(float amount)
        {
            
            if (staminaPoints > amount) staminaPoints -= amount;
            else return false;
            EventBus<PlayerStaminaChange>.Raise(new PlayerStaminaChange()
            {
                Current = staminaPoints
            });
            Timing.KillCoroutines(_regenHandler);
            _regenHandler = Timing.RunCoroutine(RegenerateStamina(0.8f));
            return true;
        }
        
        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }

        IEnumerator<float> RegenerateStamina(float delay)
        {
            if (delay > 0f)
            {
                yield return Timing.WaitForSeconds(delay);
            }

            while (staminaPoints < maxStaminaPoints)
            {
                staminaPoints += staminaRegeneration * Time.deltaTime;
                EventBus<PlayerStaminaChange>.Raise(new PlayerStaminaChange()
                {
                    Current = staminaPoints
                });
                yield return Timing.WaitForOneFrame;
            }
            staminaPoints = maxStaminaPoints;
            EventBus<PlayerStaminaChange>.Raise(new PlayerStaminaChange()
            {
                Current = staminaPoints
            });
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