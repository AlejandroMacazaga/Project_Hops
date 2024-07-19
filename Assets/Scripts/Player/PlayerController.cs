using Assets.Player;
using Assets.Scripts.Entities;
using UnityEngine;
using Utils.StateMachine;

namespace Assets.Scripts.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : EntityController
    {
        new PlayerData Data => (PlayerData)base.Data;
        private Camera _camera;
        private  CharacterController _character;
        private StateMachine _sm;
        private AnimationSystem _animationSystem;

        [Header("Input system")]
        [SerializeField] private InputReader inputReader;

        [Header("Movement values")]
        [SerializeField] private Vector2 currentDirection;

        [Header("Animations")]
        [SerializeField] private AnimationClip idleAnim;
        [SerializeField] private AnimationClip walkAnim;
        [SerializeField] private AnimationClip airAnim;
        public override void Awake() 
        {
            base.Awake();
            _character = GetComponent<CharacterController>();
            inputReader.EnablePlayerActions();
            inputReader.Move += OnMove;
            _animationSystem = new AnimationSystem(Animator, idleAnim, walkAnim);


            #region State machine configuration
            _sm = new StateMachine();

            PlayerIdleState idle = new(this);
            PlayerAirState air = new(this, airAnim);

            #endregion



        }

        private void Update()
        {
            _animationSystem.UpdateLocomotion(_character.velocity, Data.MaxSpeed);
        }

        private void OnDestroy()
        {
            inputReader.Move -= OnMove;
            _animationSystem.Destroy();
        }

        private void OnMove(Vector2 dir)
        {
            currentDirection = dir;
        }



    }
}
