using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils.Singletons;
using Utils.EventBus;

namespace Utils.MultiSceneSystem
{
    public class LevelZonesManager : RegulatorSingleton<LevelZonesManager>
    {
        private readonly HashSet<int> _loadedScenes = new HashSet<int>();

        void Start()
        {
            EventBus<LoadSceneGroupEvent>.Register(new EventBinding<LoadSceneGroupEvent>(LoadAndUnload));
        }

        private void LoadAndUnload(LoadSceneGroupEvent e)
        {
            var scenesToLoad = new HashSet<int>(e.ToLoad);


            foreach (var loadedScene in new HashSet<int>(_loadedScenes).Where(loadedScene => !scenesToLoad.Contains(loadedScene)))
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
        
        
        private IEnumerator LoadZone(int buildIndex)
        {
            if (IsSceneLoaded(buildIndex))
            {
                yield break;  // Prevent loading the scene if it is already loaded
            }

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(buildIndex, LoadSceneMode.Additive);
            while (asyncLoad is { isDone: false })
            {
                yield return null;
            }

            _loadedScenes.Add(buildIndex);
            Debug.Log("Loaded Scene Index: " + buildIndex);
        }

        private IEnumerator UnloadZone(int buildIndex)
        {
            if (!IsSceneLoaded(buildIndex))
            {
                yield break;  // Prevent unloading the scene if it is not loaded
            }

            AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(buildIndex);
            while (asyncUnload is { isDone: false })
            {
                yield return null;
            }

            _loadedScenes.Remove(buildIndex);
            Debug.Log("Unloaded Scene Index: " + buildIndex);
        }

        // Helper function to check if a scene is already loaded
        private bool IsSceneLoaded(int buildIndex)
        {
            Scene scene = SceneManager.GetSceneByBuildIndex(buildIndex);
            return scene.isLoaded;
        }
    }
}