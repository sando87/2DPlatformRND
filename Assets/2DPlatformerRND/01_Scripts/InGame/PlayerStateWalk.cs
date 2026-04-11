using UnityEngine;


namespace PahlBit
{
    public class PlayerStateWalk : PlayerStateBase
    {
        public override void EnterState(object param)
        {
            base.EnterState(param);

            PlayAnim(AnimStateNameHash.Run);
        }

        public override void UpdateState()
        {
            base.UpdateState();

            float moveX = PlayerInput.MoveX * PlayerMain.Spec.MoveSpeed;
            Base.Phy.MoveHorizontally(moveX);
        }

    }
}
