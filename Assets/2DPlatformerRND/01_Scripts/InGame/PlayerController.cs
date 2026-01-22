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

            // 초기 상태
            Vector2 pos = startPosition;
            Vector2 vel = Vector2.up * (impulse / mass);

            // 중력
            Vector2 gravity = new Vector2(0f, -9.81f) * gravityScale;

            float elapsed = 0f;

            while (elapsed < totalTime)
            {
                // 중력 가속
                vel += gravity * dt;

                // Linear Damping (Rigidbody2D.drag)
                vel *= 1f / (1f + linearDamping * dt);

                vel.x = velocityX;

                // 위치 적분
                pos += vel * dt;

                positions.Add(pos);

                elapsed += dt;
            }

            return positions;
        }

        [Button("Simulate Jump Points")]
        public void SimulateJumpPoints()
        {
            // impulse:25 => max height:4.6, duration upto peak:0.4s
            // impulse:22 => max height:3.6, duration upto peak:0.36s
            // impulse:18 => max height:2.5, duration upto peak:0.32s
            // impulse:14 => max height:1.6, duration upto peak:0.24s

            // Gravity: -9.81, Mass: 1, GravityScale: 5, LinearDamping: 1, VelX: 7, ForceY Impulse : 25의 힘 기준

            // impulse:25 => max height:4.6 기준
            // 해석 : 높이4에서는 x축 Gap이 1칸부터 3칸 가능
            // 4 : 1~3
            // 3 : 1~3
            // 2 : 1~4
            // 1 : 1~5
            // 0 : 1~5
            // - 1 : 1~5
            // - 2 : 1~6
            // - 3 : 1~6
            // - 4 : 1~6
            // - 5 : 1~7


            // impulse:18 => max height:2.5 기준
            // 측정해봐야함...


            // 그냥 앞으로 가면서 떨어지는 기준
            // 0 : x
            // - 1 : 1
            // - 2 : 1~2
            // - 3 : 1~3
            // - 4 : 1~3
            // - 5 : 1~4


            var trajectory = SimulateJumpTrajectory(
                startPosition: mBaseObj.transform.position,
                impulse: 25f,
                mass: 1f,
                gravityScale: 5f,
                linearDamping: 1f,
                velocityX: 7f,
                totalTime: 1f
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