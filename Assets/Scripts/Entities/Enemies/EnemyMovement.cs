using System;
using KBCore.Refs;
using Player;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Utils;
using Utils.StateMachine;
using Utils.Timers;

namespace Entities.Enemies
{
    public class EnemyMovement : MonoBehaviour
    {
        public GameObject prefab;  // The GameObject that has the NavMeshAgent
        [SerializeField] private NavMeshAgent navAgent;
        [Header("Collider Settings:")]
        [Range(0f, 1f)] [SerializeField] float stepHeightRatio = 0.1f;
        [SerializeField] float colliderHeight = 2f;
        [SerializeField] float colliderThickness = 1f;
        [SerializeField] Vector3 colliderOffset = Vector3.zero;
        
        [SerializeField, HideInInspector, Self] Rigidbody rb;
        [SerializeField, HideInInspector, Self] Transform tr;
        [SerializeField, HideInInspector, Self] CapsuleCollider col;
        RaycastSensor _sensor;
        
        bool _isGrounded;
        float _baseSensorRange;
        Vector3 _currentGroundAdjustmentVelocity; // Velocity to adjust player position to maintain ground contact
        int _currentLayer;
        
        [Header("Sensor Settings:")]
        [SerializeField] bool isInDebugMode;
        bool _isUsingExtendedSensorRange = true; // Use extended range for smoother ground transitions

        
        public float movementSpeed = 5f;

        public StateMachine MovementStateMachine;
        public GroundedState GroundedState;
        public AirborneState AirborneState;
        public StunnedState StunnedState;

        private CountdownTimer _stunnedTimer;

        public Transform target;
        private void Start()
        {
            SpawnNavigator();
            _stunnedTimer = new CountdownTimer(0f);
            
            
            MovementStateMachine = new StateMachine();

            GroundedState = new GroundedState(this);
            AirborneState = new AirborneState(this);
            StunnedState = new StunnedState(this);
            
            MovementStateMachine.AddTransition(GroundedState, AirborneState, new FuncPredicate(() => !IsGrounded()));
            MovementStateMachine.AddTransition(AirborneState, GroundedState, new FuncPredicate(() => IsGrounded()));
            MovementStateMachine.AddTransition(StunnedState, GroundedState, new FuncPredicate(() => _stunnedTimer.IsFinished() && IsGrounded()));
            MovementStateMachine.AddTransition(StunnedState, AirborneState, new FuncPredicate(() => _stunnedTimer.IsFinished() && !IsGrounded()));
            MovementStateMachine.AddAnyTransition(StunnedState,new FuncPredicate(() => _stunnedTimer.IsRunning));
            
            MovementStateMachine.SetState(GroundedState);
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }

        void Update()
        {
            CheckForGround();
            MovementStateMachine.Update();
            
        }

        void FixedUpdate()
        {
            
            // if (navAgent.isOnNavMesh) Debug.Log("Is in nav mesh");
            if (target && !(MovementStateMachine.CurrentState is StunnedState))
            {
                navAgent.SetDestination(target.position);
            }
            else
            {
                navAgent.SetDestination(transform.position);
            }
            
            Vector3 targetPosition = navAgent.transform.position;
            
            Vector3 direction = (targetPosition - tr.position).normalized;


            if (MovementStateMachine.CurrentState is GroundedState)
            {
                rb.MovePosition(transform.position + direction * (movementSpeed * Time.fixedDeltaTime));
                /* tr.position = navAgent.transform.position + Vector3.up * 0.5f; */
                tr.rotation = navAgent.transform.rotation; 
            }
            
        }

        public void Push(Vector3 force)
        {
            Debug.Log(force);
            rb.AddForce(force, ForceMode.Force);
        }

        public void Stun(float time)
        {
            _stunnedTimer.InitialTime = time;
            if (_stunnedTimer.IsRunning) _stunnedTimer.Reset();
            else _stunnedTimer.Start();
        }

