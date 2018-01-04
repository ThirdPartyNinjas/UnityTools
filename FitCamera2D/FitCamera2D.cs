using System.Collections;

using UnityEngine;

namespace ThirdPartyNinjas
{
    public class FitCamera2D : MonoBehaviour
    {
        public enum FitType
        {
            SafeFit = 0,
            FitHeight,
            FitWidth
        }

        [Tooltip("The type of fitting to use.\n\n" +
            "FitHeight - Scale to match the original height.\n" +
            "FitWidth - Scale to match the original width.\n" +
            "SafeFit - Scale to ensure that all of the original play area is visible.")]
        public FitType fitType = FitType.SafeFit;

        [Tooltip("Match this value to the Pixels Per Unit in your sprite import settings.")]
        public int pixelsPerUnit = 100;

        [Tooltip("The target play width, in pixels.")]
        public int playAreaWidth = 1080;

        [Tooltip("The target play height, in pixels.")]
        public int playAreaHeight = 1920;

        [Tooltip("How often the camera size is checked, in seconds. (Cannot be edited at runtime.)")]
        public float updateInterval = 0.1f;

        public virtual void Start()
        {
            attachedCamera = null;
            cameraWidth = cameraHeight = 0;

            StartCoroutine(CheckCameraSizeCoroutine());
        }

        private IEnumerator CheckCameraSizeCoroutine()
        {
            var wfs = new WaitForSeconds(updateInterval);

            do
            {
                if (attachedCamera == null)
                {
                    attachedCamera = GetComponent<Camera>();
                }

                if (cameraWidth != attachedCamera.pixelWidth || cameraHeight != attachedCamera.pixelHeight)
                {
                    UpdateSize();
                }

                yield return wfs;
            } while (true);
        }

        public void UpdateSize()
        {
            // Note: This function can be called from external code (such as the FitCamera Inspector)
            //  That's why the camera is tested here, despite being set in the coroutine above.
            // Note: For some reason the null-coalescing operator (??) isn't working here.
            //  attachedCamera appears as "null" (with quotes, is it a string?!) in the debugger.
            Camera camera = (attachedCamera != null) ? attachedCamera : GetComponent<Camera>();

            cameraWidth = camera.pixelWidth;
            cameraHeight = camera.pixelHeight;

            float playAreaAspect = playAreaWidth / (float)playAreaHeight;
            float cameraAspect = cameraWidth / (float)cameraHeight;
            float scaleFactor = playAreaAspect / cameraAspect;

            switch (fitType)
            {
                case FitType.SafeFit:
                    if (cameraAspect > playAreaAspect)
                    {
                        camera.orthographicSize = playAreaHeight / (float)pixelsPerUnit / 2.0f;
                    }
                    else
                    {
                        camera.orthographicSize = scaleFactor * playAreaHeight / pixelsPerUnit / 2.0f;
                    }
                    break;

                case FitType.FitWidth:
                    camera.orthographicSize = scaleFactor * playAreaHeight / pixelsPerUnit / 2.0f;
                    break;

                case FitType.FitHeight:
                    camera.orthographicSize = playAreaHeight / (float)pixelsPerUnit / 2.0f;
                    break;
            }
        }

        private Camera attachedCamera = null;

        private int cameraWidth = 0;
        private int cameraHeight = 0;
    }
}