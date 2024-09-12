using System;
using Input;
using KBCore.Refs;
using Player.Classes;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Utils.StateMachine;
using Utils.Timers;

namespace Player
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(CeilingDetector))]
    [RequireComponent(typeof(GroundChecker))]
    public class CharacterMover : ValidatedMonoBehaviour
    {
        [SerializeField, Self] private InterfaceRef<CharacterClass> characterClass;
        [SerializeField, Self] public CharacterController characterController;
        [SerializeField, Self] public CeilingDetector ceilingDetector;
        [SerializeField, Self] public GroundChecker groundChecker;

        [SerializeField, HideInInspector, Self]
        private Transform tr;
        /*
        public float lastYRotation;
        private bool _wasTooSharp = false;
        public float rotationThresholdPerSecond = 90f;
        
        */
        public StateMachine MovementStateMachine
        {
            get;
            private set;
        }
        
        [SerializeField] private float coyoteTime = 0.2f;
        private CountdownTimer _coyoteTimer;
        
        [SerializeField] public int currentAmountOfAirJumps = 0;
        [SerializeField] public float currentVelocity;
        [SerializeField] public Vector2 currentDirection;
        [SerializeField] public Vector2 currentSpeed;
        [SerializeField] public bool isBodyLocked = false;
        [SerializeField] public bool isMovementLocked = false;
        public GroundedState GroundedState;
        public AirborneState AirborneState;
        
        void Awake()
        {
            _coyoteTimer = new CountdownTimer(coyoteTime);
            MovementStateMachine = new StateMachine();
            GroundedState = new GroundedState(characterClass.Value);
            AirborneState = new AirborneState(characterClass.Value, _coyoteTimer);
            MovementStateMachine.AddTransition(GroundedState, AirborneState,
                new FuncPredicate(() => !characterController.isGrounded));
            MovementStateMachine.AddTransition(GroundedState, AirborneState, new FuncPredicate(() => !characterController.isGrounded));
            MovementStateMachine.AddTransition(AirborneState, GroundedState, new FuncPredicate(() => characterController.isGrounded));
            currentAmountOfAirJumps = (int) characterClass.Value.GetCurrentStat(ClassStat.MaxAirJumps);
            MovementStateMachine.SetState(AirborneState);
            /* lastYRotation = tr.eulerAngles.y; */
        }

        /* private void HandleRotation()
        {
            var currentYRotation =  tr.eulerAngles.y;
            var rotationDelta = Mathf.DeltaAngle(lastYRotation, currentYRotation);

            if (rotationDelta != 0f)
            {

                float rotationDeltad = rotationDelta * Time.deltaTime;


                _wasTooSharp = rotationDeltad > rotationThresholdPerSecond * Time.deltaTime;
            }
            
            lastYRotation = currentYRotation;
        } */
        
        private void Update()
        {
           // HandleRotation();
            HandleGravity();
            MovementStateMachine.Update();
        }
        
        private void FixedUpdate()
        {
            MovementStateMachine.FixedUpdate();
        }

        public void Jump()
        {
            if (MovementStateMachine.CurrentState is not IJumpCancelable) return;
            currentSpeed = currentDirection;
            if (MovementStateMachine.CurrentState is GroundedState)
            {
                currentVelocity = characterClass.Value.GetCurrentStat(ClassStat.JumpForce);
                return;
            }

            if (MovementStateMachine.CurrentState is AirborneState && currentAmountOfAirJumps > 0)
            {
                currentAmountOfAirJumps--;
                currentVelocity = characterClass.Value.GetCurrentStat(ClassStat.JumpForce);
                return;
            }

            if (MovementStateMachine.CurrentState is DashingState state)
            {
                if (state.AirborneDash && currentAmountOfAirJumps <= 0) return;
                state.Timer.Stop();
                currentVelocity = characterClass.Value.GetCurrentStat(ClassStat.JumpForce);
            }
        }
        
        public void HandleMovement()
        {
            currentSpeed = currentDirection;
            var move = new Vector3(currentSpeed.x, 0 , currentSpeed.y);
            move = transform.TransformDirection(move);
            move *= characterClass.Value.GetCurrentStat(ClassStat.Speed);
            move.y = currentVelocity;
            
            characterController.Move(move * Time.deltaTime);
        }
        
        public void HandleAirMovement()
        {
            // Calculate the desired acceleration based on input
            var acceleration = new Vector2(
                currentDirection.x != 0f ? currentDirection.x * characterClass.Value.GetCurrentStat(ClassStat.AirAcceleration) * Time.deltaTime : 0f,
                currentDirection.y != 0f ? currentDirection.y * characterClass.Value.GetCurrentStat(ClassStat.AirAcceleration) * Time.deltaTime : 0f
            );

            // Combine the current speed and acceleration
            var newSpeed = currentSpeed + acceleration;

            // Clamp the acceleration effect (ignoring current momentum) to MaxAirSpeed
            var clampedSpeed = Vector2.ClampMagnitude(newSpeed, characterClass.Value.GetCurrentStat(ClassStat.MaxAirSpeed));

            // Apply the clamped acceleration while allowing momentum to persist
            currentSpeed = (currentSpeed.magnitude > characterClass.Value.GetCurrentStat(ClassStat.MaxAirSpeed))
                ? currentSpeed    // Preserve momentum (if already faster)
                : clampedSpeed;  // Apply clamped acceleration if below MaxAirSpeed

            // Convert 2D speed into 3D movement
            var move = new Vector3(currentSpeed.x, 0, currentSpeed.y);
            move = transform.TransformDirection(move);
    
            // Scale movement by the character's overall speed factor
            move *= characterClass.Value.GetCurrentStat(ClassStat.Speed);

            // Maintain the y-axis (falling) velocity
            move.y = currentVelocity;

            // Apply movement to the character controller
            characterController.Move(move * Time.deltaTime);
        }
        
        public void HandleGravity()
        {
            var maxVelocity = characterClass.Value.GetCurrentStat(ClassStat.MaxVelocity);
            if (characterController.isGrounded && currentVelocity <= 0f)
                currentVelocity = -3f;
            else
                currentVelocity -= characterClass.Value.GetCurrentStat(ClassStat.Gravity) * Time.deltaTime;
            if (currentVelocity < -maxVelocity) currentVelocity = -maxVelocity;
        }

        
    }
}