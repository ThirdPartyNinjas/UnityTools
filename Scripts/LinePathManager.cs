using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace ThirdPartyNinjas.UnityTools
{
    public class LinePathManager : MonoBehaviour, IEnumerable<LinePath>
    {
        public static LinePathManager Instance
        {
            get
            {
                if(instance != null)
                    return instance;
                
                if(preventCreation)
                    return null;
                
                instance = new GameObject("LinePathManager").AddComponent<LinePathManager>();
                GameObject.DontDestroyOnLoad(instance);
                return instance;
            }
        }

        void Awake()
        {
            linePaths = new List<LinePath>();
        }

        public void Add(LinePath linePath)
        {
            linePaths.Add(linePath);
        }

        public void Remove(LinePath linePath)
        {
            linePaths.Remove(linePath);
        }

        public IEnumerator<LinePath> GetEnumerator()
        {
            return linePaths.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void OnApplicationQuit()
        {
            preventCreation = true;
        }
        
        private static bool preventCreation = false;

        private static LinePathManager instance = null;
        private List<LinePath> linePaths;
    }
}
