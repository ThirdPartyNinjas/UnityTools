using System.Collections;
using UnityEngine;

namespace ThirdPartyNinjas
{
    public class PoolDemo : MonoBehaviour
    {
        public GameObject shipPrefab;
        public float spawnDelay = 0.5f;

        private void Start()
        {
            StartCoroutine(SpawnerCoroutine());
        }

        private void Update()
        {
        }

        private IEnumerator SpawnerCoroutine()
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