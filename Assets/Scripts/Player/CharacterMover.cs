using System;
using Input;
using KBCore.Refs;
using Player.Classes;
using Player.States;
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

        public StateMachine MovementStateMachine
        {
            get;
            private set;
        }
        
        [SerializeField] private float coyoteTime = 0.2f;
        private CountdownTimer _coyoteTimer;
        
        [SerializeField] public bool isPressingJump = false;
        [SerializeField] public float currentVelocity;
        [SerializeField] public Vector2 currentDirection;
        [SerializeField] public Vector2 currentSpeed;
        [SerializeField] public bool isBodyLocked = false;

        public GroundedState GroundedState;
        public JumpingState JumpingState;
        public AirborneState AirborneState;
        
        void Awake()
        {
            _coyoteTimer = new CountdownTimer(coyoteTime);
            MovementStateMachine = new StateMachine();
            GroundedState = new GroundedState(characterClass.Value);
            JumpingState = new JumpingState(characterClass.Value);
            AirborneState = new AirborneState(characterClass.Value);
            MovementStateMachine.AddTransition(GroundedState, AirborneState,
                new FuncPredicate(() => !characterController.isGrounded), _coyoteTimer.Start);
            MovementStateMachine.AddTransition(GroundedState, JumpingState,
                new FuncPredicate(() => characterController.isGrounded && isPressingJump));
            MovementStateMachine.AddTransition(AirborneState, GroundedState, new FuncPredicate(() => characterController.isGrounded));
            MovementStateMachine.AddTransition(AirborneState, JumpingState, new FuncPredicate(() => (_coyoteTimer.IsRunning) && isPressingJump));
            MovementStateMachine.AddTransition(JumpingState, AirborneState, new FuncPredicate(() => JumpingState.IsGracePeriodOver));
            
            MovementStateMachine.SetState(AirborneState);
        }
        
        private void Update()
        {
            HandleGravity();
            MovementStateMachine.Update();
        }
        
        private void FixedUpdate()
        {
            MovementStateMachine.FixedUpdate();
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
            currentSpeed = new Vector2((currentDirection.x != 0f ? currentSpeed.x + (currentDirection.x * characterClass.Value.GetCurrentStat(ClassStat.AirAcceleration) * Time.deltaTime) : currentSpeed.x),
                (currentDirection.y != 0f ? currentSpeed.y + (currentDirection.y * characterClass.Value.GetCurrentStat(ClassStat.AirAcceleration) * Time.deltaTime) : currentSpeed.y));
            currentSpeed = Vector2.ClampMagnitude(currentSpeed, characterClass.Value.GetCurrentStat(ClassStat.MaxAirSpeed));
            var move = new Vector3(currentSpeed.x, 0 , currentSpeed.y);
            move = transform.TransformDirection(move);
            move *= characterClass.Value.GetCurrentStat(ClassStat.Speed);
            move.y = currentVelocity;

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