using KBCore.Refs;
using UnityEngine;
using UnityUtils;

namespace Player.KinematicController {
    public class TurnTowardController : ValidatedMonoBehaviour {
        [SerializeField] KinematicPlayerController controller;
        public float turnSpeed = 50f;
        
        [SerializeField, HideInInspector, Self] Transform tr;
        float _currentYRotation;
        const float FallOffAngle = 90f;

        void Start() {
            tr = transform;
            
            _currentYRotation = tr.localEulerAngles.y;
        }

        void LateUpdate() {
            Vector3 velocity = Vector3.ProjectOnPlane(controller.GetMovementVelocity(), tr.parent.up);
            if (velocity.magnitude < 0.001f) return;
            
            float angleDifference = VectorMath.GetAngle(tr.forward, velocity.normalized, tr.parent.up);
            
            float step = Mathf.Sign(angleDifference) *
                         Mathf.InverseLerp(0f, FallOffAngle, Mathf.Abs(angleDifference)) *
                         Time.deltaTime * turnSpeed;
            
            _currentYRotation += Mathf.Abs(step) > Mathf.Abs(angleDifference) ? angleDifference : step;
            
            tr.localRotation = Quaternion.Euler(0f, _currentYRotation, 0f);
        }
    }
}
