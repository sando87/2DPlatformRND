using UnityEngine;


namespace PahlBit
{
    public class PlayerStateDash : PlayerStateBase
    {
        [SerializeField] float _dashForce = 20f;

        public override void HandleInput()
        {
            if (PlayerInput.JustPressed(PlayerUnitInputType.Dash))
            {
                Base.StateMachine.ChangeState(this);
            }
        }

        public override void EnterState(object param)
        {
            base.EnterState(param);
            Base.Phy.LockGravity = true;

            DoDash();
            
            ExitStateOnEnd();
        }

        public override void LeaveState()
        {
            base.LeaveState();
            Base.Phy.LockGravity = false;
        }



        private void DoDash()
        {
            Base.AnimHelper.CrossFadeToState(AnimStateNameHash.Dash);

            Base.Phy.Velocity = new Vector2(transform.right.x * _dashForce, 0f);
        }

    }
}
