using UnityEngine;


namespace PahlBit
{
    public class PlayerStateDash : PlayerStateBase
    {
        [SerializeField] float _dashForce = 20f;
        [SerializeField] float _dashDuration = 1.2f;

        public override void HandleInput()
        {
            if (Base.PlayerInput.JustPressed(PlayerUnitInputType.Dash) && Base.PlayerCTRL.IsGrounded)
            {
                Base.StateMachine.ChangeState(this);
            }
        }

        public override void EnterState(object param)
        {
            base.EnterState(param);

            this.ExDelayedCoroutine(_dashDuration, () => ChangeStateToIdle());
            
            DoDash();
        }


        private void DoDash()
        {
            Base.AnimHelper.CrossFadeToState("PlayerDash", 0);

            Base.PlayerCTRL.Velocity = new Vector2(transform.right.x * _dashForce, 0f);
        }

    }
}
