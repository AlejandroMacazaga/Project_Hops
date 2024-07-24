using UnityEngine;

namespace Player.States
{
    public class PlayerOnAirState : PlayerBaseState
    {
        private readonly StatModifier _gravityModifier;
        private const float GravityMultiplier = 3f;
        private AnimationClip _animation;
        public PlayerOnAirState(PlayerController controller, AnimationClip animation) : base(controller)
        {
            this._animation = animation;
            _gravityModifier = new StatModifier(StatModifier.ModifierType.Percent, GravityMultiplier);
        }

        public override void FixedUpdate()
        {
        }

        public override void OnEnter()
        {
            Debug.Log("We on the air");
            Controller.PlayerStats.AddModifier("Gravity", _gravityModifier);
            //controller.AnimationSystem.PlayOneShot(animation, true);
        }

        public override void OnExit()
        {
            Debug.Log("We off the air");
            Controller.PlayerStats.RemoveModifier("Gravity", _gravityModifier);
        }

        public override void Update()
        {

        }
    }
}