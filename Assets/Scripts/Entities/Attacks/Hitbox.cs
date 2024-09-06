using System;
using KBCore.Refs;
using Player.Classes;
using UnityEngine;
using UnityEngine.Serialization;

namespace Entities.Attacks
{
    public class Hitbox : ValidatedMonoBehaviour
    {
        [SerializeField, Anywhere] private InterfaceRef<CharacterClass> owner;
        [SerializeField] public DamageComponent damage;
        [SerializeField, Self] private BoxCollider box;
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("Ey");
            if (!other.TryGetComponent<IVisitable>(out var visitable)) return;
            damage.damageAmount *= owner.Value.GetCurrentStat(ClassStat.AttackDamage);
            damage.Visit(visitable);
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