using UnityEngine;


namespace PahlBit
{
    public class PlayerStateWalk : PlayerStateBase
    {
        public float moveSpeed = 7f;

        public override void HandleInput()
        {
            Vector2 moveInput = PlayerInput.GetInputValue<Vector2>(PlayerUnitInputType.Move);
            if(Mathf.Abs(moveInput.x) > 0.01f && PlayerMain.IsGrounded && !PlayerMain.LockControl)
            {
                Base.StateMachine.ChangeState(this);
            }
        }
        public override void EnterState(object param)
        {
            base.EnterState(param);

            PlayAnim(AnimStateNameHash.Run);
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
