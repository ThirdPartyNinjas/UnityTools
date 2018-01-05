using UnityEngine;

namespace ThirdPartyNinjas
{
    public class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public virtual bool PersistOnSceneChange { get { return true; } }

        public static T Instance
        {
            get
            {
                if (instance != null)
                    return instance;
                instance = new GameObject(typeof(T).Name).AddComponent<T>();
                return instance;
            }
        }

        public static T ExistingInstance
        {
            get
            {
                return instance;
            }
        }

        public virtual void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);
                return;
            }

            instance = GetComponent<T>();

            // Only root GameObjects can be set to not destroy on load
            if (PersistOnSceneChange && transform.root == transform)
            {
                DontDestroyOnLoad(this.gameObject);
            }
        }

        public virtual void OnDestroy()
        {
            instance = null;
        }

        private static T instance = null;
    }
}