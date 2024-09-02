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
        [SerializeField, Self] private CharacterController characterController;
        [SerializeField, Self] private CeilingDetector ceilingDetector;
        [SerializeField, Self] private GroundChecker groundChecker;

        public StateMachine MovementStateMachine
        {
            get;
            private set;
        }
        
        [SerializeField] private float coyoteTime = 0.2f;
        private CountdownTimer _coyoteTimer;
        private PlayerCooldownTimer _dashCooldown;
        
        [SerializeField] public bool isPressingJump = false;
        [SerializeField] public bool isPressingDash = false;
        [SerializeField] private int amountOfAirJumps = 0;
        [SerializeField] private int currentAmountOfAirJumps = 0;
        [SerializeField] public float currentVelocity;
        [SerializeField] public Vector2 currentDirection;
        [SerializeField] public Vector2 currentSpeed;
        [SerializeField] private int amountOfAirDash = 1;
        [SerializeField] private int currentAmountOfAirDash = 1;
        [SerializeField] public bool isBodyLocked = false;

        public GroundedState GroundedState;
        public JumpingState JumpingState;
        public AirborneState AirborneState;
        
        void Awake()
        {
            _coyoteTimer = new CountdownTimer(coyoteTime);
            _dashCooldown = new PlayerCooldownTimer(characterClass.Value.GetClassData(), ClassStat.DashCooldown);
            MovementStateMachine = new StateMachine();
            GroundedState = new GroundedState(characterClass.Value);
            JumpingState = new JumpingState(characterClass.Value);
            AirborneState = new AirborneState(characterClass.Value);
            MovementStateMachine.AddTransition(GroundedState, AirborneState,
                new FuncPredicate(() => !characterController.isGrounded), _coyoteTimer.Start);
            MovementStateMachine.AddTransition(GroundedState, JumpingState,
                new FuncPredicate(() => characterController.isGrounded && isPressingJump));
            MovementStateMachine.AddTransition(AirborneState, GroundedState, new FuncPredicate(() => characterController.isGrounded));
            MovementStateMachine.AddTransition(AirborneState, JumpingState, new FuncPredicate(() => (_coyoteTimer.IsRunning || currentAmountOfAirJumps > 0) && isPressingJump));
            MovementStateMachine.AddTransition(JumpingState, AirborneState, new FuncPredicate(() => JumpingState.IsGracePeriodOver));
            
            MovementStateMachine.SetState(AirborneState);
        }
        
        private void Update()
        {
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
            
            characterController.Move(move * Time.fixedDeltaTime);
        }
        
        public void HandleAirMovement()
        {
            currentSpeed = new Vector2((currentDirection.x != 0f ? currentSpeed.x + (currentDirection.x * characterClass.Value.GetCurrentStat(ClassStat.AirAcceleration) * Time.fixedDeltaTime) : currentSpeed.x),
                (currentDirection.y != 0f ? currentSpeed.y + (currentDirection.y * characterClass.Value.GetCurrentStat(ClassStat.AirAcceleration) * Time.fixedDeltaTime) : currentSpeed.y));
            currentSpeed = Vector2.ClampMagnitude(currentSpeed, characterClass.Value.GetCurrentStat(ClassStat.MaxAirSpeed));
            var move = new Vector3(currentSpeed.x, 0 , currentSpeed.y);
            move = transform.TransformDirection(move);
            move *= characterClass.Value.GetCurrentStat(ClassStat.Speed);
            move.y = currentVelocity;

            characterController.Move(move * Time.fixedDeltaTime);
        }
        
        public void HandleGravity()
        {
            var maxVelocity = characterClass.Value.GetCurrentStat(ClassStat.MaxVelocity);
            if (characterController.isGrounded && currentVelocity <= 0f)
                currentVelocity = -3f;
            else
                currentVelocity -= characterClass.Value.GetCurrentStat(ClassStat.Gravity) * Time.fixedDeltaTime;
            if (currentVelocity < -maxVelocity) currentVelocity = -maxVelocity;
        }

        public void HandleDashing()
        {
            currentSpeed = currentDirection;
            var move = new Vector3(currentSpeed.x, 0 , currentSpeed.y);
            move = transform.TransformDirection(move);
            move *= characterClass.Value.GetCurrentStat(ClassStat.Speed);
            move.y = currentVelocity;
            
            characterController.Move(move * Time.fixedDeltaTime);
        }
        
    }
}