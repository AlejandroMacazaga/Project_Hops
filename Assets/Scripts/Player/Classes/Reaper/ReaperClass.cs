using System;
using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Entities;
using Entities.Attacks;
using Input;
using Items;
using KBCore.Refs;
using Player.Events;
using Projectiles;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.Serialization;
using Utils.AnimationSystem;
using Utils.EventBus;
using Utils.Flyweight;
using Utils.StateMachine;
using Utils.Timers;
using Weapons;
using Random = UnityEngine.Random;

namespace Player.Classes.Reaper
{
    [RequireComponent(typeof(BloodResourceComponent))]
    public class ReaperClass : CharacterClass
    {
        public DashingState DashingState;
        public PlayerCooldownTimer DashCooldown;

        public ReaperNoAttackState NoAttackState;
        public ReaperPrimaryAttackChargingState ChargingState;
        public ReaperFastPrimaryAttackState FastPrimaryAttackState;
        public ReaperChargedPrimaryAttackState ChargedPrimaryAttackState;
        public ReaperSecondaryAttackState SecondaryAttackState;
        public ReaperReloadAttackState ReloadAttackState;
        
        public readonly StateMachine CombatStateMachine = new(); 
        
        [FormerlySerializedAs("bloodResourceComponent")]
        [Header("Secondary Attack")]
        [SerializeField, Self, HideInInspector] public BloodResourceComponent blood;
        [SerializeField] public BulletTrailSettings secondaryAttackSettings;
        [SerializeField] public Transform bulletSpawnTransform;
        public float bulletTravelDuration = 0.1f;
        
        
        [SerializeField] public AnimatorConfig animatorConfig;
        public AnimationSystem AnimationSystem;
        PlayableGraph _playableGraph;
        
        
        private bool _isPressingDash, _hasStartedPrimaryAttack;
        public bool isAttacking = false;
        public bool isPressingPrimaryAttack = false;
        public bool isPressingReload = false;
        public bool isLeftAttack = false;
        public CountdownTimer HoldAttackTimer;
        public float spreadAngle;
        public LayerMask secondaryAttackLayerMask;
        private AttackHoldEvent _holdEvent;
        
        public readonly StatModifier ChargeSlowdown = new StatModifier(ModifierType.Multiplier, 0.5f);
        
        [SerializeField] public SerializedDictionary<ReaperAction, ReaperAttack> attacks;
        
        private Action _primaryAttackStart, _secondaryAttackStart, _reload;
        
        private Camera _cam;
        public override void Start()
        {
            base.Start();
            AnimationSystem = new AnimationSystem(animatorConfig);
            #region MovementStateMachine
            DashingState = new(this);
            DashCooldown = new PlayerCooldownTimer(data, ClassStat.DashCooldown);
            mover.MovementStateMachine.AddTransition(mover.GroundedState, DashingState, new FuncPredicate(() => _isPressingDash && DashCooldown.IsFinished() && stamina.UseStamina(50)), () => DashingState.AirborneDash = false);
            mover.MovementStateMachine.AddTransition(mover.AirborneState, DashingState, new FuncPredicate(() => _isPressingDash && DashCooldown.IsFinished() && stamina.UseStamina(50)), () => DashingState.AirborneDash = true);

            mover.MovementStateMachine.AddTransition(DashingState, mover.GroundedState,
                new FuncPredicate(() => DashingState.IsFinished && mover.characterController.isGrounded), () => DashCooldown.Start());
            mover.MovementStateMachine.AddTransition(DashingState, mover.AirborneState, new FuncPredicate(() => DashingState.IsFinished && !mover.characterController.isGrounded), () => DashCooldown.Start());
            #endregion
            
            #region ReaperStateMachine
            
            NoAttackState = new ReaperNoAttackState(this);
            ChargingState = new ReaperPrimaryAttackChargingState(this);
            FastPrimaryAttackState = new ReaperFastPrimaryAttackState(this, attacks[ReaperAction.FastPrimaryAttack]);
            ChargedPrimaryAttackState = new ReaperChargedPrimaryAttackState(this,  attacks[ReaperAction.ChargedPrimaryAttack]);
            SecondaryAttackState = new ReaperSecondaryAttackState(this);
            ReloadAttackState = new ReaperReloadAttackState(this, attacks[ReaperAction.ReloadAttack]);
            CombatStateMachine.AddAnyTransition(NoAttackState, new FuncPredicate(() => !isAttacking));
            CombatStateMachine.AddTransition(NoAttackState, ChargingState, new ActionPredicate(ref _primaryAttackStart));
            CombatStateMachine.AddTransition(NoAttackState, ChargingState, new FuncPredicate(() => isPressingPrimaryAttack));
            CombatStateMachine.AddTransition(ChargingState, FastPrimaryAttackState,
                new FuncPredicate(() => !isPressingPrimaryAttack && HoldAttackTimer.IsRunning && ChargingState.CanBeCanceled()));
            CombatStateMachine.AddTransition(ChargingState, ChargedPrimaryAttackState, 
                new FuncPredicate(() => !isPressingPrimaryAttack && !HoldAttackTimer.IsRunning && ChargingState.CanBeCanceled()));
            CombatStateMachine.AddTransition(NoAttackState, SecondaryAttackState, new ActionPredicate(ref _secondaryAttackStart));
            CombatStateMachine.AddTransition(ChargingState, SecondaryAttackState, new ActionPredicate(ref _secondaryAttackStart));
            CombatStateMachine.AddTransition(NoAttackState, ReloadAttackState, new ActionPredicate(ref _reload));
            CombatStateMachine.SetState(NoAttackState);
            #endregion

            #region Inputs
            inputReader.PrimaryAttack += OnPrimaryAttack;
            inputReader.SecondaryAttack += OnSecondaryAttack;
            inputReader.Reload += OnReload;
            inputReader.Dash += OnDash;
            inputReader.EnablePlayerActions();
            #endregion
            
            HoldAttackTimer = new CountdownTimer(1f);
            HoldAttackTimer.OnTimerTick += OnHoldTick;
            _cam = Camera.main;
            EventBus<EnergyPickupEvent>.Register(new EventBinding<EnergyPickupEvent>(OnEnergyPickup));

        }

