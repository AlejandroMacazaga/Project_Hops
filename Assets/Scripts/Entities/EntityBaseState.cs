using System.Collections;
using System.Collections.Generic;
using Utils.StateMachine;
using UnityEngine;
using Assets.Scripts.Entities;


namespace Assets.Scripts.Entities
{
    public abstract class EntityBaseState : IState
    {
        public readonly IEntityController Controller;
        public readonly Animator Animator;

        public EntityBaseState(IEntityController controller, Animator animator)
        {
            Controller = controller;
            Animator = animator;
        }

        public virtual void FixedUpdate()
        {

        }

        public virtual void OnEnter()
        {

        }

        public virtual void OnExit()
        {

        }

        public virtual void Update()
        {
        }
    }
}

