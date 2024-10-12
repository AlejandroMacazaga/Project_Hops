using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Utils.LoadingScreen
{
    public class LoadingSceneController : MonoBehaviour
    {
        public Slider loadingSlider;

        private void Start()
        {
            StartCoroutine(nameof(SceneLoadUnload));
        }

        IEnumerator SceneLoadUnload()
        {
            // First, unload the previous scene
            var sceneToUnload = SceneManager.UnloadSceneAsync(LoadingManager.Instance.oldScene);
            while (!sceneToUnload!.isDone)
            {
                loadingSlider.value = sceneToUnload.progress / 2;
                yield return null;
            }
            
            // Next, load the new scene
            var sceneToLoad = SceneManager.LoadSceneAsync(LoadingManager.Instance.newScene);
            while (!sceneToLoad!.isDone)
            {
                loadingSlider.value = sceneToUnload.progress / 2 + sceneToLoad.progress / 2;
                yield return null;
            }
        }
    }
}
