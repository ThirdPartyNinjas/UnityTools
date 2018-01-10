using System.Collections.Generic;
using UnityEngine;

namespace ThirdPartyNinjas
{
    [CreateAssetMenu(menuName = "ScriptableObjects/CatmullRomSpline2D")]
    public class CatmullRomSpline2D : ScriptableObject
    {
        public int subsegmentSteps = 10;
        public List<Vector2> points = new List<Vector2>();
        public List<float> subsegmentDistances = new List<float>();
        public float length = 0.0f;

        // Given the distance along the spline, return the current segment and the percentage through that segment
        public void GetSegment(float distance, out int segment, out float s)
        {
            if (distance <= 0)
            {
                segment = 0;
                s = 0.0f;
                return;
            }
            else if (distance >= length)
            {
                segment = points.Count - 4;
                s = 1.0f;
                return;
            }

            int sub = 0;
            float subS = 0.0f;

            for (int i = 0; i < subsegmentDistances.Count - 1; i++)
            {
                distance -= subsegmentDistances[i];
                if (distance < subsegmentDistances[i + 1])
                {
                    sub = i;
                    subS = distance / subsegmentDistances[i + 1];
                    break;
                }
            }

            segment = sub / subsegmentSteps;
            s = (sub % subsegmentSteps + subS) / (float)subsegmentSteps;
            return;
        }

        public Vector2 GetPosition(int segment, float s)
        {
            return CatmullRom(points[segment],
                points[segment + 1],
                points[segment + 2],
                points[segment + 3], s);
        }

        public Vector2 GetDirection(int segment, float s)
        {
            return CatmullRomDerivative(points[segment],
                points[segment + 1],
                points[segment + 2],
                points[segment + 3], s);
        }

        public void CalculateDistances()
        {
            // todo: move subsegments to the editor only, we don't need them here

            if (points.Count >= 4)
            {
                subsegmentDistances = new List<float>();

                var position = CatmullRom(points[0], points[1], points[2], points[3], 0);

                subsegmentDistances.Add(0.0f);

                var lastPosition = position;
                float totalDistance = 0.0f;

                for (int i = 0; i < points.Count - 3; i++)
                {
                    for (int segment = 1; segment <= subsegmentSteps; segment++)
                    {
                        float s = segment / (float)subsegmentSteps;

                        position = CatmullRom(points[i + 0], points[i + 1], points[i + 2], points[i + 3], s);
                        var distance = Vector2.Distance(position, lastPosition);

                        subsegmentDistances.Add(distance);

                        totalDistance += distance;
                        lastPosition = position;
                    }
                }

                length = totalDistance;
            }
        }

        public static Vector2 CatmullRom(Vector2 v0, Vector2 v1, Vector2 v2, Vector2 v3, float s)
        {
            // todo: optimize by moving the 0.5f down to the final calculation
            Vector2 a = 0.5f * (-v0 + 3.0f * v1 - 3.0f * v2 + v3);
            Vector2 b = 0.5f * (2.0f * v0 - 5.0f * v1 + 4.0f * v2 - v3);
            Vector2 c = 0.5f * (v2 - v0);
            Vector2 e = 0.5f * (2.0f * v1);

            // p(s) = a*s3 + b*s2 + c*s + e
            return a * s * s * s + b * s * s + c * s + e;
        }

        public static Vector2 CatmullRomDerivative(Vector2 v0, Vector2 v1, Vector2 v2, Vector2 v3, float s)
        {
            // todo: optimize by moving the 0.5f down to the final calculation
            Vector2 a = 0.5f * (-v0 + 3.0f * v1 - 3.0f * v2 + v3);
            Vector2 b = 0.5f * (2.0f * v0 - 5.0f * v1 + 4.0f * v2 - v3);
            Vector2 c = 0.5f * (v2 - v0);

            // dp/ds = 3a*s2 + 2b*s + c
            return 3.0f * a * s * s + 2.0f * b * s + c;
        }
    }
}