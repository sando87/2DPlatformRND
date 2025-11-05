using UnityEngine;


namespace PahlBit
{
    public class PlayerStateJump : PlayerStateBase
    {
        public float moveSpeed = 7f;
        public float jumpForce = 14f;

        public override void HandleInput()
        {
            if (PlayerMain.LockControl)
                return;

            if (PlayerInput.JustPressed(PlayerUnitInputType.Jump))
            {
                PlayerMain.DoJump(jumpForce);
                Base.StateMachine.ChangeState(this);
            }
            else if(!PlayerMain.IsGrounded)
            {
                Base.StateMachine.ChangeState(this);
            }
        }

        public override void EnterState(object param)
        {
            base.EnterState(param);

            PlayAnim(AnimStateNameHash.Jump);
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
