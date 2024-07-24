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
            Debug.Log("We on the idle state");
        }

        public override void OnExit()
        {
            Debug.Log("We off the idle state");
        }

        public override void Update()
        {
            
        }

    }
}