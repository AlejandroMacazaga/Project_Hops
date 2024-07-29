using Input;
using UnityEngine;
using UnityEngine.Serialization;
using Utils.Timers;

namespace Player
{
    public class FirstPersonCamera : MonoBehaviour
    {
        [SerializeField] private InputReader playerInputs;
        [SerializeField] private Camera fpCamera;
        [SerializeField] private float senX, senY;
        [SerializeField] private Transform bodyOrientation;
        [SerializeField] private bool isLocked;
        private CountdownTimer _timer;
        private float _xRotation, _yRotation;

        void Awake()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            playerInputs.EnablePlayerActions();
        }
        
        

        private void Update()
        {
            if (!isLocked) HandleCamera();
        }

        private void HandleCamera()
        {
            var movement = playerInputs.CameraLook;
            var mouseX = movement.x * Time.deltaTime * senX;
            var mouseY = movement.y * Time.deltaTime * senY;

            _yRotation += mouseX;
            _xRotation -= mouseY;
            _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

            transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
            bodyOrientation.Rotate(Vector3.up * mouseX);
        }

        public void LockCamera(float time, float angle)
        {
            isLocked = true;
            _timer ??= new CountdownTimer(time);
            _timer.Start();
            _timer.OnTimerStop += UnlockCamera;
            transform.localRotation = Quaternion.Euler(angle, 0f, 0f);
            bodyOrientation.Rotate(Vector3.up);
        }

        public void LockCamera(float angle)
        {
            isLocked = true;
            transform.localRotation = Quaternion.Euler(angle, 0f, 0f);
            bodyOrientation.Rotate(Vector3.up);
        }

        private void UnlockCamera()
        {
            isLocked = false;
        }

    }
}