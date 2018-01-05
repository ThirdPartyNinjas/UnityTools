using UnityEngine;

namespace ThirdPartyNinjas
{
    public abstract class PrefabMonoBehaviour : MonoBehaviour
    {
        [HideInInspector]
        public GameObject Prefab { get; set; }
    }
}