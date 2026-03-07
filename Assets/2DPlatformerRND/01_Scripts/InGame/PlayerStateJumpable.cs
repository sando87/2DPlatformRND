using UnityEngine;


namespace PahlBit
{
    public class PlayerStateJumpable : PlayerStateBase
    {
        public float jumpForce = 25f;

        public override void HandleInput()
        {
            if (PlayerInput.JustPressed(PlayerUnitInputType.Jump))
            {
                if (GetCurrentState().IsStateCancelable)
                    Base.StateMachine.TryChangeState(this, null, true);
                else
                    ChangeStateToThis();
            }
        }

        public override void EnterState(object param)
        {
            base.EnterState(param);

            Base.Phy.DoJump(jumpForce);
            Base.StateMachine.TryChangeState<PlayerStateFloating>(null, true);
        }

    }
}
