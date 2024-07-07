using System;
using UnityEngine.SceneManagement;
using Utils.Singletons;

namespace Utils.LoadingScreen
{
    public class LoadingManager : PersistentSingleton<LoadingManager>
    {
        public string oldScene;
        public string newScene;
        public string loadingScreenName = "LoadingScene";

        public void LoadNewScene(string newSceneName)
        {
            oldScene = SceneManager.GetActiveScene().name;
            newScene = newSceneName;
            
            // Load the Loading Screen is Additive mode, to unload the current scene in the Loading Screen itself
            SceneManager.LoadScene(loadingScreenName, LoadSceneMode.Additive);
        }
    }
}
