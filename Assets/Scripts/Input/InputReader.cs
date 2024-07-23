using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Input
{
    [CreateAssetMenu(fileName = "InputReader", menuName = "FPS/InputReader")]
    public class InputReader : ScriptableObject, PlayerInputActions.IPlayerActions {
        public event UnityAction<Vector2> Move = delegate { };
        public event UnityAction<Vector2, bool> Look = delegate { };
        public event UnityAction<bool> Jump = delegate { };
        public event UnityAction<bool> Dash = delegate { };
        public event UnityAction LightAttack = delegate { };
        public event UnityAction HeavyAttack = delegate { };

        PlayerInputActions _inputActions;
        
        public Vector3 Direction => _inputActions.Player.Move.ReadValue<Vector2>();

        void OnEnable()
        {
            if (_inputActions != null) return;
            _inputActions = new PlayerInputActions();
            _inputActions.Player.SetCallbacks(this);
        }
        
        public void EnablePlayerActions() {
            _inputActions.Enable();
        }

        public void OnMove(InputAction.CallbackContext context) {
            Move.Invoke(context.ReadValue<Vector2>());
        }

        public void OnLook(InputAction.CallbackContext context) {
            Look.Invoke(context.ReadValue<Vector2>(), IsDeviceMouse(context));
        }

        bool IsDeviceMouse(InputAction.CallbackContext context) => context.control.device.name == "Mouse";

        public void OnLightAttack(InputAction.CallbackContext context) {
            if (context.phase == InputActionPhase.Started) {
                LightAttack.Invoke();
            }
        }

        public void OnHeavyAttack(InputAction.CallbackContext context) {
            if (context.phase == InputActionPhase.Started) {
                HeavyAttack.Invoke();
            }
        }

        public void OnDash(InputAction.CallbackContext context) {
            switch (context.phase) {
                case InputActionPhase.Started:
                    Dash.Invoke(true);
                    break;
                case InputActionPhase.Canceled:
                    Dash.Invoke(false);
                    break;
            }
        }

        public void OnJump(InputAction.CallbackContext context) {
            switch (context.phase) {
                case InputActionPhase.Started:
                    Jump.Invoke(true);
                    break;
                case InputActionPhase.Canceled:
                    Jump.Invoke(false);
                    break;
            }
        }


    }
}