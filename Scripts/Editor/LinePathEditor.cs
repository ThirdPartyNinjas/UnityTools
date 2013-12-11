using System.Collections;

using UnityEngine;
using UnityEditor;

namespace ThirdPartyNinjas.UnityTools
{
    [CustomEditor(typeof(LinePath))]
    public class LinePathEditor : Editor
    {
        void OnSceneGUI()
        {
            LinePath linePath = (LinePath)target;

            Event e = Event.current;
            if(e.type == EventType.MouseDown && e.shift)
            {
                Undo.RecordObject(linePath, "Add Point to Path");
                Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                linePath.Points.Add(new Vector2(ray.origin.x - linePath.transform.position.x, ray.origin.y - linePath.transform.position.y));
                linePath.Points.Sort((a, b) => {
                    return a.x.CompareTo(b.x); });
            }

            Vector3[] pointsArray = new Vector3[linePath.Points.Count];
            for(int i=0; i<linePath.Points.Count; i++)
            {
                pointsArray[i] = new Vector3(linePath.transform.position.x + linePath.Points[i].x,
                                         linePath.transform.position.y + linePath.Points[i].y);
            }

            Handles.color = Color.red;
            Handles.DrawPolyLine(pointsArray);

            Handles.color = Color.white;
            for(int i=0; i<linePath.Points.Count; i++)
            {
                Vector3 before = new Vector3(linePath.Points[i].x, linePath.Points[i].y);
                Vector3 after = Handles.Slider2D(i + 1, before, linePath.transform.position, Vector3.up, Vector3.up, Vector3.left, 1.0f, Handles.DotCap, Vector2.one * 0.1f);
                if(before != after)
                {
                    Undo.RecordObject(linePath, "Move Point");
                    linePath.Points[i] = new Vector3(after.x, after.y, 0);
                }
            }
        }
    
        public override void OnInspectorGUI()
        {
            LinePath linePath = (LinePath)target;

            if(linePath.Points.Count > 0)
            {
                int remove = -1;
                for(int i=0; i<linePath.Points.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    Vector2 localPositionBefore = linePath.Points[i];
                    Vector2 worldPosition = localPositionBefore + new Vector2(linePath.transform.position.x, linePath.transform.position.y);
                    worldPosition = EditorGUILayout.Vector2Field("Point " + (i + 1), worldPosition);
                    Vector2 localPositionAfter = worldPosition - new Vector2(linePath.transform.position.x, linePath.transform.position.y);
                    if(localPositionBefore != localPositionAfter)
                    {
                        Undo.RecordObject(linePath, "Move Point");
                        linePath.Points[i] = localPositionAfter;
                    }
                    if(GUILayout.Button("Delete"))
                        remove = i;
                    EditorGUILayout.EndHorizontal();
                }

                if(remove != -1)
                {
                    Undo.RecordObject(linePath, "Delete Point from Path");
                    linePath.Points.RemoveAt(remove);
                }
            }
            else
            {
                GUILayout.Label("Shift-Click to add points");
            }

            SceneView.RepaintAll();
        }
    }
}