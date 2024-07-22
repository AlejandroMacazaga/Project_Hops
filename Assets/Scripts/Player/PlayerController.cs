using Assets.Scripts.Entities;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using Utils.StateMachine;
using Utils.AnimationSystem;
namespace Assets.Scripts.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : EntityController
    {
        new PlayerData Data => (PlayerData)base.Data;
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
        public override void Awake() 
        {
            base.Awake();
            _character = GetComponent<CharacterController>();
            inputReader.EnablePlayerActions();
            inputReader.Move += OnMove;
            _animationSystem = new AnimationSystem(Animator, animations["idle"], animations["walk"]);


            #region State machine configuration
            _sm = new StateMachine();

            #endregion



        }

        private void Update()
        {
            _sm.Update();
            _animationSystem.UpdateLocomotion(_character.velocity, Data.MaxSpeed);
        }

        private void FixedUpdate()
        {
            _sm.FixedUpdate();
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

        public void PlayAnimation(AnimationClip animation, bool loop = false)
        {
            _animationSystem.PlayOneShot(animation, loop);
        }
    }
}
