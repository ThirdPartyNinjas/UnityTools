using System.Collections.Generic;

using UnityEngine;

namespace ThirdPartyNinjas
{
    public class PrefabPool : MonoBehaviourSingleton<PrefabPool>
    {
        public override bool PersistOnSceneChange { get { return false; } }

        public PrefabType TakeObject<PrefabType>(GameObject prefab, Transform parent = null) where PrefabType : PrefabMonoBehaviour
        {
            List<MonoBehaviour> existingObjects;

            if (!prefabLists.TryGetValue(prefab, out existingObjects))
            {
                existingObjects = new List<MonoBehaviour>();
                prefabLists.Add(prefab, existingObjects);
            }

            PrefabType fabricated;

            if (existingObjects.Count > 0)
            {
                fabricated = (PrefabType)existingObjects[existingObjects.Count - 1];
                existingObjects.RemoveAt(existingObjects.Count - 1);
                fabricated.gameObject.SetActive(true);
            }
            else
            {
                var gameObject = (GameObject)GameObject.Instantiate(prefab);
                fabricated = gameObject.GetComponent<PrefabType>();
                fabricated.Prefab = prefab;
            }

            fabricated.transform.SetParent((parent == null) ? transform : parent);

            return fabricated;
        }

        public void ReturnObject<PrefabType>(PrefabType fabricated) where PrefabType : PrefabMonoBehaviour
        {
            List<MonoBehaviour> existingObjects;

            if (!prefabLists.TryGetValue(fabricated.Prefab, out existingObjects))
            {
                existingObjects = new List<MonoBehaviour>();
                prefabLists.Add(fabricated.Prefab, existingObjects);
            }

            fabricated.transform.SetParent(transform);
            fabricated.gameObject.SetActive(false);

            existingObjects.Add(fabricated);
        }

        public override void Awake()
        {
            base.Awake();

            prefabLists = new Dictionary<GameObject, List<MonoBehaviour>>();
        }

        private Dictionary<GameObject, List<MonoBehaviour>> prefabLists;
    }
}