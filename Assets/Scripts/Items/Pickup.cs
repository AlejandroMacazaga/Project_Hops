using System.Collections;
using System.Collections.Generic;
using KBCore.Refs;
using MEC;
using Player;
using Player.Classes;
using Player.Events;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;
using Utils.EventBus;
using Utils.Flyweight;

namespace Items
{
    public class Pickup : Flyweight
    {
        new PickupSettings settings => (PickupSettings)base.settings;

        [SerializeField, HideInInspector, Self]
        private ParticleSystem system;

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
            yield return Timing.WaitForSeconds(0.5f);
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
                switch (settings.item)
                {
                    case ConsumableItem t1:
                        InventoryManager.Instance.Add(t1);
                        break;
                    case ResourceItem t2:
                        InventoryManager.Instance.Add(t2);
                        break;
                    case KeyItem t3:
                        InventoryManager.Instance.Add(t3);
                        break;
                }

                _particles[i] = p;
            }

            system.SetTriggerParticles(ParticleSystemTriggerEventType.Enter, _particles);

            if (_particles.Count == settings.amountOfParticles) FlyweightManager.ReturnToPool(this);
        }

        IEnumerator DespawnAfterDelay(float delay)
        {
            yield return Helpers.GetWaitForSeconds(delay);
            FlyweightManager.ReturnToPool(this);
        }
    }
}