using System;
using System.Collections.Generic;
using MEC;
using Player.Events;
using UnityEngine;
using UnityEngine.Events;
using Utils.EventBus;

namespace Entities
{
    public class StaminaComponent : MonoBehaviour, IVisitable
    {
        [SerializeField] public float staminaPoints;

        [SerializeField] public float maxStaminaPoints;

        [SerializeField] public float staminaRegeneration;

        private CoroutineHandle _regenHandler;
        private CoroutineHandle _useHandler;


        void Start()
        {

        }

        public void SetValues(StaminaData data)
        {
            staminaPoints = data.staminaPoints;
            maxStaminaPoints = data.maxStaminaPoints;
            staminaRegeneration = data.staminaRegeneration;
        }

        public void Regenerate(float amount)
        {
            staminaPoints += amount;
            if (staminaPoints > maxStaminaPoints) staminaPoints = maxStaminaPoints;

            RaiseEvent();
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

        public void UseContinuously(float usePerSecond)
        {
            _useHandler = Timing.RunCoroutine(DrainStamina(usePerSecond));
        }

        public void StopUseContinously()
        {
            Timing.KillCoroutines(_useHandler);
        }



        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }

        IEnumerator<float> DrainStamina(float usePerSecond)
        {
            while (staminaPoints > 0)
            {
                staminaPoints -= usePerSecond * Time.deltaTime;
                RaiseEvent();
                yield return Timing.WaitForOneFrame;
            }
            staminaPoints = 0;
            RaiseEvent();
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
                RaiseEvent();
                yield return Timing.WaitForOneFrame;
            }

            staminaPoints = maxStaminaPoints;
            RaiseEvent();
        }

        private void RaiseEvent()
        {
            EventBus<PlayerStaminaChange>.Raise(new PlayerStaminaChange()
            {
                Current = staminaPoints
            });
        }

        public void Accept(object visitor)
        {
            throw new NotImplementedException();
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