using UnityEngine;


namespace PahlBit
{
    public class PlayerStateIdle : PlayerStateBase
    {
        public override void InitState()
        {
            base.InitState();
        }

        public override void HandleInput()
        {
            Vector3 moveInput = PlayerInput.GetInputValue<Vector2>(PlayerUnitInputType.Move);
            if (Mathf.Abs(moveInput.x) == 0 && PlayerMain.IsGrounded && !PlayerMain.LockControl)
            {
                Base.StateMachine.ChangeState(this);
            }
        }

        public override void EnterState(object param)
        {
            base.EnterState(param);

            PlayAnim(AnimStateNameHash.Idle);
            PlayerMain.StopMoving();
        }



    }
}
