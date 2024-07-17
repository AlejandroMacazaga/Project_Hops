using Assets.Scripts.Entities;
using UnityEngine;
using Utils.StateMachine;

namespace Assets.Scripts.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : EntityController
    {
        new PlayerData Data => (PlayerData)base.Data;
        private Camera Camera;
        private CharacterController Character;
        private StateMachine PlayerStateMachine;

        [Header("Input system")]
        [SerializeField] private InputReader InputReader;


        [Header("Movement values")]
        [SerializeField] private Vector2 CurrentDirection;

        public override void Awake() 
        {
            base.Awake();
            Character = GetComponent<CharacterController>();
            InputReader.EnablePlayerActions();
            InputReader.Move += OnMove;


            #region State machine configuration
            PlayerStateMachine = new StateMachine();



            #endregion



        }

        private void OnDestroy()
        {
            InputReader.Move -= OnMove;
        }

        private void OnMove(Vector2 dir)
        {
            CurrentDirection = dir;
        }



    }
}
