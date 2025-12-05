using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;


namespace PahlBit
{
    public class PlayerStateGeneral : PlayerStateBase
    {
        [AnimatorStateHash]
        public int StateNameHash = 0;

        [Foldout("Events")]
        public UnityEvent<int> OnFireIndex = new UnityEvent<int>();

        private Action mEventFire;
        private Action<int> mEventFireIdx;

        public override void EnterState(object param)
        {
            base.EnterState(param);

            if (param is Action action)
                mEventFire = action;
            else if (param is Action<int> actionIdx)
                mEventFireIdx = actionIdx;

            Base.Phy.Velocity = Vector2.zero;
            Base.Phy.LockGravity = true;

            PlayAnim(StateNameHash);

            AddEventMiddle(StateNameHash, InvokeEvent);

            ExitStateOnEnd();
        }

        void InvokeEvent(int fireIndex)
        {
            OnFireIndex?.Invoke(fireIndex);

            mEventFire?.Invoke();
            mEventFireIdx?.Invoke(fireIndex);
        }

        public override void LeaveState()
        {
            base.LeaveState();
            Base.Phy.LockGravity = false;
            mEventFire = null;
            mEventFireIdx = null;
        }
    }
}
