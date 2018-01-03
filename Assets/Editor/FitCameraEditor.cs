using UnityEditor;
using UnityEngine;

namespace ThirdPartyNinjas
{
    [CustomEditor(typeof(FitCamera))]
    public class FitCameraEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            if (GUILayout.Button("Update camera size"))
            {
                FitCamera fitCamera = (FitCamera)target;
                fitCamera.UpdateSize();
            }
        }
    }
}