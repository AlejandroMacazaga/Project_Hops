using System.Collections.Generic;
using KBCore.Refs;
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

      private void Start()
      {
         system ??= GetComponent<ParticleSystem>();
         system.trigger.AddCollider(GameObject.FindWithTag("PickupArea").transform);
         system.emission.SetBurst(0, new ParticleSystem.Burst()
         {
            cycleCount = 1,
            count = new ParticleSystem.MinMaxCurve(settings.amountOfParticles)
         });
      }

      void OnEnable()
      {
         system.Play();
      }
      
      void OnValidate()
      {
         this.ValidateRefs();
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
