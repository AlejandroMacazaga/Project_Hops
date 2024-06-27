using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils.Singletons;

namespace Utils.LoadingScreen
{
    public class LoadingManager : PersistentSingleton<LoadingManager>
    {
        public String currentScene;
        public String newScene;
        public String loadingScreenName = "LoadingScene";

        public void LoadScene(String newSceneName)
        {
            currentScene = SceneManager.GetActiveScene().name;
            newScene = newSceneName;
            LoadScene(loadingScreenName);
        }
    }
}
