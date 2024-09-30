using Entities.Attacks;
using Items;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.Serialization;
using Utils.Flyweight;

namespace Entities
{
   public class RecollectComponent : ValidatedMonoBehaviour, IVisitable
   {
      public AttackEffect vulnerability;

      public PickupSettings toDrop;

      public int amountOfDrops;
      public void Accept(IVisitor visitor)
      {
         visitor.Visit(this);
      }

      public void Load()
      {
         
      }

      public void Collect()
      {
         Pickup pickup = (Pickup) FlyweightManager.Spawn(toDrop);
         pickup.transform.position = transform.position;
         pickup.transform.rotation = transform.rotation;
         amountOfDrops--;
         if (amountOfDrops <= 0) Deactivate();
      }

      private void Deactivate()
      {
         gameObject.SetActive(false);
      }
      
      
      
      
   }
}
