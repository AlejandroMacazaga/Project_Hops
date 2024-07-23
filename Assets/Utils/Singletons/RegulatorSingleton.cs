using UnityEngine;

namespace Utils.Singletons
{
    public class RegulatorSingleton<T> : MonoBehaviour where T : Component
    {
        private static T _instance;

        public static bool HasInstance => _instance != null;

        public float InitializationTime { get; private set; }

        public static T Instance
        {
            get {
                if (_instance) return _instance;
                _instance = FindAnyObjectByType<T>();
                if (_instance) return _instance;
                var obj = new GameObject(typeof(T).Name + " Auto Instance")
                {
                    hideFlags = HideFlags.HideAndDontSave
                };
                _instance = obj.AddComponent<T>();
                return _instance;
            }
        }

        // <summary>
        // Make sure to call base.Awake() in override if you need awake.
        // </summary>
        private void Awake()
        {
            InitializeSingleton();
        }

        private void InitializeSingleton()
        {
            if (!Application.isPlaying) return;
            InitializationTime = Time.time;
            DontDestroyOnLoad(gameObject);
            
            T[] oldInstances = FindObjectsOfType<T>();
            foreach (T old in oldInstances)
            {
                if (old.GetComponent<RegulatorSingleton<T>>().InitializationTime < InitializationTime)
                {
                    Destroy(old.gameObject);
                }
            }

            if (_instance != null) return;
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
    }
}