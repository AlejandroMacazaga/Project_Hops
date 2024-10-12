using Utils.StateMachine;

namespace Player.KinematicController {
    public class GroundedState : IState {
        readonly KinematicPlayerController _controller;

        public GroundedState(KinematicPlayerController controller) {
            this._controller = controller;
        }

        public void OnEnter() {
            _controller.OnGroundContactRegained();
        }

        public void Update()
        {
            throw new System.NotImplementedException();
        }

        public void FixedUpdate()
        {
            throw new System.NotImplementedException();
        }

        public void OnExit()
        {
            throw new System.NotImplementedException();
        }
    }

    public class FallingState : IState {
        readonly KinematicPlayerController _controller;

        public FallingState(KinematicPlayerController controller) {
            this._controller = controller;
        }

        public void OnEnter() {
            _controller.OnFallStart();
        }

        public void Update()
        {
            throw new System.NotImplementedException();
        }

        public void FixedUpdate()
        {
            throw new System.NotImplementedException();
        }

        public void OnExit()
        {
            throw new System.NotImplementedException();
        }
    }

    public class SlidingState : IState {
        readonly KinematicPlayerController controller;

        public SlidingState(KinematicPlayerController controller) {
            this.controller = controller;
        }

        public void OnEnter() {
            controller.OnGroundContactLost();
        }

        public void Update()
        {
            throw new System.NotImplementedException();
        }

        public void FixedUpdate()
        {
            throw new System.NotImplementedException();
        }

        public void OnExit()
        {
            throw new System.NotImplementedException();
        }
    }

    public class RisingState : IState {
        readonly KinematicPlayerController controller;
        private IState _stateImplementation;

        public RisingState(KinematicPlayerController controller) {
            this.controller = controller;
        }

        public void OnEnter() {
            controller.OnGroundContactLost();
        }

        public void Update()
        {
            throw new System.NotImplementedException();
        }

        public void FixedUpdate()
        {
            throw new System.NotImplementedException();
        }

        public void OnExit()
        {
            throw new System.NotImplementedException();
        }
    }

    public class JumpingState : IState {
        readonly KinematicPlayerController controller;

        public JumpingState(KinematicPlayerController controller) {
            this.controller = controller;
        }

        public void OnEnter() {
            controller.OnGroundContactLost();
            controller.OnJumpStart();
        }

        public void Update()
        {
            throw new System.NotImplementedException();
        }

        public void FixedUpdate()
        {
            throw new System.NotImplementedException();
        }

        public void OnExit()
        {
            throw new System.NotImplementedException();
        }
    }
}