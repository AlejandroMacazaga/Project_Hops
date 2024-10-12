using System;
using System.Collections.Generic;
using Entities.Enemies;
using KBCore.Refs;
using MEC;
using UnityEngine;

namespace Entities.Attacks
{
    public class EnemyHitbox : ValidatedMonoBehaviour
    {
        [SerializeField, Anywhere] public InterfaceRef<IEnemy> owner; 
        [SerializeField] public Damage damage;
        [SerializeField, Self] private BoxCollider box;
        public event Action<bool> ActiveChange = delegate {};
        private CoroutineHandle _activeHandle;
        private void OnTriggerEnter(Collider other)
        {
            if ((owner.Value as IStunned)!.IsStunned())
            {
                DeactivateAndActivate(owner.Value.GetStat(EnemyStat.AttackSpeed));
                return;
            }
            if (!other.TryGetComponent<IVisitable>(out var visitable)) return;
            damage.damageAmount *= owner.Value.GetStat(EnemyStat.AttackDamage);
            var damaged = other.GetComponents<IVisitable>();
            foreach (var dam in damaged)
            {
                damage.Visit(dam);
            }

            DeactivateAndActivate(owner.Value.GetStat(EnemyStat.AttackSpeed));
        }
        
        public void DeactivateAndActivate(float activeTime)
        {
            _activeHandle = Timing.RunCoroutine(Activate(activeTime));
        }
        
        private IEnumerator<float> Activate(float activeTime)
        {
            box.enabled = false;
            ActiveChange?.Invoke(box.enabled);
            yield return Timing.WaitForSeconds(activeTime);
            box.enabled = true;
            ActiveChange?.Invoke(box.enabled);
        }
        
        
    }
}