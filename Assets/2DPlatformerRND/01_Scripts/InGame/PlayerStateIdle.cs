using UnityEngine;


namespace PahlBit
{
    public class PlayerStateIdle : PlayerStateBase
    {
        public override void InitState()
        {
            base.InitState();

            Base.AnimHelper.AddEventEnter(AnimStateNameHash.Idle, () =>
            {
                ChangeStateToIdle();
            });
        }

        // public override void HandleInput()
        // {
        //     Vector3 moveInput = Base.PlayerInput.GetInputValue<Vector2>(PlayerUnitInputType.Move);
        //     if (Mathf.Abs(moveInput.x) == 0 && Base.PlayerCTRL.IsGrounded)
        //     {
        //         Base.StateMachine.ChangeState(this);
        //     }
        // }

        public override void EnterState(object param)
        {
            base.EnterState(param);

            PlayerMain.StopMoving();
        }



    }
}
