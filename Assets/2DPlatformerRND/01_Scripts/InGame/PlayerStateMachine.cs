using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace PahlBit
{
    public class PlayerStateMachine : MonoBehaviour
    {
        private List<PlayerStateBase> mAllStates = new List<PlayerStateBase>();
        public PlayerStateBase PreviousState { get; private set; } = null;
        private PlayerStateBase mCurrentState = null;
        private PlayerStateIdle mIdleState = null;

        void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            PlayerStateBase[] states = GetComponentsInChildren<PlayerStateBase>();
            foreach (var state in states)
            {
                state.InitState();
                mAllStates.Add(state);

                if (state is PlayerStateIdle)
                {
                    mIdleState = state as PlayerStateIdle;
                }
            }
        }

        void Start()
        {
            mCurrentState = mIdleState;
            mCurrentState.EnterState(null);
        }

        public void ChangeState(PlayerStateBase newState, object param = null)
        {
            if (newState.Priority < mCurrentState.Priority)
            {
                return;
            }

            // 상태 전환 로직 구현
            mCurrentState.LeaveState();
            PreviousState = mCurrentState;
            mCurrentState = newState;
            mCurrentState.EnterState(param);
        }
        
        public void ChangeStateToIdle()
        {
            // 상태 전환 로직 구현
            mCurrentState.LeaveState();
            PreviousState = mCurrentState;
            mCurrentState = mIdleState;
            mCurrentState.EnterState(null);
        }

        public void HandleAllStateInput()
        {
            foreach (var state in mAllStates)
            {
                if (state != mCurrentState)
                {
                    state.HandleInput();
                }
            }
        }
        public void UpdateState()
        {
            mCurrentState.UpdateState();
        }
        public void FixedUpdateState()
        {
            mCurrentState.FixedUpdateState();
        }
    }
}