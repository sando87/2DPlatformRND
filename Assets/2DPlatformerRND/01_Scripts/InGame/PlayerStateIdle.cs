using UnityEngine;


namespace PahlBit
{
    public class PlayerStateIdle : PlayerStateBase
    {
        bool mIsOpenedSelector = false;

        public override void InitState()
        {
            base.InitState();
        }

        // public override void HandleInput()
        // {
        //     Vector3 moveInput = PlayerInput.GetInputValue<Vector2>(PlayerUnitInputType.Move);
        //     if (Mathf.Abs(moveInput.x) == 0 && PlayerMain.IsGrounded && !PlayerMain.LockControl)
        //     {
        //         Base.StateMachine.ChangeState(this);
        //     }
        // }

        public override void EnterState(object param)
        {
            base.EnterState(param);

            PlayAnim(AnimStateNameHash.Idle);
            Base.Phy.StopMoving();

            if (PlayerMain.ItemsAround.Count > 0)
            {
                mIsOpenedSelector = true;
                InGameEngine.Instance.ShowItemSelector(Base.Body.Foot, PlayerMain.ItemsAround);
            }
        }

        public override void UpdateState()
        {
            base.UpdateState();

            if (mIsOpenedSelector)
            {
                if (PlayerInput.IsLoseControl)
                {
                    CloseItemSelector();
                }
                else if (PlayerInput.JustPressed(PlayerUnitInputType.UIMove))
                {
                    if (PlayerInput.MoveY > 0)
                        InGameEngine.Instance.MoveItemSelector(true);
                    else if (PlayerInput.MoveY < 0)
                        InGameEngine.Instance.MoveItemSelector(false);
                }
            }
        }

        public override void LeaveState()
        {
            base.LeaveState();

            if (mIsOpenedSelector)
            {
                CloseItemSelector();
            }
        }

        void CloseItemSelector()
        {
            InGameEngine.Instance.HideItemSelector();
            mIsOpenedSelector = false;
        }



    }
}
