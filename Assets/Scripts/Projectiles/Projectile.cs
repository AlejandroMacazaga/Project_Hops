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
        
        private Vector3 _lastPosition;

        public float currentDamage;
        void OnEnable() {
            StartCoroutine(DespawnAfterDelay(settings.despawnDelay));
            _lastPosition = transform.position;
        }
        
        void FixedUpdate() {
            transform.Translate(Vector3.forward * (settings.speed * Time.fixedDeltaTime));

            Debug.DrawLine(transform.position, _lastPosition, Color.red, 1f);
            if (Physics.Linecast(transform.position, _lastPosition, out var hitInfo, LayerMask.NameToLayer("Player")))
            {
                transform.position = hitInfo.transform.position;
            }
            _lastPosition = transform.position;
        }

        void OnCollisionEnter(Collision other)
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