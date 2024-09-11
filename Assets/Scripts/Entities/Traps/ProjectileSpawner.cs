using System;
using System.Collections.Generic;
using KBCore.Refs;
using MEC;
using Projectiles;
using UnityEngine;
using UnityEngine.Serialization;
using Utils.Flyweight;

namespace Entities.Traps
{
    public class ProjectileSpawner : ValidatedMonoBehaviour
    {
        [SerializeField] private ProjectileSettings settings;

        [SerializeField] private bool canShoot = true;
        [SerializeField] private float rateOfFire = 1f;
        [SerializeField, Child] private Transform spawnPosition;
        private CoroutineHandle _handler;

        private void Start()
        {
            _handler = Timing.RunCoroutine(ShootPeriodically());
        }
        private IEnumerator<float> ShootPeriodically()
        {
            while (canShoot)
            {
                var instance = (Projectile) FlyweightManager.Spawn(settings);
                instance.currentDamage = 5;
                instance.transform.position = spawnPosition.position;
                instance.transform.rotation =  spawnPosition.rotation;
                yield return Timing.WaitForSeconds(rateOfFire);
            }
        }
    }
}
