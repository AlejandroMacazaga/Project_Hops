using System;
using System.Collections.Generic;
using KBCore.Refs;
using MEC;
using Player.Classes;
using UnityEngine;
using UnityEngine.Serialization;
using Utils.Timers;

namespace Entities.Attacks
{
    public class Hitbox : ValidatedMonoBehaviour
    {
        [SerializeField, Anywhere] private InterfaceRef<CharacterClass> owner;
        [SerializeField] public Damage damage;
        [SerializeField, Self] private BoxCollider box;
        private CoroutineHandle _activeHandle;
        public event Action<bool> ActiveChange = delegate {};
        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent<IVisitable>(out var visitable)) return;
            damage.damageAmount *= owner.Value.GetCurrentStat(ClassStat.AttackDamage);
            var damaged = other.GetComponents<IVisitable>();
            foreach (var dam in damaged)
            {
                damage.Visit(dam);
            }
        }

        public void ActivateHitbox(float activeTime)
        {
            _activeHandle = Timing.RunCoroutine(Activate(activeTime));
        }

        private IEnumerator<float> Activate(float activeTime)
        {
            box.enabled = true;
            ActiveChange?.Invoke(box.enabled);
            yield return Timing.WaitForSeconds(activeTime);
            box.enabled = false;
            ActiveChange?.Invoke(box.enabled);
        }
    }
}