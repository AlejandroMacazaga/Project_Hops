using UnityEngine;

namespace Utils.Flyweight
{
    public abstract class FlyweightSettings : ScriptableObject {
        public string type;
        public GameObject prefab;

        public abstract Flyweight Create();
        
        public virtual void OnGet(Flyweight f) => f.gameObject.SetActive(true);
        public virtual void OnRelease(Flyweight f) => f.gameObject.SetActive(false);
        public virtual void OnDestroyPoolObject(Flyweight f) => Destroy(f.gameObject);
    }
}