using UnityEngine;

namespace Player.States
{
    public class PlayerIdleState : PlayerBaseState
    {
        public PlayerIdleState(PlayerController controller) : base(controller)
        {
        }

        public override void FixedUpdate()
        {
        }

        public override void OnEnter()
        {
            Debug.Log("Idle State");
        }

        public override void OnExit()
        {
        }

        public override void Update()
        {
            Controller.HandleGravity();
            Controller.HandleMovement();
        }

    }
}