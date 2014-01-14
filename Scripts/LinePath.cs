using System.Collections.Generic;

using UnityEngine;

namespace ThirdPartyNinjas.UnityTools
{
    public class LinePath : MonoBehaviour
    {
        public bool AllowOffset = true;

        public List<Vector2> Points = new List<Vector2>();

        public float MinimumX { get { return GetWorldPoint(0).x; } }
        public float MaximumX { get { return GetWorldPoint(Points.Count - 1).x; } }

        public Vector2 GetWorldPoint(int index)
        {
            return Points[index] + new Vector2(transform.position.x, transform.position.y);
        }

        public float Angle(float x)
        {
            for(int i = 0; i < Points.Count - 1; i++)
            {
                if(x < GetWorldPoint(i + 1).x)
                {
                    return (float)System.Math.Atan2(GetWorldPoint(i + 1).y - GetWorldPoint(i).y, GetWorldPoint(i + 1).x - GetWorldPoint(i).x);
                }
            }
            return (float)System.Math.Atan2(GetWorldPoint(Points.Count - 1).y - GetWorldPoint(Points.Count - 2).y, GetWorldPoint(Points.Count - 1).x - GetWorldPoint(Points.Count - 2).x);
        }

        public void OffsetPosition(Vector3 offset)
        {
            if(AllowOffset)
                transform.position += offset;
        }

        public float InterpolateY(float x)
        {
            for(int i = 0; i < Points.Count - 1; i++)
            {
                if(x < GetWorldPoint(i + 1).x)
                {
                    return InterpolateY(x, GetWorldPoint(i), GetWorldPoint(i + 1));
                }
            }
            return InterpolateY(x, GetWorldPoint(Points.Count - 2), GetWorldPoint(Points.Count - 1));
        }

        public static float InterpolateY(float x, Vector2 p0, Vector2 p1)
        {
            return (x - p0.x) * ((p1.y - p0.y) / (p1.x - p0.x)) + p0.y;
        }

        void OnEnable()
        {
            LinePathManager lpm = LinePathManager.Instance;

            if(lpm != null)
                lpm.Add(this);
        }

        void OnDisable()
        {
            LinePathManager lpm = LinePathManager.Instance;
            
            if(lpm != null)
                lpm.Remove(this);
        }
    }
}