using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace PahlBit
{
    public enum PlayerUnitInputType { None, Jump, Move, Attack, Skill, Dash }

    public class PlayerUnitInput : MonoBehaviour
    {
        private PlayerInputActions mInputActions;
        private Dictionary<PlayerUnitInputType, PlayerUnitInputState> mInputStates = new Dictionary<PlayerUnitInputType, PlayerUnitInputState>();

        public bool JustPressed(PlayerUnitInputType type) { return GetInputAction(type).triggered; }
        public bool IsPressing(PlayerUnitInputType type) { return GetInputAction(type).IsPressed(); }

        // public bool JustPressed(PlayerUnitInputType type) { return mInputStates[type].justPressed; }
        // public bool IsPressing(PlayerUnitInputType type) { return mInputStates[type].isPressed; }
        // public float HeldTime(PlayerUnitInputType type) { return mInputStates[type].HeldTime; }

        public TValue GetInputValue<TValue>(PlayerUnitInputType type) where TValue : struct
        {
            InputAction action = mInputStates[type].inputAction;
            if (action != null)
                return action.ReadValue<TValue>();

            return default(TValue);
        }

        public UnityEvent<PlayerUnitInputType> EnterInput = new UnityEvent<PlayerUnitInputType>();
        public UnityEvent<PlayerUnitInputType> LeaveInput = new UnityEvent<PlayerUnitInputType>();

        private void Awake()
        {
            mInputActions = new PlayerInputActions();

            InitEnumKeys();
        }

        void InitEnumKeys()
        {
            foreach (PlayerUnitInputType type in MyUtils.EnumForeach<PlayerUnitInputType>())
            {
                if (type == PlayerUnitInputType.None)
                    continue;

                mInputStates[type] = new PlayerUnitInputState();
                mInputStates[type].inputAction = GetInputAction(type);

                mInputStates[type].inputAction.started += ctx => OnStarted(type);
                mInputStates[type].inputAction.performed += ctx => OnPerformed(type);
                mInputStates[type].inputAction.canceled += ctx => OnCanceled(type);
            }

        }

        private void OnStarted(PlayerUnitInputType type)
        {
            // EnterInput.Invoke(type);

            // mInputStates[type].isPressed = true;
        }
        private void OnPerformed(PlayerUnitInputType type)
        {
            // UpdateInput.Invoke(type);
            mInputStates[type].isPressedPreState = mInputStates[type].isPressed;
            mInputStates[type].isPressed = true;

            if (mInputStates[type].justPressed)
            {
                mInputStates[type].pressedTime = Time.time;
                EnterInput.Invoke(type);
            }
        }
        private void OnCanceled(PlayerUnitInputType type)
        {
            mInputStates[type].isPressedPreState = false;
            mInputStates[type].isPressed = false;
            mInputStates[type].pressedTime = 0;

            LeaveInput.Invoke(type);
        }

        private void OnEnable() => mInputActions.Enable();
        private void OnDisable() => mInputActions.Disable();

        private InputAction GetInputAction(PlayerUnitInputType type)
        {
            switch (type)
            {
                case PlayerUnitInputType.Jump:
                    return mInputActions.Player.Jump;
                case PlayerUnitInputType.Move:
                    return mInputActions.Player.Move;
                case PlayerUnitInputType.Attack:
                    return mInputActions.Player.Attack;
                case PlayerUnitInputType.Skill:
                    return mInputActions.Player.Skill;
                case PlayerUnitInputType.Dash:
                    return mInputActions.Player.Dash;
                default:
                    return null;
            }
        }
    }

    public class PlayerUnitInputState
    {
        public bool isPressed = false;
        public bool isPressedPreState = false;
        public float pressedTime = 0f;
        public InputAction inputAction = null;

        public bool justPressed => isPressed && !isPressedPreState;
        public float HeldTime => isPressed ? Time.time - pressedTime : 0f;
    }
}