using Eflatun.SceneReference;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.Serialization;
using Utils.EventBus;

namespace Utils.MultiSceneSystem
{
    [RequireComponent(typeof(BoxCollider))]
    public class LoadTrigger : ValidatedMonoBehaviour
    {
        [SerializeField, Self] private BoxCollider areaTrigger;
        [SerializeField] private SceneGroup scenesToLoad;
        [SerializeField] private LayerMask allowed;
        // Start is called before the first frame update
        void Start()
        {
            areaTrigger.isTrigger = true;
        }

        void OnTriggerEnter(Collider c)
        {
            if (((1 << c.gameObject.layer) & allowed) != 0)
            {
                EventBus<LoadSceneGroupEvent>.Raise(new LoadSceneGroupEvent()
                {
                    toLoad = scenesToLoad.scenes
                        
                });
            }
        }
    }
}
