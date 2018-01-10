using UnityEngine;

namespace ThirdPartyNinjas
{
    public class PoolDemoShip : PrefabMonoBehaviour
    {
        private Vector3 speed = Vector3.zero;

        public void Reset()
        {
            transform.position = Vector3.zero;
            speed.x = Random.Range(-10.0f, 10.0f);
            speed.y = Random.Range(-10.0f, 10.0f);
            speed.z = 0.0f;
        }

        private void Update()
        {
            transform.position += speed * Time.deltaTime;

            if (Mathf.Abs(transform.position.x) > 10.0f || Mathf.Abs(transform.position.y) > 10.0f)
            {
                PrefabPool.Instance.ReturnObject(this);
                return;
            }
        }
    }
}