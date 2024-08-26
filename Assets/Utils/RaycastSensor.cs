using UnityEngine;

namespace Utils {
    public class RaycastSensor {
        public float castLength = 1f;
        public LayerMask layermask = 255;
        
        Vector3 _origin = Vector3.zero;
        readonly Transform _tr;
        
        public enum CastDirection { Forward, Right, Up, Backward, Left, Down }
        CastDirection castDirection;
        
        RaycastHit hitInfo;

        public RaycastSensor(Transform playerTransform) {
            _tr = playerTransform;
        }

        public void Cast() {
            Vector3 worldOrigin = _tr.TransformPoint(_origin);
            Vector3 worldDirection = GetCastDirection();
            
            Physics.Raycast(worldOrigin, worldDirection, out hitInfo, castLength, layermask, QueryTriggerInteraction.Ignore);
        }
        
        public bool HasDetectedHit() => hitInfo.collider != null;
        public float GetDistance() => hitInfo.distance;
        public Vector3 GetNormal() => hitInfo.normal;
        public Vector3 GetPosition() => hitInfo.point;
        public Collider GetCollider() => hitInfo.collider;
        public Transform GetTransform() => hitInfo.transform;
        
        public void SetCastDirection(CastDirection direction) => castDirection = direction;
        public void SetCastOrigin(Vector3 pos) => _origin = _tr.InverseTransformPoint(pos);

        Vector3 GetCastDirection() {
            return castDirection switch {
                CastDirection.Forward => _tr.forward,
                CastDirection.Right => _tr.right,
                CastDirection.Up => _tr.up,
                CastDirection.Backward => -_tr.forward,
                CastDirection.Left => -_tr.right,
                CastDirection.Down => -_tr.up,
                _ => Vector3.one
            };
        }
        
        public void DrawDebug() {
            if (!HasDetectedHit()) return;

            Debug.DrawRay(hitInfo.point, hitInfo.normal, Color.red, Time.deltaTime);
            float markerSize = 0.2f;
            Debug.DrawLine(hitInfo.point + Vector3.up * markerSize, hitInfo.point - Vector3.up * markerSize, Color.green, Time.deltaTime);
            Debug.DrawLine(hitInfo.point + Vector3.right * markerSize, hitInfo.point - Vector3.right * markerSize, Color.green, Time.deltaTime);
            Debug.DrawLine(hitInfo.point + Vector3.forward * markerSize, hitInfo.point - Vector3.forward * markerSize, Color.green, Time.deltaTime);
        }
    }
}