using Utils.StateMachine;
using UnityEngine;



namespace Assets.Scripts.Entities
{
    public abstract class BaseEntityState : IState
    {
        public readonly IEntityController Controller;

        public BaseEntityState(IEntityController controller)
        {
            Controller = controller;
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

