using System;
using UnityEngine.SceneManagement;
using Utils.Singletons;

namespace Utils.LoadingScreen
{
    public class LoadingManager : PersistentSingleton<LoadingManager>
    {
        public String oldScene;
        public String newScene;
        public String loadingScreenName = "LoadingScene";

        public void LoadNewScene(String newSceneName)
        {
            oldScene = SceneManager.GetActiveScene().name;
            newScene = newSceneName;
            
            // Load the Loading Screen is Additive mode, to unload the current scene in the Loading Screen itself
            SceneManager.LoadScene(loadingScreenName, LoadSceneMode.Additive);
        }
    }
}
