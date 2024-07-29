using System;
using AYellowpaper.SerializedCollections;
using Entities;
using Input;
using Player.States;
using Projectiles;
using UnityEngine;
using Utils.AnimationSystem;
using Utils.EventBus;
using Utils.StateMachine;
using Weapons;
using StateMachine = Utils.StateMachine.StateMachine;

namespace Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : EntityController
    {
        private new PlayerData data => (PlayerData)base.data;

        private PlayerStats _playerStats;
        private CharacterController _character;
        private StateMachine _sm;
        private AnimationSystem _animationSystem;
        public CharacterController Character => _character;
        public PlayerStats PlayerStats => _playerStats;
        public AnimationSystem AnimationSystem => _animationSystem;
        public StateMachine StateMachine => _sm;
        public bool IsPressingJump => isPressingJump;

        private IWeapon _currentWeapon;

        [Header("Input system")]
        [SerializeField] private InputReader inputReader;

        [Header("First Person Camera")]
        [SerializeField] private Camera fpCamera;
        [Header("Movement values")]
        [SerializeField] private Vector2 currentDirection;
        [SerializeField] private float currentVelocity = 0f;
        [SerializeField] private float maxVelocity = 6f;
        [SerializedDictionary("Name", "Animation Clips")]
        public SerializedDictionary<string, AnimationClip> animations;

        [Header("Weapon data")] 
        [SerializeField] private ProjectileSettings projectileSettings;
        [Header("Player input actions")]
        [SerializeField] private bool isPressingJump;
        
        [Header("Action blockers")]
        [SerializeField] private bool hasMovementBlocked = false;
        public bool isOnUnstableGround = false;
        
        public override void Awake()
        {
            base.Awake();
            _character = GetComponent<CharacterController>();
            //_animationSystem = new AnimationSystem(Animator, animations["idle"], animations["walk"]);
            _playerStats = new PlayerStats(data);
            StatModifier multiplyByZero = new(StatModifier.ModifierType.Percent, 0f);
            
            #region State machine configuration
            _sm = new StateMachine();
            var idleState = new PlayerIdleState(this);
            var jumpState = new PlayerJumpState(this, null, multiplyByZero);
            var onAirState = new PlayerOnAirState(this, null);
            _sm.AddTransition(idleState, onAirState, new FuncPredicate(() => !_character.isGrounded));
            _sm.AddTransition(idleState, jumpState, new FuncPredicate(() => _character.isGrounded && isPressingJump && !isOnUnstableGround));
            _sm.AddTransition(onAirState, idleState, new FuncPredicate(() => _character.isGrounded));
            _sm.AddTransition(jumpState, onAirState, new FuncPredicate(() => jumpState.IsGracePeriodOver));
            

            _sm.SetState(idleState);
            #endregion

            #region Input system configuration
            inputReader.EnablePlayerActions();
            inputReader.Move += OnMove;
            inputReader.Jump += OnJump;
            inputReader.LightAttack += OnLightAttack;

            #endregion


            _currentWeapon = new Weapon(projectileSettings, fpCamera.gameObject);
        }

        private void OnEnable()
        {
            EnableCursor(false);
        }

        private void Update()
        {
            _sm.Update();
            HandleGravity();
            HandleMovement();
            // _animationSystem.UpdateLocomotion(_character.velocity, Data.MaxSpeed);
        }

        
        private void FixedUpdate()
        {
            _sm.FixedUpdate();
        }

        private void OnDestroy()
        {
            inputReader.Move -= OnMove;
            inputReader.Jump -= OnJump;
            inputReader.LightAttack -= OnLightAttack;
            // _animationSystem.Destroy();
        }

        private void OnMove(Vector2 dir)
        {
            currentDirection = dir;
        }

        private void HandleMovement()
        {
            var move = new Vector3(currentDirection.x, 0 , currentDirection.y);
            move = transform.TransformDirection(move);
            move.y = currentVelocity;
            move *= PlayerStats.GetStat("MaxSpeed");
 

            _character.Move(move * Time.deltaTime);
        }

        private void HandleGravity()
        {
            if (_character.isGrounded && currentVelocity <= 0f)
                currentVelocity = -1f;
            else
                currentVelocity -= _playerStats.GetStat("Gravity") * Time.deltaTime;
            if (currentVelocity < -maxVelocity) currentVelocity = -maxVelocity;
        }

        private void OnJump(bool pressed)
        {
            isPressingJump = pressed;
        }

        public void PlayAnimation(AnimationClip clip, bool loop = false)
        {
            _animationSystem.PlayOneShot(clip, loop);
        }

        public void SetVelocity(float value)
        {
            currentVelocity = value;
        }


        private static void EnableCursor(bool enable)
        {
            Cursor.lockState = enable ? CursorLockMode.Locked : CursorLockMode.None;
        }

        private void OnLightAttack()
        {
            _currentWeapon.Shoot();
        }
    }
}
