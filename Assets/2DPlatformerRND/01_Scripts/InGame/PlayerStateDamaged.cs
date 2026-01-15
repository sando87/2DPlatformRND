using UnityEngine;


namespace PahlBit
{
    public class PlayerStateDamaged : PlayerStateBase
    {
        public override void InitState()
        {
            base.InitState();

            Base.Health.OnDamaged.AddListener(() =>
            {
                ChangeStateToThis();
            });
        }

        public override void EnterState(object param)
        {
            base.EnterState(param);

            Base.Phy.StopMoving();
            Base.Ctrl.LockAll = true;

            PlayAnim(AnimStateNameHash.Damaged)
            .OnEnd(DoLeaveCurrentState);
        }

        public override void LeaveState()
        {
            base.LeaveState();
            Base.Ctrl.LockAll = false;
        }
    }
}
