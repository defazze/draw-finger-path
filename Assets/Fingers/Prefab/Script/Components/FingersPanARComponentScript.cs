using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalRubyShared
{
    /// <summary>
    /// Useful script for using a pan gesture to move an object forward or back along z axis using pan up and down
    /// and left or right using pan left or right
    /// </summary>
    [AddComponentMenu("Fingers Gestures/Component/Pan AR", 5)]
    public class FingersPanARComponentScript : MonoBehaviour
    {
        [Tooltip("The camera to use to convert screen coordinates to world coordinates. Defaults to Camera.main.")]
        public Camera Camera;

        [Range(-100.0f, 100.0f)]
        [Tooltip("The speed at which to move the object forward and backwards.")]
        public float SpeedForwardBack = 16.0f;

        [Range(-100.0f, 100.0f)]
        [Tooltip("The speed at which to move the object left and right")]
        public float SpeedLeftRight = 16.0f;

        [Range(-3.0f, 3.0f)]
        [Tooltip("Orbit speed")]
        public float OrbitSpeed = 0.25f;

        /// <summary>
        /// Allow moving the target
        /// </summary>
        public PanGestureRecognizer PanGesture { get; private set; }

        private Vector3? orbitTarget;
        private float prevMouseX;

        private void PanGestureStateUpdated(GestureRecognizer gesture)
        {
            if (gesture.State == GestureRecognizerState.Executing && orbitTarget == null)
            {
                Vector3 right = Camera.transform.right;
                right.y = 0.0f;
                right = right.normalized;
                Vector3 forward = Camera.transform.forward;
                forward.y = 0.0f;
                forward = forward.normalized;
                gameObject.transform.Translate(right * gesture.DeltaX * Time.deltaTime, Space.World);
                gameObject.transform.Translate(forward * gesture.DeltaY * Time.deltaTime, Space.World);
            }
        }

        private void OnEnable()
        {
            if (Camera == null)
            {
                Camera = Camera.main;
            }
            PanGesture = new PanGestureRecognizer();
            PanGesture.StateUpdated += PanGestureStateUpdated;
            PanGesture.PlatformSpecificView = gameObject;
            FingersScript.Instance.AddGesture(PanGesture);
        }

        private void OnDisable()
        {
            if (FingersScript.HasInstance)
            {
                FingersScript.Instance.RemoveGesture(PanGesture);
            }
        }

        private void Update()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.LinuxPlayer:
                case RuntimePlatform.LinuxEditor:
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.OSXPlayer:
                case RuntimePlatform.OSXEditor:
                    if (UnityEngine.Input.GetMouseButtonDown(1))
                    {
                        orbitTarget = transform.position;
                        prevMouseX = UnityEngine.Input.mousePosition.x;
                    }
                    else if (UnityEngine.Input.GetMouseButtonUp(1))
                    {
                        orbitTarget = null;
                    }
                    if (orbitTarget != null)
                    {
                        Camera.transform.RotateAround(orbitTarget.Value, Vector3.up, (UnityEngine.Input.mousePosition.x - prevMouseX) * Time.deltaTime * OrbitSpeed);
                    }
                    break;
            }
        }
    }
}
