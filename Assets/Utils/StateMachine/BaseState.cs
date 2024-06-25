using UnityEngine;

namespace Utils.StateMachine
{
    public abstract class BaseState : IState
    {
        protected readonly IEntityController Entity;
        protected readonly Animator Animator;
        protected BaseState(IEntityController entity, Animator animator)
        {
            this.Entity = entity;
            this.Animator = animator;
        }
        public virtual void OnEnter()
        {
            // noop
        }

        public virtual void Update()
        {
            // noop
        }

        public virtual void FixedUpdate()
        {
            // noop
        }

        public virtual void OnExit()
        {
            // noop
        }
    }
}