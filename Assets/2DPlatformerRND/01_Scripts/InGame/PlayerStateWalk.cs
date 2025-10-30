using UnityEngine;


namespace PahlBit
{
    public class PlayerStateWalk : PlayerStateBase
    {
        public float moveSpeed = 7f;

        public override void HandleInput()
        {
            Vector2 moveInput = Base.PlayerInput.GetInputValue<Vector2>(PlayerUnitInputType.Move);
            if(Mathf.Abs(moveInput.x) > 0.01f && Base.PlayerCTRL.IsGrounded)
            {
                Base.StateMachine.ChangeState(this);
            }
        }
        public override void EnterState(object param)
        {
            base.EnterState(param);
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
