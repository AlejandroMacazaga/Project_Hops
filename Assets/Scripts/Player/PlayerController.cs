using AYellowpaper.SerializedCollections;
using Entities;
using Input;
using Player.States;
using UnityEngine;
using Utils.AnimationSystem;
using Utils.StateMachine;

namespace Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : EntityController
    {
        new PlayerData data => (PlayerData)base.data;

        private PlayerStats _playerStats;
        private Camera _camera;
        private CharacterController _character;
        private StateMachine _sm;
        private AnimationSystem _animationSystem;
        public CharacterController Character => _character;
        public PlayerStats PlayerStats => _playerStats;
        public AnimationSystem AnimationSystem => _animationSystem;
        public StateMachine StateMachine => _sm;

        

        [Header("Input system")]
        [SerializeField] private InputReader inputReader;

        [Header("Movement values")]
        [SerializeField] private Vector2 currentDirection;
        [SerializeField] private float currentVelocity = 0f;
        [SerializeField] private float maxVelocity = 6f;
        [SerializedDictionary("Name", "Animation Clips")]
        public SerializedDictionary<string, AnimationClip> animations;


        [Header("Player input actions")]
        [SerializeField] private bool isPressingJump;
        public override void Awake()
        {
            base.Awake();
            _character = GetComponent<CharacterController>();
            //_animationSystem = new AnimationSystem(Animator, animations["idle"], animations["walk"]);
            _playerStats = new PlayerStats(data);
            _playerStats.AddModifier("Gravity", new StatModifier(StatModifier.ModifierType.Percent, 3f));

            #region State machine configuration
            _sm = new StateMachine();
            var idleState = new PlayerIdleState(this);
            var jumpState = new PlayerJumpState(this, null);

            _sm.AddAnyTransition(jumpState, new FuncPredicate(() => _character.isGrounded && isPressingJump));

            _sm.AddTransition(jumpState, idleState, new FuncPredicate(() => !_character.isGrounded));


            _sm.SetState(idleState);
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
            if (_character.isGrounded && currentVelocity <= 0f)
                currentVelocity = -1f;
            else
                currentVelocity -= _playerStats.GetStat("Gravity") * Time.deltaTime;
        }

        private void OnJump(bool pressed)
        {
            isPressingJump = pressed;
        }

        public void PlayAnimation(AnimationClip clip, bool loop = false)
        {
            _animationSystem.PlayOneShot(clip, loop);
        }

        public CharacterController GetCharacter()
        {
            return _character;
        }

    }
}
