using System;
using System.Collections;
using System.Collections.Generic;
using KBCore.Refs;
using Utils.EventBus;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils.Singletons;

namespace Utils.MultiSceneSystem
{
    public class LevelManager : ValidatedMonoBehaviour
    {
        public Transform player;            
        public float loadDistance = 50f;    
        public float unloadDistance = 60f;
        public ZoneData[] zones;
        
        private readonly HashSet<int> _loadedScenes = new HashSet<int>();
        
        void Start()
        {
            StartCoroutine(CheckZones());
            _loadedScenes.Add(SceneManager.GetActiveScene().buildIndex);
        }

        private IEnumerator CheckZones()
        {
            while (true)
            {
                Vector3 playerPosition = player.position;

                foreach (var zone in zones)
                {
                    float distance = Vector3.Distance(playerPosition, zone.zonePosition);

                    // Load zone if within loadDistance and not loaded
                    if (distance <= loadDistance && !_loadedScenes.Contains(zone.sceneBuildIndex))
                    {
                        StartCoroutine(LoadZone(zone.sceneBuildIndex));
                    }
                    // Unload zone if out of unloadDistance and is currently loaded
                    else if (distance > unloadDistance && _loadedScenes.Contains(zone.sceneBuildIndex))
                    {
                        StartCoroutine(UnloadZone(zone.sceneBuildIndex));
                    }
                }

                yield return new WaitForSeconds(1f);  // Check every 1 second
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



    public enum Levels
    {
        None,
        FirstLevel
    }
    
    
}
