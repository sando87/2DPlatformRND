using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace PahlBit
{
    public class FiniteStateMachine : MonoBehaviour
    {
        public FiniteStateBase CurrentStateForDebug = null;

        private Dictionary<int, StateMachineLayer> mLayers = new Dictionary<int, StateMachineLayer>();

        void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            FiniteStateBase[] states = GetComponentsInChildren<FiniteStateBase>();
            foreach (var state in states)
            {
                state.InitState();

                if (!mLayers.ContainsKey(state.Layer))
                {
                    mLayers[state.Layer] = new StateMachineLayer();
                }

                mLayers[state.Layer].AllStates.Add(state);

                if (state is PlayerStateIdle || state is PlayerStateUpperIdle)
                {
                    mLayers[state.Layer].IdleState = state;
                }
            }
        }

        void Start()
        {
            foreach (var layerSet in mLayers)
            {
                int layer = layerSet.Key;
                mLayers[layer].CurrentState = mLayers[layer].IdleState;
                mLayers[layer].CurrentState.EnterState(null);
                
                if (layer == 0)
                {
                    CurrentStateForDebug = mLayers[layer].CurrentState;
                }
            }
        }

        public void ChangeState(FiniteStateBase newState, object param = null, bool ignorePriority = false)
        {
            StateMachineLayer currentLayer = mLayers[newState.Layer];
            if (currentLayer.CurrentState == newState)
                return;

            if(!ignorePriority)
            {
                if (newState.Priority < currentLayer.CurrentState.Priority)
                    return;
            }

            // 상태 전환 로직 구현
            currentLayer.CurrentState.LeaveState();
            currentLayer.PreviousState = currentLayer.CurrentState;
            currentLayer.CurrentState = newState;
            currentLayer.CurrentState.EnterState(param);

            if (newState.Layer == 0)
            {
                CurrentStateForDebug = newState;
            }
        }

        public void ChangeState<T>(object param = null, bool ignorePriority = false) where T : FiniteStateBase
        {
            FiniteStateBase state = FindState<T>();
            if(state != null)
            {
                ChangeState(state, param, ignorePriority);
            }
        }

        public void ChangeStateToIdle(int layerIndex)
        {
            StateMachineLayer currentLayer = mLayers[layerIndex];
            if (currentLayer.CurrentState == currentLayer.IdleState)
                return;

            currentLayer.CurrentState.LeaveState();
            currentLayer.PreviousState = currentLayer.CurrentState;
            currentLayer.CurrentState = currentLayer.IdleState;
            currentLayer.CurrentState.EnterState(null);

            if (layerIndex == 0)
            {
                CurrentStateForDebug = currentLayer.CurrentState;
            }
        }

        public T FindState<T>() where T : FiniteStateBase
        {
            return GetComponentInChildren<T>();
        }

        public FiniteStateBase GetCurrentState(int layerIndex)
        {
            if (mLayers.ContainsKey(layerIndex))
            {
                return mLayers[layerIndex].CurrentState;
            }
            return null;
        }

        public void HandleAllStateInput()
        {
            foreach (var layer in mLayers)
            {
                foreach (var state in layer.Value.AllStates)
                {
                    if (layer.Value.CurrentState != state)
                        state.HandleInput();
                }
            }
        }
        public void UpdateState()
        {
            foreach (var layer in mLayers)
            {
                if (layer.Value.CurrentState != null)
                {
                    layer.Value.CurrentState.UpdateState();
                }
            }
        }
        public void FixedUpdateState()
        {
            foreach (var layer in mLayers)
            {
                if (layer.Value.CurrentState != null)
                {
                    layer.Value.CurrentState.FixedUpdateState();
                }
            }
        }
    }

    [System.Serializable]
    public class StateMachineLayer
    {
        public List<FiniteStateBase> AllStates = new List<FiniteStateBase>();
        public FiniteStateBase PreviousState = null;
        public FiniteStateBase CurrentState = null;
        public FiniteStateBase IdleState = null;
    }
}