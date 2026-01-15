using UnityEngine;


namespace PahlBit
{
    public class PlayerStateDeath : PlayerStateBase
    {
        public override void InitState()
        {
            base.InitState();

            Base.Health.OnDied.AddListener(() =>
            {
                ChangeStateToThis();
            });
        }

        public override void EnterState(object param)
        {
            base.EnterState(param);

            PlayAnim(AnimStateNameHash.Death);
            Base.Phy.StopMoving();
            Base.Ctrl.LockAll = true;
        }

        public override void LeaveState()
        {
            base.LeaveState();
            Base.Ctrl.LockAll = false;
        }
    }
}
