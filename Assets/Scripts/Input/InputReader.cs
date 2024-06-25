using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static PlayerInputActions;

namespace Platformer {
    [CreateAssetMenu(fileName = "InputReader", menuName = "FPS/InputReader")]
    public class InputReader : ScriptableObject, IPlayerActions {
        public event UnityAction<Vector2> Move = delegate { };
        public event UnityAction<Vector2, bool> Look = delegate { };
        public event UnityAction<bool> Jump = delegate { };
        public event UnityAction<bool> Dash = delegate { };
        public event UnityAction LightAttack = delegate { };
        public event UnityAction HeavyAttack = delegate { };

        PlayerInputActions inputActions;
        
        public Vector3 Direction => inputActions.Player.Move.ReadValue<Vector2>();

        void OnEnable() {
            if (inputActions == null) {
                inputActions = new PlayerInputActions();
                inputActions.Player.SetCallbacks(this);
            }
        }
        
        public void EnablePlayerActions() {
            inputActions.Enable();
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