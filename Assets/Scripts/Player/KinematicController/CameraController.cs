using Input;
using KBCore.Refs;
using UnityEngine;

namespace Player.KinematicController {
    public class CameraController : ValidatedMonoBehaviour {
        #region Fields
        float _currentXAngle;
        float _currentYAngle;
        
        [Range(0f, 90f)] public float upperVerticalLimit = 35f;
        [Range(0f, 90f)] public float lowerVerticalLimit = 35f;
        
        public float cameraSpeed = 50f;
        public bool smoothCameraRotation;
        [Range(1f, 50f)] public float cameraSmoothingFactor = 25f;
        
        [SerializeField, HideInInspector, Self] Transform tr;
        [SerializeField] InputReader input;
        #endregion
        
        public Vector3 GetUpDirection() => tr.up;
        public Vector3 GetFacingDirection () => tr.forward;

        void Awake() {
            _currentXAngle = tr.localRotation.eulerAngles.x;
            _currentYAngle = tr.localRotation.eulerAngles.y;
        }

        void Update() {
            RotateCamera(input.CameraLook.x, -input.CameraLook.y);
        }

        void RotateCamera(float horizontalInput, float verticalInput) {
            if (smoothCameraRotation) {
                horizontalInput = Mathf.Lerp(0, horizontalInput, Time.deltaTime * cameraSmoothingFactor);
                verticalInput = Mathf.Lerp(0, verticalInput, Time.deltaTime * cameraSmoothingFactor);
            }
            
            _currentXAngle += verticalInput * cameraSpeed * Time.deltaTime;
            _currentYAngle += horizontalInput * cameraSpeed * Time.deltaTime;
            
            _currentXAngle = Mathf.Clamp(_currentXAngle, -upperVerticalLimit, lowerVerticalLimit);
            
            tr.localRotation = Quaternion.Euler(_currentXAngle, _currentYAngle, 0);
        }
    }
}