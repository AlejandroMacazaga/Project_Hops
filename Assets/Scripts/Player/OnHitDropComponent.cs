using Entities;
using Items;
using Player.Classes.Reaper;
using UnityEngine;
using Utils.Flyweight;

namespace Player
{
    public class OnHitDropComponent : MonoBehaviour, IVisitable
    {
        public EnergyPickupSettings energy;
        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }

        public void Drop(float multiplier)
        {
            EnergyPickup pickup = (EnergyPickup) FlyweightManager.Spawn(energy);
            pickup.transform.position = transform.position;
            pickup.transform.rotation = transform.rotation;
        }
    }
}