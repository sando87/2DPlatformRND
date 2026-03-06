using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;


namespace PahlBit
{
    public class PlayerStateAttackLoop : PlayerStateBase
    {
        [AnimatorStateHash]
        public int StateLoopStart = 0;
        [AnimatorStateHash]
        public int StateLoop = 0;
        [AnimatorStateHash]
        public int StateLoopEnd = 0;

        private Action mEventFire;

        public void StopAttack()
        {
            Base.AnimHelper.SetParamBool(AnimatorParams.StopLoop, true);
        }

        public override void EnterState(object param)
        {
            base.EnterState(param);

            if (param is Action actionIdx)
                mEventFire = actionIdx;

            Base.Phy.Velocity = Vector2.zero;
            Base.Phy.LockGravity = true;
            Base.Ctrl.LockMove = true;

            Base.AnimHelper.SetParamBool(AnimatorParams.StopLoop, false);
            PlayAnim(StateLoopStart);
            AddEventEnter(StateLoop, OnLoopStart);
            AddEventLeave(StateLoopEnd, OnLeaveState);
        }

        void OnLoopStart()
        {
            mEventFire?.Invoke();
        }

        void OnLeaveState()
        {
            DoLeaveCurrentState();
        }

        public override void LeaveState()
        {
            base.LeaveState();
            Base.AnimHelper.SetParamBool(AnimatorParams.StopLoop, false);
            Base.Phy.LockGravity = false;
            Base.Ctrl.LockMove = false;
            mEventFire = null;
        }
    }
}
