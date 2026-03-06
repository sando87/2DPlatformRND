using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;


namespace PahlBit
{
    public class PlayerStateCombo : PlayerStateBase
    {
        [AnimatorStateHash]
        public int[] StateNameHashes = null;

        [Foldout("Events")]
        public UnityEvent<int> OnFireIndex = new UnityEvent<int>();

        private Action<int> mEventFireIdx;
        private int ComboIndex = 0;

        public void DoNextCombo()
        {
            Base.AnimHelper.SetParamBool(AnimatorParams.DoNextCombo, true);
        }

        public override void EnterState(object param)
        {
            base.EnterState(param);

            if (param is Action<int> actionIdx)
                mEventFireIdx = actionIdx;

            Base.Phy.Velocity = Vector2.zero;
            Base.Phy.LockGravity = true;
            Base.Ctrl.LockMove = true;

            foreach (int stateHash in StateNameHashes)
            {
                AddEventMiddle(stateHash, InvokeFireEvent);
                AddEventLeave(stateHash, OnLeaveState);
            }

            PlayAnim(StateNameHashes[0]);
            ComboIndex = 0;
        }

        void InvokeFireEvent(int fireIndex)
        {
            OnFireIndex?.Invoke(fireIndex);
            mEventFireIdx?.Invoke(fireIndex);
        }

        void OnLeaveState()
        {
            bool isNextCombo = Base.AnimHelper.GetParamBool(AnimatorParams.DoNextCombo);
            if (isNextCombo && ComboIndex < StateNameHashes.Length - 1)
            {
                ComboIndex++;
                Base.AnimHelper.SetParamBool(AnimatorParams.DoNextCombo, false);
            }
            else
            {
                DoLeaveCurrentState();
            }
        }

        public override void LeaveState()
        {
            base.LeaveState();
            Base.AnimHelper.SetParamBool(AnimatorParams.DoNextCombo, false);
            Base.Phy.LockGravity = false;
            Base.Ctrl.LockMove = false;
            mEventFireIdx = null;
            ComboIndex = 0;
        }
    }
}
