using System.Collections;
using System.Collections.Generic;
using KBCore.Refs;
using MEC;
using UnityEngine;
using Utils;
using Utils.Flyweight;

namespace Items
{
    public class Pickup : Flyweight
    {
        new PickupSettings settings => (PickupSettings)base.settings;

        [SerializeField, Self]
        private ParticleSystem system;

        private readonly List<ParticleSystem.Particle> _particles = new();
        private CoroutineHandle _forcesHandle;
        private int _amount = 1;

        void OnEnable()
        {
            if (!settings) return;
            system.trigger.AddCollider(GameObject.FindWithTag("PickupArea").transform);
            _amount = Random.Range(settings.min, settings.max + 1);
            system.emission.SetBurst(0, new ParticleSystem.Burst()
            {
                cycleCount = 1,
                count = new ParticleSystem.MinMaxCurve(_amount)
            });
            _forcesHandle = Timing.RunCoroutine(ActivateForce());
            system.Play();
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

            if (_particles.Count == _amount) FlyweightManager.ReturnToPool(this);
        }

        IEnumerator DespawnAfterDelay(float delay)
        {
            yield return Helpers.GetWaitForSeconds(delay);
            FlyweightManager.ReturnToPool(this);
        }
    }
}