        private void OnEnergyPickup(EnergyPickupEvent @event)
        {
            switch (@event.Item.type)
            {
                case EnergyType.Health:
                    health.HealReceived(@event.Item.amount);
                    break;
                case EnergyType.Stamina:
                    stamina.Regenerate(@event.Item.amount);
                    break;
                case EnergyType.Blood:
                    blood.Add(@event.Item.amount);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnHoldTick()
        {
            _holdEvent.Progress = HoldAttackTimer.Progress;
            EventBus<AttackHoldEvent>.Raise(_holdEvent);
        }

        
        private void OnDash(ActionState action, IInputInteraction interaction)
        {
            switch (action)
            {
                case ActionState.Press:
                    _isPressingDash = true;
                    break;
                case ActionState.Hold:
                    break;
                case ActionState.Release:
                    _isPressingDash = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(action), action, null);
            }
        }

        private void OnPrimaryAttack(ActionState action, IInputInteraction interaction)
        {
            switch (action)
            {
                case ActionState.Press:
                    isPressingPrimaryAttack = true;
                    if (isAttacking) return;
                    _primaryAttackStart?.Invoke();
                    break;
                case ActionState.Hold:
                    break;
                case ActionState.Release:
                    isPressingPrimaryAttack = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(action), action, null);
            }
        }
        
        private void OnSecondaryAttack(ActionState action, IInputInteraction interaction)
        {
            switch (action)
            {
                case ActionState.Press:
                    if (blood.currentBloodPoints > 0)
                    {
                        _secondaryAttackStart?.Invoke();
                    }
                    else
                    {
                        // TODO: Throw something that tells us there is no blood
                    }
                    break;
                case ActionState.Hold:
                    break;
                case ActionState.Release:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(action), action, null);
            }
        }
        
        private void OnReload(ActionState action, IInputInteraction interaction)
        {
            switch (action)
            {
                case ActionState.Press:
                    isPressingReload = true;
                    _reload?.Invoke();
                    break;
                case ActionState.Hold:
                    break;
                case ActionState.Release:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(action), action, null);
            }
        }
        void Update()
        {
            CombatStateMachine.Update();
        }
        
        void FixedUpdate()
        {
            CombatStateMachine.FixedUpdate();
        }

        public void HandleSecondaryAttack()
        {
            var amountOfBullets = blood.Use();
            Dictionary<IVisitable, int> toDamage = new();
            for (var i = 0; i < amountOfBullets; i++)
            {
                Vector3 spreadDirection =
                    GenerateRandomDirection(_cam.transform.forward, spreadAngle);

                Vector3 endPoint;
                if (!Physics.Raycast(_cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f)), spreadDirection,
                        out var hit, 50))
                {
                    // If the raycast doesn't hit anything
                    // Debug.DrawRay(_cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f)), spreadDirection * 99,
                        //Color.blue, 10.0f);
                    endPoint = bulletSpawnTransform.position + spreadDirection * 50;
                    DrawBulletTrail(bulletSpawnTransform.position, endPoint);
                    continue;
                }
                //Debug.DrawRay(_cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f)), spreadDirection * hit.distance,
                    //Color.red, 10.0f);
                endPoint = hit.point;
                DrawBulletTrail(bulletSpawnTransform.position, endPoint);
                // If you hit something, stack damage
                if (!hit.transform.TryGetComponent<IVisitable>(out var visitable)) continue;
                if (toDamage.TryGetValue(visitable, out var amount))
                {
                    toDamage[visitable] = amount + 1;
                }
                else
                {
                    toDamage.Add(visitable, 1);
                }
            }

            foreach (var visitable in toDamage.Keys)
            {
                var damageComponent = attacks[ReaperAction.SecondaryAttack].damage;
                var start = damageComponent.damageAmount;
                damageComponent.damageAmount *= toDamage[visitable];
                damageComponent.Visit(visitable);
                damageComponent.damageAmount = start;
            }
        }

        Vector3 GenerateRandomDirection(Vector3 forward, float angle)
        {
            // Convert the spread angle to radians
            float spreadInRadians = angle * Mathf.Deg2Rad;

            // Generate random angles in spherical coordinates
            float randomPitch = Random.Range(-spreadInRadians, spreadInRadians);  // Vertical (pitch)
            float randomYaw = Random.Range(-spreadInRadians, spreadInRadians);    // Horizontal (yaw)

            // Apply the random pitch and yaw to the forward direction
            Quaternion rotation = Quaternion.Euler(randomPitch * Mathf.Rad2Deg, randomYaw * Mathf.Rad2Deg, 0);
            return rotation * forward;
        }
        
        void DrawBulletTrail(Vector3 start, Vector3 end)
        {
            var instance = (BulletTrail) FlyweightManager.Spawn(secondaryAttackSettings);
            instance.SetPosition(start, end);
        }
    }
    
    
    public enum ReaperAction
    {
        FastPrimaryAttack,
        ChargedPrimaryAttack,
        SecondaryAttack,
        ReloadAttack,
    }

    [System.Serializable]
    public struct ReaperAttack
    {
        public List<AnimationClip> animations;
        public Hitbox hitbox;
        [FormerlySerializedAs("damageComponent")] public Damage damage;
        public float startup, duration, recovery;
    }
    
}