        private void SpawnNavigator()
        {
            NavMeshTriangulation triangulation = NavMesh.CalculateTriangulation();
            NavMeshHit hit;
            if (!NavMesh.SamplePosition(transform.position, out hit, 1.0f, NavMesh.AllAreas)) return;
            navAgent = Instantiate(prefab).GetComponent<NavMeshAgent>();
            navAgent.Warp(hit.position);
            navAgent.speed = movementSpeed;
        }

        public bool RealocateNavigator()
        {
            NavMeshTriangulation triangulation = NavMesh.CalculateTriangulation();
            NavMeshHit hit;
            if (!NavMesh.SamplePosition(transform.position, out hit, 1.0f, NavMesh.AllAreas)) return false;

            navAgent.Warp(hit.position);
            navAgent.speed = movementSpeed;
            return true;
        }
        void OnValidate() {
            this.ValidateRefs();
            if (gameObject.activeInHierarchy) {
                RecalculateColliderDimensions();
            }
        }
        
        void LateUpdate() {
            if (isInDebugMode) {
                _sensor.DrawDebug();
            }
        }

        public void CheckForGround() {
            if (_currentLayer != gameObject.layer) {
                RecalculateSensorLayerMask();
            }
            
            _currentGroundAdjustmentVelocity = Vector3.zero;
            _sensor.castLength = _isUsingExtendedSensorRange 
                ? _baseSensorRange + colliderHeight * tr.localScale.x * stepHeightRatio
                : _baseSensorRange;
            _sensor.Cast();
            
            _isGrounded = _sensor.HasDetectedHit();
            if (!_isGrounded) return;
            
            float distance = _sensor.GetDistance();
            float upperLimit = colliderHeight * tr.localScale.x * (1f - stepHeightRatio) * 0.5f;
            float middle = upperLimit + colliderHeight * tr.localScale.x * stepHeightRatio;
            float distanceToGo = middle - distance;
            
            _currentGroundAdjustmentVelocity = tr.up * (distanceToGo / Time.fixedDeltaTime);
        }
        
        public bool IsGrounded() => _isGrounded;
        public Vector3 GetGroundNormal() => _sensor.GetNormal();
        
        // NOTE: Older versions of Unity use rb.velocity instead
        public void SetVelocity(Vector3 velocity) => rb.velocity = velocity + _currentGroundAdjustmentVelocity;
        public void SetExtendSensorRange(bool isExtended) => _isUsingExtendedSensorRange = isExtended;
        
        void RecalculateColliderDimensions() {
            if (col == null) {
                rb.freezeRotation = true;
                rb.useGravity = false;
            }
            
            col.height = colliderHeight * (1f - stepHeightRatio);
            col.radius = colliderThickness / 2f;
            col.center = colliderOffset * colliderHeight + new Vector3(0f, stepHeightRatio * col.height / 2f, 0f);

            if (col.height / 2f < col.radius) {
                col.radius = col.height / 2f;
            }
            
            RecalibrateSensor();
        }

        void RecalibrateSensor() {
            _sensor ??= new RaycastSensor(tr);
            
            _sensor.SetCastOrigin(col.bounds.center);
            _sensor.SetCastDirection(RaycastSensor.CastDirection.Down);
            RecalculateSensorLayerMask();
            
            const float safetyDistanceFactor = 0.001f; // Small factor added to prevent clipping issues when the sensor range is calculated
            
            float length = colliderHeight * (1f - stepHeightRatio) * 0.5f + colliderHeight * stepHeightRatio;
            _baseSensorRange = length * (1f + safetyDistanceFactor) * tr.localScale.x;
            _sensor.castLength = length * tr.localScale.x;
        }

        void RecalculateSensorLayerMask() {
            int objectLayer = gameObject.layer;
            int layerMask = Physics.AllLayers;

            for (int i = 0; i < 32; i++) {
                if (Physics.GetIgnoreLayerCollision(objectLayer, i)) {
                    layerMask &= ~(1 << i);
                }
            }
            
            int ignoreRaycastLayer = LayerMask.NameToLayer("Ignore Raycast");
            layerMask &= ~(1 << ignoreRaycastLayer);
            
            _sensor.layermask = layerMask;
            _currentLayer = objectLayer;
        }
    }
}