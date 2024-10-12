using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Eflatun.SceneReference;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils.Singletons;
using Utils.EventBus;

namespace Utils.MultiSceneSystem
{
    public class LevelZonesManager : RegulatorSingleton<LevelZonesManager>
    {
        private readonly HashSet<SceneReference> _loadedScenes = new();
        
        void Start()
        {
            EventBus<LoadSceneGroupEvent>.Register(new EventBinding<LoadSceneGroupEvent>(LoadAndUnload));
        }

        private void LoadAndUnload(LoadSceneGroupEvent e)
        {
            var scenesToLoad = e.toLoad.ToHashSet();
            

            foreach (var loadedScene in new HashSet<SceneReference>(_loadedScenes).Where(loadedScene => !scenesToLoad.Contains(loadedScene)))
            {
                // Unload the scene because it's not in the new sceneIds array
                StartCoroutine(UnloadZone(loadedScene));
                _loadedScenes.Remove(loadedScene); // Remove from the loaded scenes set
            }

            foreach (var sceneId in scenesToLoad.Where(sceneId => !_loadedScenes.Contains(sceneId)))
            {
                // Load the scene because it's not already loaded
                StartCoroutine(LoadZone(sceneId));
                _loadedScenes.Add(sceneId); // Add to the loaded scenes set
            }
        }
        
        public void LoadAndUnload(SceneReference[] toLoad)
        {
            var scenesToLoad = new HashSet<SceneReference>(toLoad);


            foreach (var loadedScene in new HashSet<SceneReference>(_loadedScenes).Where(loadedScene => !scenesToLoad.Contains(loadedScene)))
            {
                // Unload the scene because it's not in the new sceneIds array
                StartCoroutine(UnloadZone(loadedScene));
                _loadedScenes.Remove(loadedScene); // Remove from the loaded scenes set
            }
            
            foreach (var sceneId in scenesToLoad.Where(sceneId => !_loadedScenes.Contains(sceneId)))
            {
                // Load the scene because it's not already loaded
                StartCoroutine(LoadZone(sceneId));
                _loadedScenes.Add(sceneId); // Add to the loaded scenes set
            }
        }
        
        
        private IEnumerator LoadZone(SceneReference sceneReference)
        {
            if (IsSceneLoaded(sceneReference.BuildIndex))
            {
                yield break;  // Prevent loading the scene if it is already loaded
            }

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneReference.BuildIndex, LoadSceneMode.Additive);
            while (asyncLoad is { isDone: false })
            {
                yield return null;
            }

            _loadedScenes.Add(sceneReference);
            Debug.Log("Loaded Scene Index: " + sceneReference);
        }

        private IEnumerator UnloadZone(SceneReference sceneReference)
        {
            if (!IsSceneLoaded(sceneReference.BuildIndex))
            {
                yield break;  // Prevent unloading the scene if it is not loaded
            }

            AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(sceneReference.BuildIndex);
            while (asyncUnload is { isDone: false })
            {
                yield return null;
            }

            _loadedScenes.Remove(sceneReference);
            Debug.Log("Unloaded Scene Index: " + sceneReference.BuildIndex);
        }

        // Helper function to check if a scene is already loaded
        private bool IsSceneLoaded(int buildIndex)
        {
            Scene scene = SceneManager.GetSceneByBuildIndex(buildIndex);
            return scene.isLoaded;
        }
    }
}