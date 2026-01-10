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

            PlayAnim(AnimStateNameHash.Damaged);
            PlayerMain.StopMoving();
            PlayerMain.LockControl = true;

            ExitStateOnEnd();
        }

        public override void LeaveState()
        {
            base.LeaveState();
            PlayerMain.LockControl = false;
        }
    }
}
