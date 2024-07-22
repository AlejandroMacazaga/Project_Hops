using Scripts.Entities;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using Utils.StateMachine;
using Utils.AnimationSystem;
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

        [Header("Input system")]
        [SerializeField] private InputReader inputReader;

        [Header("Movement values")]
        [SerializeField] private Vector2 currentDirection;


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
        
        private void OnJump(bool pressed)
        {
            isPressingJump = pressed;
        }

        public void PlayAnimation(AnimationClip animation, bool loop = false)
        {
            _animationSystem.PlayOneShot(animation, loop);
        }
    }
}
