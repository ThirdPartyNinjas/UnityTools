using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThirdPartyNinjas
{
    public class PoolDemo : MonoBehaviour
    {
        public GameObject shipPrefab;
        public float spawnDelay = 0.5f;

        void Start()
        {
            StartCoroutine(SpawnerCoroutine());
        }

        void Update()
        {
        }

        IEnumerator SpawnerCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(spawnDelay);

                var ship = PrefabPool.Instance.TakeObject<PoolDemoShip>(shipPrefab);
                ship.Reset();
            }
        }
    }
}