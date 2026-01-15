using UnityEngine;


namespace PahlBit
{
    public class PlayerStateDash : PlayerStateBase
    {
        [SerializeField] float _dashForce = 20f;

        public override void EnterState(object param)
        {
            base.EnterState(param);
            Base.Ctrl.LockMove = true;
            Base.Phy.LockGravity = true;

            DoDash();
        }

        private void DoDash()
        {
            Base.Phy.TurnToInput(Base.Input.MoveX);
            Base.Phy.Velocity = new Vector2(transform.right.x * _dashForce, 0f);

            PlayAnim(AnimStateNameHash.Dash)
            .OnEnd(DoLeaveCurrentState);
        }

        public override void LeaveState()
        {
            base.LeaveState();
            Base.Phy.LockGravity = false;
            Base.Ctrl.LockMove = false;
        }
    }
}
