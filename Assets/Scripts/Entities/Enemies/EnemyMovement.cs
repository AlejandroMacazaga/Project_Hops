using System;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Utils.StateMachine;
using Utils.Timers;

namespace Entities.Enemies
{
    public class EnemyMovement : ValidatedMonoBehaviour
    {
        public GameObject prefab;  // The GameObject that has the NavMeshAgent
        [SerializeField] private NavMeshAgent navAgent;
        [SerializeField, HideInInspector, Self] private Rigidbody rb;

        public float movementSpeed = 5f;

        public StateMachine MovementStateMachine;
        public GroundedState GroundedState;
        public AirborneState AirborneState;
        public StunnedState StunnedState;

        private CountdownTimer _stunnedTimer;

        public Transform target;
        private void Start()
        {
            navAgent = Instantiate(prefab).GetComponent<NavMeshAgent>();
            navAgent.transform.position = transform.localPosition;
            navAgent.speed = movementSpeed;
            _stunnedTimer = new CountdownTimer(0f);
            
            
            MovementStateMachine = new StateMachine();

            GroundedState = new GroundedState();
            AirborneState = new AirborneState();
            StunnedState = new StunnedState();
            
            MovementStateMachine.AddTransition(GroundedState, AirborneState, new FuncPredicate(() => false));
            MovementStateMachine.AddTransition(GroundedState, StunnedState, new FuncPredicate(() => _stunnedTimer.IsRunning));
            MovementStateMachine.AddTransition(StunnedState, GroundedState,new FuncPredicate(() => _stunnedTimer.IsFinished()));
            
            MovementStateMachine.SetState(GroundedState);
        }

        void Update() => MovementStateMachine.Update();
        void FixedUpdate()
        {
            // if (navAgent.isOnNavMesh) Debug.Log("Is in nav mesh");
            if (target && !(MovementStateMachine.CurrentState is StunnedState))
                navAgent.SetDestination(target.position);
            else navAgent.SetDestination(transform.position);
            
            Vector3 targetPosition = navAgent.transform.position;
            
            Vector3 direction = (targetPosition - transform.position).normalized;


            if (MovementStateMachine.CurrentState is GroundedState)
            {
                rb.MovePosition(transform.position + direction * (movementSpeed * Time.fixedDeltaTime));
            }

            if (MovementStateMachine.CurrentState is StunnedState)
            {
                rb.velocity = Vector3.zero;
            }
        }

        public void Push(Vector3 force)
        {
            Debug.Log(force);
            rb.AddForce(force, ForceMode.Force);
        }

        public void Stun(float time)
        {
            _stunnedTimer.InitialTime = time;
            if (_stunnedTimer.IsRunning) _stunnedTimer.Reset();
            else _stunnedTimer.Start();
        }
    }
}