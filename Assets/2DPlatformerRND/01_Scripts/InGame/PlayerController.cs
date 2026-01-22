using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;

namespace PahlBit
{
    /// <summary>
    /// 플레이어 캐릭터의 입력을 받아서 이동, 점프, 상호작용 등을 처리하는 컨트롤러 클래스
    /// 주요기능 : 복잡한 주변의 상황과 입력에 따라 그에 맞는 FSM 상태를 전환시킨다.
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] float _JumpForce = 25f;

        private bool mIsGround = false;
        public bool IsGrounded { get => mIsGround && mBaseObj.Phy.VelocityY <= 0.1f; }

        public bool LockMove { get; set; } = false;
        public bool LockJump { get; set; } = false;
        public bool LockDash { get; set; } = false;
        public bool LockAll
        {
            get { return LockMove && LockJump && LockDash; }
            set { LockMove = value; LockJump = value; LockDash = value; }
        }

        BaseObject mBaseObj = null;
        CharObject mSpec = null;
        PlayerUnitInput mPlayerInput = null;
        FiniteStateMachine mFSM = null;

        PlayerStateIdle mFsmIdle = null;
        PlayerStateWalk mFsmWalk = null;
        PlayerStateFloating mFsmFloat = null;

        private void Awake()
        {
            mBaseObj = GetComponentInParent<BaseObject>();
            mSpec = mBaseObj.GetComponentInChildren<CharObject>();
            mPlayerInput = mBaseObj.GetComponentInChildren<PlayerUnitInput>();

            mFSM = mBaseObj.StateMachine;
            mFsmIdle = mFSM.FindState<PlayerStateIdle>();
            mFsmWalk = mFSM.FindState<PlayerStateWalk>();
            mFsmFloat = mFSM.FindState<PlayerStateFloating>();
        }
        void Start()
        {
        }
        private void Update()
        {
            DoMovement();
            Jump();
            DropDown();
            Dash();
        }

        void FixedUpdate()
        {
            UpdateEnvironmentState();
        }

        void DoMovement()
        {
            if (LockMove)
                return;

            if (IsGrounded)
            {
                float moveX = mPlayerInput.MoveX;
                if (!moveX.ExIsAlmostZero())
                {
                    mFSM.ChangeState(mFsmWalk);
                }
                else
                {
                    mFSM.ChangeState(mFsmIdle);
                }
            }
            else
            {
                mFSM.ChangeState(mFsmFloat);
            }

        }
        void Jump()
        {
            if (LockJump)
                return;

            if (mPlayerInput.JustPressed(PlayerUnitInputType.Jump)
            && mPlayerInput.MoveY >= 0)
            {
                SimulateJumpPoints();
                mBaseObj.Phy.DoJump(_JumpForce);
                mFSM.ChangeStateForce(mFsmFloat);
            }
        }
        void DropDown()
        {
            if (LockJump)
                return;

            if (mPlayerInput.JustPressed(PlayerUnitInputType.Jump)
            && mPlayerInput.MoveY < 0
            && IsGrounded)
            {
                mBaseObj.Body.LockThinPlatform = true;
                this.ExDelayedCoroutine(0.2f, () => mBaseObj.Body.LockThinPlatform = false);
            }
        }
        void Dash()
        {
            if (LockDash)
                return;

            if (mPlayerInput.JustPressed(PlayerUnitInputType.Dash))
            {
                mFSM.ChangeState<PlayerStateDash>();
            }
        }


        private void UpdateEnvironmentState()
        {
            int layerMask = MyLayerMask.Ground;
            Vector2 footPos = mBaseObj.Body.Foot;

            bool isOverlapped = Physics2D.OverlapCircle(footPos + new Vector2(0, 0.1f), 0.05f, layerMask);

            Vector2 bodySize = mBaseObj.Body.Size;
            Rect box = new Rect();
            box.size = new Vector2(bodySize.x, 0.1f);
            box.center = footPos + new Vector2(0, 0.05f);
            bool isCasted = Physics2D.BoxCast(box.center, box.size, 0, Vector2.down, 0.1f, layerMask);

            mIsGround = !isOverlapped && isCasted;
        }


        public static List<Vector2> SimulateJumpTrajectory(
            Vector2 startPosition,
            float impulse,
            float mass,
            float gravityScale,
            float linearDamping,
            float velocityX,
            float totalTime)
        {
            List<Vector2> positions = new();

            float dt = Time.fixedDeltaTime;
            // float dt = 0.01f;

            // 초기 상태
            Vector2 pos = startPosition;
            Vector2 vel = Vector2.up * (impulse / mass);

            // int startY = (int)startPosition.y;
            // float peakY = 0;
            // int refY = 0;

            // 중력
            Vector2 gravity = new Vector2(0f, -9.81f) * gravityScale;

            float elapsed = 0f;
            // List<Vector2> points = new List<Vector2>();

            while (elapsed < totalTime)
            {
                // 중력 가속
                vel += gravity * dt;

                // Linear Damping (Rigidbody2D.drag)
                vel *= 1f / (1f + linearDamping * dt);

                vel.x = velocityX;
                
                // if(vel.y < 0)
                // {
                //     if(peakY == 0)
                //     {
                //         peakY = pos.y;
                //         refY = (int)peakY;
                //         LOG.trace("Peak Y at: " + (peakY - startPosition.y).ToString("F2") + "m");
                //         LOG.trace("Time to Peak: " + elapsed.ToString("F2") + "s");
                //     }
                // }

                // 위치 적분
                pos += vel * dt;

                // if(vel.y < 0)
                // {
                //     if(pos.y < refY)
                //     {
                //         int dy = refY - startY;
                //         // LOG.trace(dy + " : " + elapsed.ToString("F2") + "s");
                //         points.Add(new Vector2(dy, elapsed));
                //         refY--;
                //     }
                // }

                positions.Add(pos);

                elapsed += dt;
            }

            // string result = string.Join(",", points);
            // Debug.Log(result);

            return positions;
        }

        [Button("Simulate Jump Points")]
        public void SimulateJumpPoints()
        {
            // Gravity: -9.81, Mass: 1, GravityScale: 5, LinearDamping: 1, VelX: 7 
            // Impulse 힘에 따른 높이까지 떨어지는데 걸리는 시간 테이블
            // 힘 25점프, 높이 4.7, 피크까지시간 0.41s
            // (4.00, 0.58),(3.00, 0.68),(2.00, 0.76),(1.00, 0.82),(0.00, 0.88),(-1.00, 0.93),(-2.00, 0.98),(-3.00, 1.02),(-4.00, 1.07),(-5.00, 1.11),(-6.00, 1.15)
            // 힘 22점프, 높이 3.73, 피크까지시간 0.37s
            // (3.00, 0.54),(2.00, 0.64),(1.00, 0.72),(0.00, 0.78),(-1.00, 0.84),(-2.00, 0.89),(-3.00, 0.94),(-4.00, 0.98),(-5.00, 1.03),(-6.00, 1.07)
            // 힘 18점프, 높이 2.59, 피크까지시간 0.31s
            // (2.00, 0.46),(1.00, 0.57),(0.00, 0.65),(-1.00, 0.71),(-2.00, 0.77),(-3.00, 0.82),(-4.00, 0.87),(-5.00, 0.92),(-6.00, 0.96)
            // 힘 14점프, 높이 1.62, 피크까지시간 0.25s
            // (1.00, 0.41),(0.00, 0.51),(-1.00, 0.59),(-2.00, 0.65),(-3.00, 0.71),(-4.00, 0.76),(-5.00, 0.81),(-6.00, 0.86)

            var trajectory = SimulateJumpTrajectory(
                startPosition: mBaseObj.transform.position,
                impulse: _JumpForce,
                mass: 1f,
                gravityScale: 5f,
                linearDamping: 1f,
                velocityX: 7f,
                totalTime: 1.5f
            );

            for (int i = 0; i < trajectory.Count - 1; i++)
            {
                bool isUp = trajectory[i + 1].y > trajectory[i].y;
                Color lineColor = isUp ? Color.red : Color.blue;
                Debug.DrawLine(trajectory[i], trajectory[i] + new Vector2(0.2f, 0), lineColor, 5f);
            }
        }

    }
}