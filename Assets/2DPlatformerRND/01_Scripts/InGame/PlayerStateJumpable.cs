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
                ChangeStateToThis();
            }
        }

        public override void EnterState(object param)
        {
            base.EnterState(param);
            
            PlayerMain.DoJump(jumpForce);
            Base.StateMachine.ChangeState<PlayerStateFloating>(null, true);
        }

    }
}
