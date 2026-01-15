using UnityEngine;


namespace PahlBit
{
    public class PlayerStateFloating : PlayerStateBase
    {
        public float moveSpeed = 7f;

        // public override void HandleInput()
        // {
        //     if (PlayerMain.LockControl)
        //         return;

        //     if(!PlayerMain.IsGrounded)
        //     {
        //         Base.StateMachine.ChangeState(this);
        //     }
        // }

        public override void EnterState(object param)
        {
            base.EnterState(param);

            PlayAnim(AnimStateNameHash.Jump);
        }

        public override void UpdateState()
        {
            base.UpdateState();

            float moveX = PlayerInput.MoveX * moveSpeed;
            Base.Phy.MoveHorizontally(moveX);
        }

    }
}
