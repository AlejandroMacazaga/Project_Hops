using System.Collections.Generic;
using KBCore.Refs;
using MEC;
using Player.Classes.Reaper;
using Player.Events;
using UnityEngine;
using UnityEngine.Serialization;
using Utils.EventBus;
using Utils.Flyweight;

namespace Items
{
   public class EnergyPickup : Flyweight
   {
      new EnergyPickupSettings settings => (EnergyPickupSettings)base.settings;
   
      [SerializeField, HideInInspector, Self] private ParticleSystem system;
      private readonly List<ParticleSystem.Particle> _particles = new();
      private CoroutineHandle _forcesHandle;
      private void Start()
      {
         system ??= GetComponent<ParticleSystem>();
         system.trigger.AddCollider(GameObject.FindWithTag("PickupArea").transform);
         system.emission.SetBurst(0, new ParticleSystem.Burst()
         {
            cycleCount = 1,
            count = new ParticleSystem.MinMaxCurve(settings.amountOfParticles)
         });
         var forces = system.externalForces;
         forces.enabled = false;
      }

      void OnEnable()
      {
         system.Play();
         _forcesHandle = Timing.RunCoroutine(ActivateForce());
      }

      void OnDisable()
      {
         Timing.KillCoroutines(_forcesHandle);
      }
      
      void OnValidate()
      {
         this.ValidateRefs();
      }

      IEnumerator<float> ActivateForce()
      {
         yield return Timing.WaitForSeconds(1.5f);
         var forces = system.externalForces;
         forces.enabled = true;
      }
      
      void OnParticleTrigger()
      {
         int triggeredParticles = system.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, _particles);

         for (var i = 0; i < triggeredParticles; i++)
         {
            ParticleSystem.Particle p = _particles[i];
            p.remainingLifetime = 0f;
            EventBus<EnergyPickupEvent>.Raise(new EnergyPickupEvent()
            {
               Item = settings.item
            });
            _particles[i] = p;
         }
      
         system.SetTriggerParticles(ParticleSystemTriggerEventType.Enter, _particles);
         
         if (_particles.Count == settings.amountOfParticles)  FlyweightManager.ReturnToPool(this);
      }
   }
}
