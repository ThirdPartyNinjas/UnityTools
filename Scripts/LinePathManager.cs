using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace ThirdPartyNinjas.UnityTools
{
    public class LinePathManager : MonoBehaviour, IEnumerable<LinePath>
    {
        public static LinePathManager Instance { get { return instance; } }

        void Awake()
        {
            instance = this;
            linePaths = new List<LinePath>();
        }

        void OnDestroy()
        {
            linePaths.Clear();
            linePaths = null;
            instance = null;
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

        private static LinePathManager instance = null;
        private List<LinePath> linePaths;
    }
}
