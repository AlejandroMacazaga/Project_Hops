using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;
using Utils.Singletons;

namespace Utils.SoundSystem
{
    public class SoundManager : PersistentSingleton<SoundManager>
    {
        IObjectPool<SoundEmitter> _pool;
        [SerializeField] private List<SoundEmitter> activeSounds = new();
        public readonly Queue<SoundEmitter> FrequentSounds = new();
        
        [SerializeField] private SoundEmitter soundEmitterPrefab;
        [SerializeField] private bool collectionCheck = true;
        [SerializeField] private int defaultCapacity = 10;
        [SerializeField] private int maxPoolSize = 100;
        [SerializeField] private int maxActiveSounds = 30;
        
        void Start()
        {
            InitializePool();
        }

        public SoundBuilder CreateSound() => new SoundBuilder(this);
        
        public bool CanPlaySound(SoundData soundData)
        {
            // return !Counts.TryGetValue(soundData, out var count) || count < maxActiveSounds;

            if (!soundData.frequentSound) return true;

            if (FrequentSounds.Count >= maxActiveSounds && FrequentSounds.TryDequeue(out var soundEmitter))
            {
                try
                {
                    soundEmitter.Stop();
                    return true;
                }
                catch
                {
                    Debug.Log("SoundEmitter is already released");
                }

                return false;
            }
            return true;
        }
        
        public SoundEmitter Get()
        {
            return _pool.Get();
        }
        
        public void ReturnToPool(SoundEmitter soundEmitter)
        {
            _pool.Release(soundEmitter);
        }
        
        void InitializePool()
        {
            _pool = new ObjectPool<SoundEmitter>(
                CreateSoundEmitter,
                OnTakeFromPool, 
                OnReturnedToPool,
                OnDestroyPoolObject,
                collectionCheck,
                defaultCapacity,
                maxPoolSize
            );
        }
        

        private void OnTakeFromPool(SoundEmitter soundEmitter)
        {
            soundEmitter.gameObject.SetActive(true);
            activeSounds.Add(soundEmitter);
        }
        
        private void OnReturnedToPool(SoundEmitter soundEmitter)
        {
            soundEmitter.gameObject.SetActive(false);
            activeSounds.Remove(soundEmitter);
        }
        
        private void OnDestroyPoolObject(SoundEmitter soundEmitter)
        {
            Destroy(soundEmitter.gameObject);
        }

        private SoundEmitter CreateSoundEmitter()
        {
            var soundEmitter = Instantiate(soundEmitterPrefab);
            soundEmitter.gameObject.SetActive(false);
            return soundEmitter;
        }
    }
}