using UnityEditor;
using UnityEngine;

namespace ThirdPartyNinjas
{
    public class GameObjectAlignment
    {
        [MenuItem("Tools/GameObject Alignment/Horizontal Spread")]
        private static void HorizontalSpread()
        {
            var selectedObjects = Selection.gameObjects;

            float min = float.MaxValue;
            float max = float.MinValue;

            foreach (var go in selectedObjects)
            {
                min = Mathf.Min(min, go.transform.position.x);
                max = Mathf.Max(max, go.transform.position.x);
            }

            float distance = (max - min) / (selectedObjects.Length - 1);

            for (int i = 0; i < selectedObjects.Length; i++)
            {
                var position = selectedObjects[i].transform.position;
                position.x = min + i * distance;
                selectedObjects[i].transform.position = position;
            }
        }

        public class CircularSpreadWizard : ScriptableWizard
        {
            private void OnWizardCreate()
            {
            }

            protected override bool DrawWizardGUI()
            {
                GUILayout.Label("Select the center object, and the distance\nto use, then check Apply Distance");
                EditorGUILayout.Space();

                if (gameObjects == null)
                    gameObjects = (GameObject[])Selection.gameObjects.Clone();

                var applyStart = apply;
                var distanceStart = distance;
                var selectedCenterObjectStart = selectedCenterObject;

                for (int i = 0; i < gameObjects.Length; i++)
                {
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Toggle(selectedCenterObject == i, ""))
                        selectedCenterObject = i;
                    EditorGUILayout.ObjectField(gameObjects[i], typeof(GameObject), true);
                    GUILayout.EndHorizontal();
                }

                GUILayout.BeginHorizontal();
                distance = GUILayout.HorizontalSlider(distance, 0, 5);

                if (distance != distanceStart)
                    distanceText = distance.ToString();

                distanceText = GUILayout.TextField(distanceText, GUILayout.Width(80));

                float tempDistance;
                if (float.TryParse(distanceText, out tempDistance))
                {
                    distance = tempDistance;
                }
                GUILayout.EndHorizontal();

                apply = GUILayout.Toggle(apply, "Apply Distance");

                return (apply != applyStart) || (distance != distanceStart) || (selectedCenterObject != selectedCenterObjectStart);
            }

            private void OnWizardUpdate()
            {
                if (apply)
                {
                    if (gameObjects[selectedCenterObject] == null)
                        return;

                    int objectCount = 0;
                    for (int i = 0; i < gameObjects.Length; i++)
                    {
                        if (i != selectedCenterObject && gameObjects[i] != null)
                            objectCount++;
                    }

                    float angleDistance = 2 * Mathf.PI / objectCount;
                    float currentAngle = 0.0f;
                    var centerPosition = gameObjects[selectedCenterObject].transform.position;

                    for (int i = 0; i < gameObjects.Length; i++)
                    {
                        if (i == selectedCenterObject)
                            continue;

                        var direction = new Vector3(Mathf.Cos(currentAngle), Mathf.Sin(currentAngle), 0);
                        var position = gameObjects[i].transform.position;
                        position = centerPosition + direction * distance;
                        gameObjects[i].transform.position = position;

                        currentAngle += angleDistance;
                    }
                }
            }

            private int selectedCenterObject = 0;
            private bool apply = false;
            private float distance = 0.0f;
            private string distanceText = "0.0";
            private GameObject[] gameObjects;
        }

        [MenuItem("Tools/GameObject Alignment/Circular Spread")]
        private static void ShowCircularSpreadWizard()
        {
            ScriptableWizard.DisplayWizard<CircularSpreadWizard>("Circular Spread", "Close");
        }
    }
}