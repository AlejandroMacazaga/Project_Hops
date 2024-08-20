using System.Collections;
using Player;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;
using Utils.Flyweight;

namespace Items
{
    public class Pickup : Flyweight 
    {
        new PickupSettings settings => (PickupSettings) base.settings;
        public GameObject spriteGameObject;

        void Awake()
        {
        }
        void OnEnable() {
            Debug.Log("Hello");
            spriteGameObject = transform.GetChild(0).gameObject;
            StartCoroutine(DespawnAfterDelay(settings.despawnDelay));
        }

        void FixedUpdate()
        {
            spriteGameObject.transform.Rotate(0, 5, 0);
        }
        
        void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<PlayerController>(out var player))
            {
                FlyweightManager.ReturnToPool(this);
                switch (settings.item)
                {
                    case ConsumableItem t1:
                        InventoryManager.Instance.Add(t1);
                        break;
                    case ResourceItem t2:
                        InventoryManager.Instance.Add(t2);
                        break;
                    case KeyItem t3:
                        InventoryManager.Instance.Add(t3);
                        break;
                }
            }
        }
        IEnumerator DespawnAfterDelay(float delay) {
            Debug.Log(delay);
            yield return Helpers.GetWaitForSeconds(delay);
            FlyweightManager.ReturnToPool(this);
        }
        
    }
}