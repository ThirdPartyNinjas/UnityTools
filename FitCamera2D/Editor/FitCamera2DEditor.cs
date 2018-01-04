using UnityEditor;
using UnityEngine;

namespace ThirdPartyNinjas
{
    [CustomEditor(typeof(FitCamera2D))]
    public class FitCamera2DEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            if (GUILayout.Button("Update camera size"))
            {
                FitCamera2D fitCamera2D = (FitCamera2D)target;
                fitCamera2D.UpdateSize();
            }
        }
    }
}