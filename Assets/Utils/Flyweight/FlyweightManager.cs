using System.Collections.Generic;
using UnityEngine.Pool;
using Utils.Singletons;

namespace Utils.Flyweight
{
    public class FlyweightManager : PersistentSingleton<FlyweightManager>
    {
        private readonly Dictionary<string, IObjectPool<Flyweight>> _pools = new();
        
        public Flyweight Spawn(FlyweightSettings s) => Instance.GetOrCreatePool(s).Get();
        public void ReturnToPool(Flyweight f) => Instance.GetOrCreatePool(f.settings)?.Release(f);
        public IObjectPool<Flyweight> GetOrCreatePool(FlyweightSettings settings)
        {
            if (_pools.TryGetValue(settings.group, out var pool)) return pool;

            pool = new ObjectPool<Flyweight>(settings.Create,
                settings.OnGet,
                settings.OnRelease,
                settings.OnDestroyPoolObject,
                settings.collectionCheck,
                settings.defaultCapacity,
                settings.maxPoolSize
                );
            return pool;
        }
    }
}
