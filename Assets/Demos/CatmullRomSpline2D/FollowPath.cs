using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThirdPartyNinjas
{
    public class FollowPath : MonoBehaviour
    {
        public List<CatmullRomSpline2D> paths;
        public float speed;
        public bool turnToPath;

        void Update()
        {
            // move along the path until we reach the end, then start the next path
            float moveDistance = Time.deltaTime * speed;
            distanceAlongPath += moveDistance;
            if (distanceAlongPath >= paths[currentPath].Length)
            {
                currentPath = (currentPath + 1) % paths.Count;
                distanceAlongPath = 0.0f;
            }
            else
            {
                int segment;
                float s;
                paths[currentPath].GetPosition(distanceAlongPath, out segment, out s);

                var position = CatmullRomSpline2D.CatmullRom(paths[currentPath].points[segment],
                    paths[currentPath].points[segment + 1],
                    paths[currentPath].points[segment + 2],
                    paths[currentPath].points[segment + 3], s);
                transform.position = position;

                if (turnToPath)
                {
                    var direction = CatmullRomSpline2D.CatmullRomDerivative(paths[currentPath].points[segment],
                        paths[currentPath].points[segment + 1],
                        paths[currentPath].points[segment + 2],
                        paths[currentPath].points[segment + 3], s);
                    transform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.down, direction));
                }
            }
        }

        private int currentPath = 0;
        private float distanceAlongPath = 0.0f;
    }
}
