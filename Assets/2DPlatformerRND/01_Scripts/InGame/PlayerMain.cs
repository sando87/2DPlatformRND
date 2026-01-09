using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

namespace PahlBit
{
    public class PlayerMain : MonoBehaviour
    {
        private bool mIsGround = false;
        public bool IsGrounded { get => mIsGround && mBaseObj.Phy.VelocityY <= 0.1f; }
        public bool LockControl { get; set; } = false;

        BaseObject mBaseObj = null;
        CharObject mSpec = null;
        PlayerUnitInput mPlayerInput = null;

        private void Awake()
        {
            mBaseObj = GetComponentInParent<BaseObject>();
            mSpec = mBaseObj.GetComponentInChildren<CharObject>();
            mPlayerInput = mBaseObj.GetComponentInChildren<PlayerUnitInput>();
        }
        void Start()
        {
            mBaseObj.Health.InitHealth(mSpec.TotalStats.Health, mSpec.TotalStats.Mana, mSpec.TotalStats.Shield);
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
        }

        void UpdateStateMachine()
        {
            mBaseObj.StateMachine.HandleAllStateInput();
            mBaseObj.StateMachine.UpdateState();
        }

        public void MoveHorizontally(float moveHoriVelocity)
        {
            mBaseObj.Phy.VelocityX = moveHoriVelocity;
            FlipToDir(moveHoriVelocity);
        }
        public void FlipToDir(float dir)
        {
            if (dir == 0) return;

            Vector3 front = dir > 0 ? Vector3.forward : Vector3.back;
            transform.rotation = Quaternion.LookRotation(front, transform.up);
        }
        public void FlipFrontByInput()
        {
            float inputDir = mPlayerInput.GetInputValue<Vector2>(PlayerUnitInputType.Move).x;
            if (inputDir == 0) return;

            Vector3 front = inputDir > 0 ? Vector3.forward : Vector3.back;
            transform.rotation = Quaternion.LookRotation(front, transform.up);
        }
        public void FlipToTarget(Transform target)
        {
            if (target == null) return;

            if (transform.position.x < target.position.x)
                FlipToDir(1);
            else
                FlipToDir(-1);
        }

        public void StopMoving()
        {
            mBaseObj.Phy.Velocity = Vector2.zero;
        }

        public void DoJump(float jumpForce)
        {
            // 수직 속도 초기화 후 점프력 적용
            mBaseObj.Phy.VelocityY = 0;
            mBaseObj.Phy.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        public void OnIteractWith(Collider2D other)
        {
            LOG.trace(other.transform.gameObject.name);
        }

        public void DoSlowEffect(float slowTimeScale, float duration, float fadeoutDuration)
        {
            Time.timeScale = slowTimeScale;
            this.ExDelayedCoroutine(duration, () =>
            {
                if (fadeoutDuration <= 0)
                    Time.timeScale = 1;
                else
                    this.ExForAWhileCoroutine(fadeoutDuration, (rate) => Time.timeScale = Mathf.Max(slowTimeScale, rate));
            });
        }
    }
}