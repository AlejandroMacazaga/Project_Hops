using System;
using Input;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Classes
{
    public class CharacterClass : ValidatedMonoBehaviour, ICharacterClass
    {
        [SerializeField] protected ClassData data;
        [SerializeField, Self] public CharacterMover mover;
        [SerializeField] protected InputReader inputReader;
        
        public virtual void Awake()
        {
            inputReader.EnablePlayerActions();
            inputReader.Jump += OnJump;
            inputReader.Move += OnMove;
        }

        public virtual void OnDestroy()
        {
            inputReader.Jump += OnJump;
            inputReader.Move += OnMove;
        }
        
        public float GetCurrentStat(ClassStat stat)
        {
            return data.GetStat(stat);
        }

        public ClassData GetClassData()
        {
            return data;
        }
        
        private void OnJump(ActionState action, IInputInteraction interaction)
        {
            switch (action)
            {
                case ActionState.Press:
                    mover.isPressingJump = true;
                    break;
                case ActionState.Hold:
                    break;
                case ActionState.Release:
                    mover.isPressingJump = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(action), action, null);
            }
        }
        
        private void OnMove(Vector2 dir)
        {
            mover.currentDirection = dir;
        }
    }
    public interface ICharacterClass
    {
        public float GetCurrentStat(ClassStat stat);
        public ClassData GetClassData();
    }
}
