using UnityEngine;
using Utils.Flyweight;

namespace Projectiles
{
    public class Projectile : Flyweight
    {
        new ProjectileSettings settings => (ProjectileSettings)base.settings;
        
        private void OnEnable()
        {
            StartCoroutine(DespawnAfterDelay());
        }
        
        private void Update()
        {
            transform.Translate(Vector3.forward * (settings.speed * Time.deltaTime));
        }
        
        private void OnTriggerEnter(Collider other)
        {
            FlyweightManager.Instance.ReturnToPool(this);
        }
        
        
        
    }
}