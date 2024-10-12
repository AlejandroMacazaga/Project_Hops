using System;
using AdvancedController;
using Input;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityUtils;
using Utils.StateMachine;
using Utils.Timers;

namespace Player.KinematicController {
    [RequireComponent(typeof(PlayerMover))]
    public class KinematicPlayerController : ValidatedMonoBehaviour {
        #region Fields
        [SerializeField] InputReader input;
        
        [SerializeField, HideInInspector, Self] Transform tr;
        [SerializeField, HideInInspector, Self] PlayerMover mover;
        [SerializeField, HideInInspector, Self] AdvancedController.CeilingDetector ceilingDetector;
        
        
        public float movementSpeed = 7f;
        public float airControlRate = 2f;
        public float jumpSpeed = 10f;
        public float jumpDuration = 0.2f;
        public float airFriction = 0.5f;
        public float groundFriction = 100f;
        public float gravity = 30f;
        public float slideGravity = 5f;
        public float slopeLimit = 30f;
        public bool useLocalMomentum;
        
        StateMachine _stateMachine;
        
        [SerializeField] Transform cameraTransform;
        
        public Vector3 momentum, savedVelocity, savedMovementVelocity;
        
        public event Action<Vector3> OnJump = delegate { };
        public event Action<Vector3> OnLand = delegate { };
        #endregion
        
        bool IsGrounded() => _stateMachine.CurrentState is GroundedState or SlidingState;
        public Vector3 GetVelocity() => savedVelocity;
        public Vector3 GetMomentum() => useLocalMomentum ? tr.localToWorldMatrix * momentum : momentum;
        public Vector3 GetMovementVelocity() => savedMovementVelocity;

        void Awake() {
            tr = transform;
            mover = GetComponent<PlayerMover>();
            ceilingDetector = GetComponent<AdvancedController.CeilingDetector>();
            
            SetupStateMachine();
        }

        void Start() {
            input.EnablePlayerActions();
            input.Jump += HandleJumpKeyInput;
        }

        void HandleJumpKeyInput(ActionState state, IInputInteraction interaction) {
           
        }

        void SetupStateMachine() {
            _stateMachine = new StateMachine();
            
            var grounded = new GroundedState(this);
            var falling = new FallingState(this);
            var sliding = new SlidingState(this);
            var rising = new RisingState(this);
            
            At(grounded, rising, new FuncPredicate(() => IsRising()));
            At(grounded, sliding, new FuncPredicate(() => mover.IsGrounded() && IsGroundTooSteep()));
            At(grounded, falling, new FuncPredicate(() => !mover.IsGrounded()));
            
            At(falling, rising, new FuncPredicate(() => IsRising()));
            At(falling, grounded, new FuncPredicate(() => mover.IsGrounded() && !IsGroundTooSteep()));
            At(falling, sliding, new FuncPredicate(() => mover.IsGrounded() && IsGroundTooSteep()));
            
            At(sliding, rising, new FuncPredicate(() => IsRising()));
            At(sliding, falling, new FuncPredicate(() => !mover.IsGrounded()));
            At(sliding, grounded, new FuncPredicate(() => mover.IsGrounded() && !IsGroundTooSteep()));
            
            At(rising, grounded, new FuncPredicate(() => mover.IsGrounded() && !IsGroundTooSteep()));
            At(rising, sliding, new FuncPredicate(() => mover.IsGrounded() && IsGroundTooSteep()));
            At(rising, falling, new FuncPredicate(() => IsFalling()));
            At(rising, falling, new FuncPredicate(() => ceilingDetector && ceilingDetector.HitCeiling()));
            
            _stateMachine.SetState(falling);
        }
        
        void At(IState from, IState to, FuncPredicate condition) => _stateMachine.AddTransition(from, to, condition);
        void Any<T>(IState to, FuncPredicate condition) => _stateMachine.AddAnyTransition(to, condition);
        
        bool IsRising() => VectorMath.GetDotProduct(GetMomentum(), tr.up) > 0f;
        bool IsFalling() => VectorMath.GetDotProduct(GetMomentum(), tr.up) < 0f;
        bool IsGroundTooSteep() => !mover.IsGrounded() || Vector3.Angle(mover.GetGroundNormal(), tr.up) > slopeLimit;
        
        void Update() => _stateMachine.Update();

        void FixedUpdate() {
            _stateMachine.FixedUpdate();
            mover.CheckForGround();
            HandleMomentum();
            Vector3 velocity = _stateMachine.CurrentState is GroundedState ? CalculateMovementVelocity() : Vector3.zero;
            velocity += useLocalMomentum ? tr.localToWorldMatrix * momentum : momentum;
            
            mover.SetExtendSensorRange(IsGrounded());
            mover.SetVelocity(velocity);
            
            savedVelocity = velocity;
            savedMovementVelocity = CalculateMovementVelocity();
            
            if (ceilingDetector) ceilingDetector.Reset();
        }
        
        Vector3 CalculateMovementVelocity() => CalculateMovementDirection() * movementSpeed;

