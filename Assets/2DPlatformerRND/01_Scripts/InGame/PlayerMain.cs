using DG.Tweening;
using UnityEngine;

namespace PahlBit
{
    public class PlayerMain : MonoBehaviour
    {
        private bool mIsGround = false;
        public bool IsGrounded { get => mIsGround && mBaseObj.Phy.VelocityY <= 0.1f; }

        BaseObject mBaseObj = null;

        private void Awake()
        {
            mBaseObj = GetComponentInParent<BaseObject>();
        }
        private void Update()
        {
            UpdateGroundState();
            UpdateStateMachine();
        }
        private void FixedUpdate()
        {
            mBaseObj.StateMachine.FixedUpdateState();
        }

        
        private void UpdateGroundState()
        {
            Vector3 footPos = mBaseObj.Body.Foot;
            mIsGround = Physics2D.OverlapCircle(footPos, 0.1f, 1 << LayerMask.NameToLayer("Terrain"));
            mBaseObj.AnimHelper.SetParamBool("IsGround", IsGrounded);
        }

        void UpdateStateMachine()
        {
            mBaseObj.StateMachine.HandleAllStateInput();
            mBaseObj.StateMachine.UpdateState();
        }

        public void MoveHorizontally(float moveHoriVelocity)
        {
            mBaseObj.AnimHelper.SetParamBool("IsMoving", moveHoriVelocity != 0);
            mBaseObj.Phy.VelocityX = moveHoriVelocity;
            FlipToDir(moveHoriVelocity);
        }
        public void FlipToDir(float dir)
        {
            if (dir == 0) return;
            
            Vector3 front = dir > 0 ? Vector3.forward : Vector3.back;
            transform.rotation = Quaternion.LookRotation(front, transform.up);
        }

        public void StopMoving()
        {
            mBaseObj.AnimHelper.SetParamBool("IsMoving", false);
            mBaseObj.Phy.Velocity = Vector2.zero;
        }

        public void DoJump(float jumpForce)
        {
            // 수직 속도 초기화 후 점프력 적용
            mBaseObj.AnimHelper.CrossFadeToState(AnimStateNameHash.Jump);
            mBaseObj.Phy.VelocityY = 0;
            mBaseObj.Phy.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        public void OnIteractWith(Collider2D other)
        {
            LOG.trace(other.transform.gameObject.name);
        }
    }
}