using UnityEngine;


namespace PahlBit
{
    public class PlayerStateFloating : PlayerStateBase
    {
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

            float moveX = PlayerInput.MoveX * PlayerMain.Spec.MoveSpeed;
            Base.Phy.MoveHorizontally(moveX);
        }

    }
}
