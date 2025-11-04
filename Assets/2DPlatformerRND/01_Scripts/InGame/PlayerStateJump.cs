using UnityEngine;


namespace PahlBit
{
    public class PlayerStateJump : PlayerStateBase
    {
        public float moveSpeed = 7f;
        public float jumpForce = 14f;

        public override void HandleInput()
        {
            if (PlayerInput.JustPressed(PlayerUnitInputType.Jump) && PlayerMain.IsGrounded)
            {
                Base.StateMachine.ChangeState(this);
            }
        }

        public override void EnterState(object param)
        {
            base.EnterState(param);

            PlayerMain.DoJump(jumpForce);
        }

        public override void UpdateState()
        {
            base.UpdateState();

            Vector2 moveInput = PlayerInput.GetInputValue<Vector2>(PlayerUnitInputType.Move);
            float moveX = moveInput.x * moveSpeed;
            PlayerMain.MoveHorizontally(moveX);
        }

    }
}
