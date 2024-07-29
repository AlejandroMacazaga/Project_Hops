using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Utils.Singletons;

namespace Utils.Flyweight
{
    public class FlyweightManager : PersistentSingleton<FlyweightManager>
    {
        [SerializeField] bool collectionCheck = true;
        [SerializeField] int defaultCapacity = 10;
        [SerializeField] int maxPoolSize = 100;
        private readonly Dictionary<string, IObjectPool<Flyweight>> _pools = new();
        
        public static Flyweight Spawn(FlyweightSettings settings) => Instance.GetPoolFor(settings)?.Get();
        public static void ReturnToPool(Flyweight f) => Instance.GetPoolFor(f.settings)?.Release(f);
        IObjectPool<Flyweight> GetPoolFor(FlyweightSettings settings) {
            if (_pools.TryGetValue(settings.type, out var pool)) return pool;

            pool = new ObjectPool<Flyweight>(
                settings.Create,
                settings.OnGet,
                settings.OnRelease,
                settings.OnDestroyPoolObject,
                collectionCheck,
                defaultCapacity,
                maxPoolSize
            );
            _pools.Add(settings.type, pool);
            return pool;
        }
    }
}
