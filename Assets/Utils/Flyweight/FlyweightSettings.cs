using UnityEngine;

namespace Utils.Flyweight
{
    [CreateAssetMenu(menuName = "Flyweight/Flyweight Settings")]
    public class FlyweightSettings : ScriptableObject
    {
        public GameObject prefab;
        [Header("Group Settings")]
        public string group;
        
        [Header("Pool Settings")]
        public int defaultCapacity;
        public int maxPoolSize;
        public float lifeTime = 5f;
        public bool collectionCheck;
        
        public virtual Flyweight Create()
        {
            var go = Instantiate(this.prefab);
            go.SetActive(false);
            go.name = prefab.name;
            
            var flyweight = prefab.AddComponent<Flyweight>();
            flyweight.settings = this;
            return flyweight;
        }
        
        public void OnGet(Flyweight flyweight) => flyweight.gameObject.SetActive(true);
    
        public void OnRelease(Flyweight flyweight) => flyweight.gameObject.SetActive(false);

        public void OnDestroyPoolObject(Flyweight flyweight) => Destroy(flyweight.gameObject);
    }
}