using System;
using System.Collections;
using System.Reflection;
using Entities;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;
using Utils.Flyweight;

namespace Projectiles
{
    public class Projectile : Flyweight {
        new ProjectileSettings settings => (ProjectileSettings) base.settings;

        private DamageArea _hitbox;
        private Vector3 _lastPosition;
        

        public float currentDamage;

        void Awake()
        {
            _hitbox = GetComponent<DamageArea>();
            
        }
        void OnEnable() {
            StartCoroutine(DespawnAfterDelay(settings.despawnDelay));
            _lastPosition = transform.position;
            _hitbox.damageComponent.amount = currentDamage;
        }
        
        void FixedUpdate() {
            transform.Translate(Vector3.forward * (settings.speed * Time.fixedDeltaTime));
            
            _lastPosition = transform.position;
        }

        void OnTriggerEnter(Collider other)
        {
            StopCoroutine(nameof(DespawnAfterDelay));
            FlyweightManager.ReturnToPool(this);
        }

        IEnumerator DespawnAfterDelay(float delay) {
            yield return Helpers.GetWaitForSeconds(delay);
            FlyweightManager.ReturnToPool(this);
        }
    }
}