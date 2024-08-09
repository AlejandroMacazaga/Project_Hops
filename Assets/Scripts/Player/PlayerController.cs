using System;
using System.Collections;
using AYellowpaper.SerializedCollections;
using Cinemachine;
using Entities;
using Input;
using Player.States;
using Projectiles;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using Utils.AnimationSystem;
using Utils.EventBus;
using Utils.StateMachine;
using Utils.Timers;
using Weapons;
using StateMachine = Utils.StateMachine.StateMachine;

namespace Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : EntityController
    {
        private new PlayerData Data => (PlayerData)base.data;

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
        public CinemachineFirstPerson fpScript;

        private Camera _camera;
        [SerializeField] private float interactableRange = 99f;
        
        [Header("Movement values")]
        public Vector2 currentDirection;
        [SerializeField] private float currentVelocity = 0f;
        [SerializeField] private float maxVelocity = 6f;
        [SerializeField] private Vector2 currentAirSpeed;
        [SerializedDictionary("Name", "Animation Clips")]
        public SerializedDictionary<string, AnimationClip> animations;

        [FormerlySerializedAs("projectileWeaponSettings")]
        [Header("Weapon data")] 
        [SerializeField] private RaycastWeaponSettings weaponSettings;

        [SerializeField] private ProjectileSettings projectileSettings;
        
        [Header("Player input actions")]
        [SerializeField] private bool isPressingJump;
        [SerializeField] private bool isPressingDash;
        
        [Header("Action blockers")]
        [SerializeField] private bool hasMovementBlocked = false;
        public bool isOnUnstableGround = false;

        private IInteractable _currentInteractable;

        public PlayerCooldownTimer DashCooldown;
        public override void Awake()
        {
            base.Awake();
            _character = GetComponent<CharacterController>();
            //_animationSystem = new AnimationSystem(Animator, animations["idle"], animations["walk"]);
            _playerStats = new PlayerStats(Data);
            StatModifier toZero = new(StatModifier.ModifierType.Zero);
            
            #region Cooldowns
            DashCooldown = new PlayerCooldownTimer(_playerStats, "DashCooldown");
            #endregion
            
            #region State machine configuration
            _sm = new StateMachine();
            var idleState = new PlayerIdleState(this);
            var jumpState = new PlayerJumpState(this, null, toZero);
            var onAirState = new PlayerOnAirState(this, null);
            var dashState = new PlayerDashState(this, null, toZero);
            var deathState = new PlayerDeathState(this);
            _sm.AddTransition(idleState, onAirState, new FuncPredicate(() => !_character.isGrounded));
            _sm.AddTransition(idleState, jumpState, new FuncPredicate(() => _character.isGrounded && isPressingJump && !isOnUnstableGround));
            _sm.AddTransition(onAirState, idleState, new FuncPredicate(() => _character.isGrounded));
            _sm.AddTransition(jumpState, onAirState, new FuncPredicate(() => jumpState.IsGracePeriodOver));
            _sm.AddAnyTransition(dashState, new FuncPredicate(() => isPressingDash && !DashCooldown.IsRunning));
            _sm.AddTransition(dashState, idleState, new FuncPredicate(() => dashState.IsFinished && _character.isGrounded));
            _sm.AddTransition(dashState, onAirState, new FuncPredicate(() => dashState.IsFinished && !_character.isGrounded));
            _sm.AddAnyTransition(deathState, new FuncPredicate(() => Health.IsDead));

            _sm.SetState(idleState);
            #endregion
            // _currentWeapon = new SingleShotWeapon(weaponSettings, this, projectileSettings);
            _currentWeapon = new ReaperMeleeWeapon(this, weaponSettings);
            #region Input system configuration
            inputReader.EnablePlayerActions();
            inputReader.Move += OnMove;
            inputReader.Jump += OnJump;
            inputReader.PrimaryAttack += OnPrimaryAttack;
            inputReader.Reload += OnReload;
            inputReader.Dash += OnDash;
            inputReader.Interact += OnInteract;

            #endregion

            _camera = Camera.main;

        }

        private void OnInteract()
        {
            Debug.Log("Hello");
            _currentInteractable?.Interact();
        }

        private void OnReload(bool pressed)
        {
            if (pressed) _currentWeapon.Reload();
        }

        private void OnEnable()
        {
            EnableCursor(false);
            StartCoroutine(nameof(HandleInteractableSearch));
        }

        private void Update()
        {
            _sm.Update();
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
            inputReader.PrimaryAttack -= OnPrimaryAttack;
            inputReader.Reload -= OnReload;
            inputReader.Dash -= OnDash;
            inputReader.Interact -= OnInteract;
            Health.OnDeath -= OnDeath;
            StopCoroutine(nameof(HandleInteractableSearch));
            // _animationSystem.Destroy();
        }

        private void OnMove(Vector2 dir)
        {
            currentDirection = dir;
        }

        public void HandleMovement()
        {
            var move = new Vector3(currentDirection.x, 0 , currentDirection.y);
            move = transform.TransformDirection(move);
            move *= PlayerStats.GetStat("Speed");
            move.y = currentVelocity;

            _character.Move(move * Time.deltaTime);
        }

        public IEnumerator HandleInteractableSearch()
        {
            while (true)
            {
                if(Physics.Raycast(
                    _camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f)),
                    _camera.transform.forward,
                    out var hit,
                    interactableRange))
                {
                    if (hit.transform.TryGetComponent<IInteractable>(out var interactable))
                    {
                        Debug.Log("This is an interactable component");
                        if (interactable != _currentInteractable)
                        {
                            _currentInteractable?.IsNotHovered();
                            _currentInteractable = interactable;
                            
                        }
                        _currentInteractable?.IsHovered();
                        // TODO: Do what needs to be done when something can be interacted with
                    }
                    else
                    {
                        if (_currentInteractable != null)
                        {
                            _currentInteractable?.IsNotHovered();
                            _currentInteractable = null;
                        }
                    }
                    
                }
                else
                {
                    if (_currentInteractable != null)
                    {
                        _currentInteractable.IsNotHovered();
                        _currentInteractable = null;
                    }
                }
                yield return Utils.Helpers.GetWaitForSeconds(0.1f);
            }
        }
        

        public void HandleAirMovement()
        {
            var move = new Vector3(currentDirection.x, 0 , currentDirection.y);
            move = transform.TransformDirection(move);
            move *= PlayerStats.GetStat("AirSpeed");
            move.y = currentVelocity;

            _character.Move(move * Time.deltaTime);
        }
        

        public void HandleGravity()
        {
            if (_character.isGrounded && currentVelocity <= 0f)
                currentVelocity = -3f;
            else
                currentVelocity -= _playerStats.GetStat("Gravity") * Time.deltaTime;
            if (currentVelocity < -maxVelocity) currentVelocity = -maxVelocity;
        }

        private void OnJump(bool pressed)
        {
            isPressingJump = pressed;
        }

        private void OnDash(bool pressed)
        {
            isPressingDash = pressed;
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

        private void OnPrimaryAttack()
        {
            _currentWeapon.PrimaryAttack();
        }

        public override void OnDeath()
        {
            
        }
    }
}