        Vector3 CalculateMovementDirection() {
            Vector3 direction = !cameraTransform 
                ? tr.right * input.Direction.x + tr.forward * input.Direction.y 
                : Vector3.ProjectOnPlane(cameraTransform.right, tr.up).normalized * input.Direction.x + 
                  Vector3.ProjectOnPlane(cameraTransform.forward, tr.up).normalized * input.Direction.y;
            
            return direction.magnitude > 1f ? direction.normalized : direction;
        }

        void HandleMomentum() {
            if (useLocalMomentum) momentum = tr.localToWorldMatrix * momentum;
            
            Vector3 verticalMomentum = VectorMath.ExtractDotVector(momentum, tr.up);
            Vector3 horizontalMomentum = momentum - verticalMomentum;
            
            verticalMomentum -= tr.up * (gravity * Time.deltaTime);
            if (_stateMachine.CurrentState is GroundedState && VectorMath.GetDotProduct(verticalMomentum, tr.up) < 0f) {
                verticalMomentum = Vector3.zero;
            }

            if (!IsGrounded()) {
                AdjustHorizontalMomentum(ref horizontalMomentum, CalculateMovementVelocity());
            }

            if (_stateMachine.CurrentState is SlidingState) {
                HandleSliding(ref horizontalMomentum);
            }
            
            float friction = _stateMachine.CurrentState is GroundedState ? groundFriction : airFriction;
            horizontalMomentum = Vector3.MoveTowards(horizontalMomentum, Vector3.zero, friction * Time.deltaTime);
            
            momentum = horizontalMomentum + verticalMomentum;
            
            
            if (_stateMachine.CurrentState is SlidingState) {
                momentum = Vector3.ProjectOnPlane(momentum, mover.GetGroundNormal());
                if (VectorMath.GetDotProduct(momentum, tr.up) > 0f) {
                    momentum = VectorMath.RemoveDotVector(momentum, tr.up);
                }
            
                Vector3 slideDirection = Vector3.ProjectOnPlane(-tr.up, mover.GetGroundNormal()).normalized;
                momentum += slideDirection * (slideGravity * Time.deltaTime);
            }
            
            if (useLocalMomentum) momentum = tr.worldToLocalMatrix * momentum;
        }

        void HandleJumping() {
            momentum = VectorMath.RemoveDotVector(momentum, tr.up);
            momentum += tr.up * jumpSpeed;
        }
        

        public void OnJumpStart() {
            if (useLocalMomentum) momentum = tr.localToWorldMatrix * momentum;
            
            momentum += tr.up * jumpSpeed;
            OnJump.Invoke(momentum);
            
            if (useLocalMomentum) momentum = tr.worldToLocalMatrix * momentum;
        }

        public void OnGroundContactLost() {
            if (useLocalMomentum) momentum = tr.localToWorldMatrix * momentum;
            
            Vector3 velocity = GetMovementVelocity();
            if (velocity.sqrMagnitude >= 0f && momentum.sqrMagnitude > 0f) {
                Vector3 projectedMomentum = Vector3.Project(momentum, velocity.normalized);
                float dot = VectorMath.GetDotProduct(projectedMomentum.normalized, velocity.normalized);
                
                if (projectedMomentum.sqrMagnitude >= velocity.sqrMagnitude && dot > 0f) velocity = Vector3.zero;
                else if (dot > 0f) velocity -= projectedMomentum;
            }
            momentum += velocity;
            
            if (useLocalMomentum) momentum = tr.worldToLocalMatrix * momentum;
        }

        public void OnGroundContactRegained() {
            Vector3 collisionVelocity = useLocalMomentum ? tr.localToWorldMatrix * momentum : momentum;
            OnLand.Invoke(collisionVelocity);
        }

        public void OnFallStart() {
            var currentUpMomemtum = VectorMath.ExtractDotVector(momentum, tr.up);
            momentum = VectorMath.RemoveDotVector(momentum, tr.up);
            momentum -= tr.up * currentUpMomemtum.magnitude;
        }
        
        void AdjustHorizontalMomentum(ref Vector3 horizontalMomentum, Vector3 movementVelocity) {
            if (horizontalMomentum.magnitude > movementSpeed) {
                if (VectorMath.GetDotProduct(movementVelocity, horizontalMomentum.normalized) > 0f) {
                    movementVelocity = VectorMath.RemoveDotVector(movementVelocity, horizontalMomentum.normalized);
                }
                horizontalMomentum += movementVelocity * (Time.deltaTime * airControlRate * 0.25f);
            }
            else {
                horizontalMomentum += movementVelocity * (Time.deltaTime * airControlRate);
                horizontalMomentum = Vector3.ClampMagnitude(horizontalMomentum, movementSpeed);
            }
        }

        void HandleSliding(ref Vector3 horizontalMomentum) {
            Vector3 pointDownVector = Vector3.ProjectOnPlane(mover.GetGroundNormal(), tr.up).normalized;
            Vector3 movementVelocity = CalculateMovementVelocity();
            movementVelocity = VectorMath.RemoveDotVector(movementVelocity, pointDownVector);
            horizontalMomentum += movementVelocity * Time.fixedDeltaTime;
        }
    }
} 