using System;
using Entities;
using Input;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Classes
{
    [RequireComponent(typeof(HealthComponent))]
    [RequireComponent(typeof(StaminaComponent))]
    [RequireComponent(typeof(CharacterMover))]
    [RequireComponent(typeof(CinemachineFirstPerson))]
    public class CharacterClass : ValidatedMonoBehaviour, ICharacterClass, IEntity
    {
        [SerializeField] public ClassData data;
        [SerializeField, Self, HideInInspector] public CharacterMover mover;
        [SerializeField] protected InputReader inputReader;
        [SerializeField, Self, HideInInspector] public SearchInteractable searcher;
        [SerializeField, Self, HideInInspector] public CinemachineFirstPerson fpCamera;
        [SerializeField, Self, HideInInspector] public Animator animator;
        [SerializeField, Self, HideInInspector] public HealthComponent health;
        [SerializeField, Self, HideInInspector] public StaminaComponent stamina;
        public virtual void Start()
        {
            health.SetValues(new HealthData()
            {
                healthPoints = data.GetStat(ClassStat.MaxHealth),
                maxHealthPoints =  data.GetStat(ClassStat.MaxHealth)
            });
            stamina.SetValues(new StaminaData()
            {
                staminaPoints = data.GetStat(ClassStat.MaxStamina),
                maxStaminaPoints = data.GetStat(ClassStat.MaxStamina),
                staminaRegeneration =  data.GetStat(ClassStat.StaminaRegen),
            });
            inputReader.EnablePlayerActions();
            inputReader.Jump += OnJump;
            inputReader.Move += OnMove;
            inputReader.Interact += OnInteract;
        }
        
        public virtual void OnDestroy()
        {
            inputReader.Jump -= OnJump;
            inputReader.Move -= OnMove;
            inputReader.Interact -= OnInteract;
        }
        
        public float GetCurrentStat(ClassStat stat)
        {
            return data.GetStat(stat);
        }

        public ClassData GetClassData()
        {
            return data;
        }

        protected virtual void OnJump(ActionState action, IInputInteraction interaction)
        {
            switch (action)
            {
                case ActionState.Press:
                    mover.Jump();
                    break;
                case ActionState.Hold:
                    break;
                case ActionState.Release:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(action), action, null);
            }
        }
        
        private void OnInteract(ActionState action, IInputInteraction interaction)
        {
            switch (action)
            {
                case ActionState.Press:
                    searcher.CurrentInteractable?.Interact();
                    break;
                case ActionState.Hold:
                    break;
                case ActionState.Release:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(action), action, null);
            }
        }
        
        private void OnMove(Vector2 dir)
        {
            mover.currentDirection = dir;
        }

        public EntityTeam GetTeam()
        {
            return EntityTeam.Player;
        }
    }
    public interface ICharacterClass
    {
        public float GetCurrentStat(ClassStat stat);
        public ClassData GetClassData();
    }
}
