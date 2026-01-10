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
            PlayerMain.StopMoving();
            PlayerMain.LockControl = true;
        }

        public override void LeaveState()
        {
            base.LeaveState();
            PlayerMain.LockControl = false;
        }
    }
}
