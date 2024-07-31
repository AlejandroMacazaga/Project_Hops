using UnityEngine;

namespace Utils
{
    public class DontGoThruThings : MonoBehaviour 
    {
        public LayerMask layerMask; //make sure we aren't in this layer 
        public float skinWidth = 0.1f; //probably doesn't need to be changed 
 
        private float _minimumExtent; 
        private float _partialExtent; 
        private float _sqrMinimumExtent; 
        private Vector3 _previousPosition; 
 
 
        //initialize values 
        void Awake() 
        { 
            _previousPosition = transform.position;
            var c = GetComponent<Collider>().bounds.extents;
            _minimumExtent = Mathf.Min(Mathf.Min(c.x, c.y), c.z); 
            _partialExtent = _minimumExtent * (1.0f - skinWidth); 
            _sqrMinimumExtent = _minimumExtent * _minimumExtent; 
        } 
 
        void Update() 
        { 
            //have we moved more than our minimum extent? 
            var movementThisStep = transform.position - _previousPosition; 
            var movementSqrMagnitude = movementThisStep.sqrMagnitude;

            var movementMagnitude = Mathf.Sqrt(movementSqrMagnitude);
            if (movementSqrMagnitude > _sqrMinimumExtent) 
            {
                //check for obstructions we might have missed 
                Debug.DrawRay(_previousPosition, movementThisStep, Color.red, movementMagnitude);
                if (Physics.Raycast(_previousPosition, movementThisStep, out var hitInfo, movementMagnitude, layerMask.value)) 
                    transform.position = hitInfo.point - (movementThisStep/movementMagnitude)*_partialExtent; 
            } 
 
            _previousPosition = transform.position; 
        }
    }
}