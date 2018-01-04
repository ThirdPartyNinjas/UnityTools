using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ThirdPartyNinjas
{
    // Note: This editor is fairly crude and will recalculate a lot of spline subsegments every time
    // you move a point handle. It could be optimized, but only does this at design time in the editor
    // so it isn't super important.
    public class CatmullRomSpline2DEditor : EditorWindow
    {
        [MenuItem("Window/Catmull-Rom Spline 2D Editor")]
        public static void ShowWindow()
        {
            GetWindow<CatmullRomSpline2DEditor>("Catmull-Rom Spline 2D Editor");
        }

        void OnEnable()
        {
            SceneView.onSceneGUIDelegate += this.OnSceneGUI;
        }

        void OnDisable()
        {
            SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
        }

        void OnGUI()
        {
            if(crs != lastCrs)
            {
                lastCrs = crs;
                HandleSplineChanges();
            }

            bool repaint = false;

            CatmullRomSpline2D before = crs;
            crs = (CatmullRomSpline2D)EditorGUILayout.ObjectField("Spline:", crs, typeof(CatmullRomSpline2D), false);
            if (before != crs)
            {
                repaint = true;
            }

            if (crs != null)
            {
                int subsegmentStepsBefore = crs.subsegmentSteps;
                int subsegmentStepsAfter = EditorGUILayout.IntField("Subsegment Steps:", subsegmentStepsBefore);
                if (subsegmentStepsBefore != subsegmentStepsAfter)
                {
                    repaint = true;
                }

                if (crs.points.Count > 0)
                {
                    int remove = -1;
                    for (int i = 0; i < crs.points.Count; i++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        Vector2 positionBefore = crs.points[i];
                        Vector2 positionAfter = EditorGUILayout.Vector2Field("Point " + (i + 1), positionBefore);
                        if (positionBefore != positionAfter)
                        {
                            crs.points[i] = positionAfter;
                            repaint = true;
                        }
                        if (GUILayout.Button("Delete"))
                        {
                            remove = i;
                        }
                        EditorGUILayout.EndHorizontal();
                    }

                    if (remove != -1)
                    {
                        crs.points.RemoveAt(remove);
                        repaint = true;
                    }
                }
                else
                {
                    GUILayout.Label("Shift-Click in the scene to add points");
                }
            }

            if (repaint)
            {
                SceneView.RepaintAll();
                HandleSplineChanges();
            }
        }

        void OnSceneGUI(SceneView sceneView)
        {
            if (crs != lastCrs)
            {
                lastCrs = crs;
                HandleSplineChanges();
            }

            bool repaint = false;

            if (crs != null)
            {
                if (crs.points.Count >= 4)
                {
                    Vector3[] pointsArray = new Vector3[subsegments.Count];
                    for (int i = 0; i < subsegments.Count; i++)
                    {
                        pointsArray[i] = new Vector3(subsegments[i].x, subsegments[i].y, 0);
                    }

                    Handles.color = Color.red;
                    Handles.DrawPolyLine(pointsArray);
                }

                Event e = Event.current;
                if (e.type == EventType.MouseDown && e.shift)
                {
                    Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                    crs.points.Add(new Vector2(ray.origin.x, ray.origin.y));
                    repaint = true;
                }

                float handleSize = 1.0f;
                Camera camera = Camera.current;
                if (camera != null)
                {
                    handleSize = Mathf.Abs(camera.transform.position.z) / 100.0f;
                }

                for (int i = 0; i < crs.points.Count; i++)
                {
                    if (i == 0)
                        Handles.color = Color.green;
                    else if (i == crs.points.Count - 1)
                        Handles.color = Color.red;
                    else
                        Handles.color = Color.white;

                    Vector3 before = new Vector3(crs.points[i].x, crs.points[i].y, 0);
                    Vector3 after = Handles.FreeMoveHandle(before, Quaternion.identity, handleSize, Vector3.one * 0.1f, Handles.DotHandleCap);

                    if (before != after)
                    {
                        crs.points[i] = new Vector2(after.x, after.y);
                        repaint = true;
                    }
                }
            }

            if (repaint)
            {
                Repaint();
                HandleSplineChanges();
            }
        }

        void HandleSplineChanges()
        {
            crs.CalculateDistances();
            CalculateSubsegments();
            EditorUtility.SetDirty(crs);
        }

        public void CalculateSubsegments()
        {
            if (crs.points.Count >= 4)
            {
                subsegments = new List<Vector2>();

                var position = CatmullRomSpline2D.CatmullRom(crs.points[0], crs.points[1], crs.points[2], crs.points[3], 0);

                subsegments.Add(position);

                for (int i = 0; i < crs.points.Count - 3; i++)
                {
                    for (int segment = 1; segment <= crs.subsegmentSteps; segment++)
                    {
                        float s = segment / (float)crs.subsegmentSteps;

                        position = CatmullRomSpline2D.CatmullRom(crs.points[i + 0], crs.points[i + 1], crs.points[i + 2], crs.points[i + 3], s);
                        subsegments.Add(position);
                    }
                }
            }
        }

        CatmullRomSpline2D crs;
        CatmullRomSpline2D lastCrs;
        List<Vector2> subsegments = new List<Vector2>();
    }
}
