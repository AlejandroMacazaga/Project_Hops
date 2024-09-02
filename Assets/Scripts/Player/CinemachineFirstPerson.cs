using Cinemachine;
using Input;
using KBCore.Refs;
using UnityEngine;
using Utils.Timers;

namespace Player
{
    public class CinemachineFirstPerson : ValidatedMonoBehaviour
    {
        public CinemachineVirtualCamera virtualCamera;
        [SerializeField] private InputReader inputs;
        [SerializeField] private float mouseSensitivity;
        private float _xRotation, _yRotation;
        [SerializeField] private Transform bodyOrientation;
        [SerializeField, Self] private CharacterMover mover;
        public bool IsBodyLocked
        {
            get => mover.isBodyLocked;
            set
            {
                mover.isBodyLocked = value;
                if (mover.isBodyLocked) return;
                bodyOrientation.Rotate(Vector3.up * virtualCamera.transform.localEulerAngles.y);
                virtualCamera.transform.localRotation = Quaternion.Euler(virtualCamera.transform.localRotation.x, 0f, 0f);
            } 
        }

        private CountdownTimer _timer;

        private void OnEnable()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            mover.isBodyLocked = false;
            inputs.EnablePlayerActions();
        }
        
        

        private void LateUpdate()
        {
            HandleCamera();
        }
        
        private void HandleCamera()
        {
            
            var movement = inputs.CameraLook;
            
            var mouseX = movement.x * Time.deltaTime * mouseSensitivity;
            var mouseY = movement.y * Time.deltaTime * mouseSensitivity;
            
            _xRotation -= mouseY;
            _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);
            
            if (mover.isBodyLocked)
            {
                // Apply the rotation to the camera's local rotation
                _yRotation += mouseX;
                virtualCamera.transform.localRotation = Quaternion.Euler(_xRotation, _yRotation, 0f);
            }
            else
            {
                _yRotation = 0f;
                virtualCamera.transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
                bodyOrientation.Rotate(Vector3.up * mouseX);
            }
        }

    }
}
