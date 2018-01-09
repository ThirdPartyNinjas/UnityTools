using System;
using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace ThirdPartyNinjas
{
    public class SceneRoot : MonoBehaviour
    {
        public string Name { get; private set; }

        public GameObject sceneContainer;

        private void Start()
        {
            Name = gameObject.scene.name;

            if (sceneContainer == null)
            {
                Debug.LogError("SceneRoot needs a reference to a scene container GameObject");
            }
        }

        public void StartScene(bool setActiveScene = false)
        {
            sceneContainer.SetActive(true);
            if(setActiveScene)
            {
                SceneManager.SetActiveScene(gameObject.scene);
            }
        }

        public void StopScene()
        {
            sceneContainer.SetActive(false);
        }

        public void UnloadScene()
        {
            SceneManager.UnloadSceneAsync(gameObject.scene);
        }

        public static IEnumerator LoadSceneCoroutine(string sceneName, Action<SceneRoot> callback)
        {
            var asyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            yield return asyncOperation;

            var roots = GameObject.FindObjectsOfType<SceneRoot>();
            foreach (var root in roots)
            {
                var scene = root.gameObject.scene;
                if (scene.name == sceneName)
                {
                    if (callback != null)
                    {
                        callback(root);
                        yield break;
                    }
                }
            }
        }

        public static IEnumerator LoadSceneCoroutine(int sceneIndex, Action<SceneRoot> callback)
        {
            var asyncOperation = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
            yield return asyncOperation;

            var roots = GameObject.FindObjectsOfType<SceneRoot>();
            foreach (var root in roots)
            {
                var scene = root.gameObject.scene;
                if (scene.buildIndex == sceneIndex)
                {
                    if (callback != null)
                    {
                        callback(root);
                        yield break;
                    }
                }
            }
        }
    }
}