using UnityEngine;


namespace PahlBit
{
    public class PlayerStateWalk : PlayerStateBase
    {
        public float moveSpeed = 7f;

        public override void EnterState(object param)
        {
            base.EnterState(param);

            PlayAnim(AnimStateNameHash.Run);
        }

        public override void UpdateState()
        {
            base.UpdateState();

            float moveX = PlayerInput.MoveX * moveSpeed;
            Base.Phy.MoveHorizontally(moveX);
        }

    }
}
