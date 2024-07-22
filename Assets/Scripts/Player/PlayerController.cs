using Scripts.Entities;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using Utils.StateMachine;
using Utils.AnimationSystem;
using System;
namespace Scripts.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : EntityController
    {
        new PlayerData Data => (PlayerData)base.Data;

        private PlayerStats _playerStats;
        private Camera _camera;
        private CharacterController _character;
        private StateMachine _sm;
        private AnimationSystem _animationSystem;
        [HideInInspector] public CharacterController Character => _character;
        [HideInInspector] public PlayerStats PlayerStats => _playerStats;
        [HideInInspector] public AnimationSystem AnimationSystem => _animationSystem;
        [HideInInspector] public StateMachine StateMachine => _sm;

        

        [Header("Input system")]
        [SerializeField] private InputReader inputReader;

        [Header("Movement values")]
        [SerializeField] private Vector2 currentDirection;
        [SerializeField] private float currentVelocity = 0f;

        [SerializedDictionary("Name", "Animation Clips")]
        public SerializedDictionary<string, AnimationClip> animations;


        [Header("Player input actions")]
        [SerializeField] private bool isPressingJump;
        public override void Awake()
        {
            base.Awake();
            _character = GetComponent<CharacterController>();
            //_animationSystem = new AnimationSystem(Animator, animations["idle"], animations["walk"]);
            _playerStats = new PlayerStats(Data);
            _playerStats.AddModifier("Gravity", new StatModifier(StatModifier.ModifierType.Percent, 3f));

            #region State machine configuration
            _sm = new StateMachine();
            PlayerIdleState pidles = new PlayerIdleState(this);
            PlayerJumpState pjumps = new PlayerJumpState(this, null);

            _sm.AddAnyTransition(pjumps, new FuncPredicate(() =>
            {
                if (!_character.isGrounded) return false;

                return isPressingJump;
            }));

            _sm.AddTransition(pjumps, pidles, new FuncPredicate(() =>
            {
                return !_character.isGrounded;
            }));


            _sm.SetState(pidles);
            #endregion

            #region Input system configuration
            inputReader.EnablePlayerActions();
            inputReader.Move += OnMove;
            inputReader.Jump += OnJump;
            #endregion

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
            if (_character.isGrounded && currentVelocity < 0f)
                currentVelocity = -1f;
            else
                currentVelocity = -(_playerStats.GetStat("Gravity") * Time.deltaTime);
            Debug.Log(_playerStats.GetStat("Gravity"));
            Debug.Log(currentVelocity);
        }

        private void OnJump(bool pressed)
        {
            isPressingJump = pressed;
        }

        public void PlayAnimation(AnimationClip animation, bool loop = false)
        {
            _animationSystem.PlayOneShot(animation, loop);
        }

        public CharacterController GetCharacter()
        {
            return _character;
        }

    }
}
