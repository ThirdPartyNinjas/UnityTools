using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace ThirdPartyNinjas
{
    public class SceneSwitch : MonoBehaviour
    {
        public List<string> sceneList;
        public float delayTime = 5.0f;

        void Start()
        {
            StartCoroutine(SwitchCoroutine());
        }

        IEnumerator SwitchCoroutine()
        {
            do
            {
                yield return StartCoroutine(SceneRoot.LoadSceneCoroutine(sceneList[sceneIndex], SceneLoadCallback));
                sceneIndex = (sceneIndex + 1) % sceneList.Count;
                yield return new WaitForSeconds(delayTime);
            } while (true);
        }

        void SceneLoadCallback(SceneRoot sceneRoot)
        {
            if (activeScene != null)
            {
                activeScene.StopScene();
                activeScene.UnloadScene();
            }

            activeScene = sceneRoot;
            activeScene.StartScene(false);
        }

        private int sceneIndex = 0;
        private SceneRoot activeScene = null;
    }
}