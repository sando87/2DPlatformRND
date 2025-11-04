using System;
using System.Collections.Generic;
using NaughtyAttributes;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;


namespace PahlBit
{
    public class FiniteStateBase : MonoBehaviour
    {
        [SerializeField][Range(0, 10)] int _Layer = 0;
        [SerializeField][Range(0, 10)] int _Priority = 0;

        public int Layer { get { return _Layer; } }
        public int Priority { get { return _Priority; } }
        
        public BaseObject Base { get; private set; }

        [Foldout("Events")]
        public UnityEvent EventEnter = new UnityEvent();
        [Foldout("Events")]
        public UnityEvent EventLeave = new UnityEvent();

        private Dictionary<int, UnityAction> mEventsEnter = new Dictionary<int, UnityAction>();
        private Dictionary<int, UnityAction<int>> mEventsMiddle = new Dictionary<int, UnityAction<int>>();
        private Dictionary<int, UnityAction> mEventsLeave = new Dictionary<int, UnityAction>();

        public virtual void InitState()
        {
            Base = GetComponentInParent<BaseObject>();
        }

        public virtual void HandleInput()
        {
            // 입력 처리 코드
        }

        public virtual void EnterState(object param)
        {
            // 상태에 진입할 때 실행되는 코드
            EventEnter?.Invoke();
        }
        public virtual void UpdateState()
        {
            // 매 프레임마다 실행되는 코드
        }
        public virtual void FixedUpdateState()
        {
            // 매 프레임마다 실행되는 코드
        }
        public virtual void LeaveState()
        {
            // 상태에서 벗어날 때 실행되는 코드
            RemoveAllEvents();
            StopAllCoroutines();
            EventLeave?.Invoke();
        }

        protected void ChangeStateToIdle()
        {
            Base.StateMachine.ChangeStateToIdle(Layer);
        }
        protected void ChangeStateToThis()
        {
            Base.StateMachine.ChangeState(this);
        }
        protected void PlayAnim(string stateName)
        {
            Base.AnimHelper.CrossFadeToState(stateName, Layer);
        }
        protected void PlayAnim(AnimStateNameHash stateHashName)
        {
            Base.AnimHelper.CrossFadeToState(stateHashName, Layer);
        }
        protected void PlayAnimWithFire(AnimStateNameHash stateHashName, UnityAction<int> onFired)
        {
            Base.AnimHelper.CrossFadeToState(stateHashName, Layer);

            AddEventMiddle(stateHashName, onFired);
        }
        protected bool IsCurrentThisState()
        {
            return Base.StateMachine.GetCurrentState(Layer) == this;
        }

        public void AddEventEnter(AnimStateNameHash stateHash, UnityAction handler)
        {
            if (mEventsEnter.ContainsKey(stateHash))
                return;
                
            mEventsEnter[stateHash] = () => { if (IsCurrentThisState()) handler.Invoke(); };
            Base.AnimHelper.AddEventEnter(stateHash, mEventsEnter[stateHash]);
        }
        public void RemoveEventEnter(AnimStateNameHash stateHash)
        {
            if (!mEventsEnter.ContainsKey(stateHash))
                return;

            Base.AnimHelper.RemoveEventEnter(stateHash, mEventsEnter[stateHash]);
            mEventsEnter.Remove(stateHash);
        }
        public void AddEventMiddle(AnimStateNameHash stateHash, UnityAction<int> handler)
        {
            if (mEventsMiddle.ContainsKey(stateHash))
                return;
                
            mEventsMiddle[stateHash] = (index) => { if (IsCurrentThisState()) handler.Invoke(index); };
            Base.AnimHelper.AddEventMiddle(stateHash, mEventsMiddle[stateHash]);
        }
        public void RemoveEventMiddle(AnimStateNameHash stateHash)
        {
            if (!mEventsMiddle.ContainsKey(stateHash))
                return;

            Base.AnimHelper.RemoveEventMiddle(stateHash, mEventsMiddle[stateHash]);
            mEventsMiddle.Remove(stateHash);
        }
        public void AddEventLeave(AnimStateNameHash stateHash, UnityAction handler)
        {
            if (mEventsLeave.ContainsKey(stateHash))
                return;
                
            mEventsLeave[stateHash] = () => { if (IsCurrentThisState()) handler.Invoke(); };
            Base.AnimHelper.AddEventLeave(stateHash, mEventsLeave[stateHash]);
        }
        public void RemoveEventLeave(AnimStateNameHash stateHash)
        {
            if (!mEventsLeave.ContainsKey(stateHash))
                return;

            Base.AnimHelper.RemoveEventLeave(stateHash, mEventsLeave[stateHash]);
            mEventsLeave.Remove(stateHash);
        }

        public void RemoveAllEvents()
        {
            foreach (var handler in mEventsEnter)
            {
                Base.AnimHelper.RemoveEventEnter(handler.Key, handler.Value);
            }
            mEventsEnter.Clear();
        }
    }
}
