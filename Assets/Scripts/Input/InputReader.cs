using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.Serialization;

namespace Input
{
    [CreateAssetMenu(fileName = "InputReader", menuName = "FPS/InputReader")]
    public class InputReader : ScriptableObject, PlayerInputActions.IPlayerActions {
        public event UnityAction<Vector2> Move = delegate { };
        public event UnityAction<Vector2, bool> Look = delegate { };
        public event UnityAction<ActionState, IInputInteraction> Jump = delegate { };
        public event UnityAction<ActionState, IInputInteraction> Dash = delegate { };
        public event UnityAction<ActionState, IInputInteraction> PrimaryAttack = delegate { };
        public event UnityAction<ActionState, IInputInteraction> SecondaryAttack = delegate { };
        public event UnityAction<ActionState, IInputInteraction> Reload = delegate { };
        public event UnityAction<ActionState, IInputInteraction> Interact = delegate { };
        
        public SerializedDictionary<PlayerAction, bool> hasStarted = new();

        PlayerInputActions _inputActions;
        
        public Vector3 Direction => _inputActions.Player.Move.ReadValue<Vector2>();

        public Vector2 CameraLook => _inputActions.Player.Look.ReadValue<Vector2>();
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

        public void OnPrimaryAttack(InputAction.CallbackContext context)
        {
            if (context.canceled)
            {
                hasStarted[PlayerAction.PrimaryAttack] = false;
                PrimaryAttack.Invoke(ActionState.Release, context.interaction);
                return;
            }
            if (hasStarted[PlayerAction.PrimaryAttack])
            {
                PrimaryAttack.Invoke(ActionState.Hold, context.interaction);
            }
            else
            {
                hasStarted[PlayerAction.PrimaryAttack] = true;
                PrimaryAttack.Invoke(ActionState.Press, context.interaction);
            }
        }

        public void OnSecondaryAttack(InputAction.CallbackContext context) {
            if (context.canceled)
            {
                hasStarted[PlayerAction.SecondaryAttack] = false;
                SecondaryAttack.Invoke(ActionState.Release, context.interaction);
                return;
            }
            if (hasStarted[PlayerAction.SecondaryAttack])
            {
                SecondaryAttack.Invoke(ActionState.Hold, context.interaction);
            }
            else
            {
                hasStarted[PlayerAction.SecondaryAttack] = true;
                SecondaryAttack.Invoke(ActionState.Press, context.interaction);
            }
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (context.canceled)
            {
                hasStarted[PlayerAction.Interact] = false;
                Interact.Invoke(ActionState.Release, context.interaction);
                return;
            }
            if (hasStarted[PlayerAction.Interact])
            {
                Interact.Invoke(ActionState.Hold, context.interaction);
            }
            else
            {
                hasStarted[PlayerAction.Interact] = true;
                Interact.Invoke(ActionState.Press, context.interaction);
            }
        }

        public void OnDash(InputAction.CallbackContext context) {
            if (context.canceled)
            {
                hasStarted[PlayerAction.Dash] = false;
                Dash.Invoke(ActionState.Release, context.interaction);
                return;
            }
            if (hasStarted[PlayerAction.Dash])
            {
                Dash.Invoke(ActionState.Hold, context.interaction);
            }
            else
            {
                hasStarted[PlayerAction.Dash] = true;
                Dash.Invoke(ActionState.Press, context.interaction);
            }
        }

        public void OnJump(InputAction.CallbackContext context) {
            if (context.canceled)
            {
                hasStarted[PlayerAction.Jump] = false;
                Jump.Invoke(ActionState.Release, context.interaction);
                return;
            }
            if (hasStarted[PlayerAction.Jump])
            {
                Jump.Invoke(ActionState.Hold, context.interaction);
            }
            else
            {
                hasStarted[PlayerAction.Jump] = true;
                Jump.Invoke(ActionState.Press, context.interaction);
            }
        }

        public void OnReload(InputAction.CallbackContext context)
        {
            if (context.canceled)
            {
                hasStarted[PlayerAction.Reload] = false;
                Reload.Invoke(ActionState.Release, context.interaction);
                return;
            }
            if (hasStarted[PlayerAction.Reload])
            {
                Reload.Invoke(ActionState.Hold, context.interaction);
            }
            else
            {
                hasStarted[PlayerAction.Reload] = true;
                Reload.Invoke(ActionState.Press, context.interaction);
            }
        }

    }

    public enum PlayerAction
    {
        Jump,
        Reload,
        Dash,
        Interact,
        SecondaryAttack,
        PrimaryAttack
    }

    public enum ActionState
    {
        Press,
        Hold,
        Release
    }
}