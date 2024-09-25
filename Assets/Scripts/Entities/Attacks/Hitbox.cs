using System;
using KBCore.Refs;
using Player.Classes;
using UnityEngine;
using UnityEngine.Serialization;
using Utils.Timers;

namespace Entities.Attacks
{
    public class Hitbox : ValidatedMonoBehaviour
    {
        [SerializeField, Anywhere] private InterfaceRef<CharacterClass> owner;
        [SerializeField] public DamageComponent damage;
        [SerializeField, Self] private BoxCollider box;
        
        void Start() {}
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

        public void Activate()
        {
            box.enabled = true;
        }

        public void Deactivate()
        {
            box.enabled = false;
        }
    }
}