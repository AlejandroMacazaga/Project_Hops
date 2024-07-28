using Input;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player
{
    public class FirstPersonCamera : MonoBehaviour
    {
        [SerializeField] private InputReader playerInputs;
        [SerializeField] private Camera fpCamera;
        [SerializeField] private float senX, senY;
        [SerializeField] private Transform bodyOrientation;

        private float _xRotation, _yRotation;

        void Awake()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            playerInputs.EnablePlayerActions();
        }

        private void Update()
        {
            Vector2 movement = playerInputs.CameraLook;
            var mouseX = movement.x * Time.deltaTime * senX;
            var mouseY = movement.y * Time.deltaTime * senY;

            Debug.Log(mouseX + " mouse X");
            Debug.Log(mouseY + " mouse Y");
            _yRotation += mouseX;
            _xRotation -= mouseY;
            _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

            transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
            bodyOrientation.Rotate(Vector3.up * mouseX);
        }

    }
}