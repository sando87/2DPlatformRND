using UnityEngine;


namespace PahlBit
{
    public class PlayerStateJump : PlayerStateBase
    {
        public float moveSpeed = 7f;
        public float jumpForce = 14f;

        public override void HandleInput()
        {
            if (Base.PlayerInput.JustPressed(PlayerUnitInputType.Jump) && Base.PlayerCTRL.IsGrounded)
            {
                Base.StateMachine.ChangeState(this);
            }
        }

        public override void EnterState(object param)
        {
            base.EnterState(param);

            Base.PlayerCTRL.DoJump(jumpForce);
        }

        public override void UpdateState()
        {
            base.UpdateState();

            Vector2 moveInput = Base.PlayerInput.GetInputValue<Vector2>(PlayerUnitInputType.Move);
            float moveX = moveInput.x * moveSpeed;
            Base.PlayerCTRL.MoveHorizontally(moveX);
        }

    }
}
