using System.Collections;
using UnityEngine;
using Utils;
using Utils.Flyweight;

namespace Projectiles
{
    public class Projectile : Flyweight {
        new ProjectileSettings settings => (ProjectileSettings) base.settings;
        
        void OnEnable() {
            StartCoroutine(DespawnAfterDelay(settings.despawnDelay));
        }
        
        void Update() {
            transform.Translate(Vector3.forward * (settings.speed * Time.deltaTime));
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