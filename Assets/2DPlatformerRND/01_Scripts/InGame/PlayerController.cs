using DG.Tweening;
using UnityEngine;

namespace PahlBit
{
    public class PlayerController : MonoBehaviour
    {
        public bool IsGrounded { get; private set; }
        public float VelocityX { get { return mRB2D.linearVelocity.x; } set { mRB2D.linearVelocity = new Vector2(value, mRB2D.linearVelocity.y); } }
        public float VelocityY { get { return mRB2D.linearVelocity.y; } set { mRB2D.linearVelocity = new Vector2(mRB2D.linearVelocity.x, value); } }
        public Vector2 Velocity { get { return mRB2D.linearVelocity; } set { mRB2D.linearVelocity = value; } }

        BaseObject mBaseObj = null;
        private Rigidbody2D mRB2D = null;

        private void Awake()
        {
            mBaseObj = GetComponentInParent<BaseObject>();
            mRB2D = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            CheckGround();
            UpdateStateMachine();
        }
        private void CheckGround()
        {
            Vector3 bodyColCenter = mRB2D.GetComponent<Collider2D>().bounds.center;
            Vector3 bodyColSize = mRB2D.GetComponent<Collider2D>().bounds.size;
            Vector3 footPos = bodyColCenter - new Vector3(0, bodyColSize.y / 2, 0);
            IsGrounded = Physics2D.OverlapCircle(footPos, 0.1f, 1 << LayerMask.NameToLayer("Terrain"));
            mBaseObj.AnimHelper.SetParamBool("IsGround", IsGrounded);
        }

        void UpdateStateMachine()
        {
            mBaseObj.StateMachine.HandleAllStateInput();
            mBaseObj.StateMachine.UpdateState();
        }

        private void FixedUpdate()
        {
            mBaseObj.StateMachine.FixedUpdateState();
        }

        public void MoveHorizontally(float moveHoriVelocity)
        {
            mBaseObj.AnimHelper.SetParamBool("IsMoving", moveHoriVelocity != 0);
            VelocityX = moveHoriVelocity;
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
            Velocity = Vector2.zero;
        }

        public void DoJump(float jumpForce)
        {
            // 수직 속도 초기화 후 점프력 적용
            VelocityY = 0;
            AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
        

        public void AddForce(Vector2 force, ForceMode2D mode = ForceMode2D.Impulse)
        {
            mRB2D.AddForce(force, mode);
        }
    }
}