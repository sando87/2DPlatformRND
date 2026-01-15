using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
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
            // ListenDropDownOnewayPlatform().Forget();
        }
        private void Update()
        {
            UpdateEnvironmentState();

            DoMovement();
            Jump();
            Dash();
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
                mBaseObj.Phy.DoJump(_JumpForce);
                mFSM.ChangeStateForce(mFsmFloat);
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
            Vector3 footPos = mBaseObj.Body.Foot;
            mIsGround = Physics2D.OverlapCircle(footPos, 0.1f, 1 << LayerID.Terrain);
        }



        async UniTask ListenDropDownOnewayPlatform()
        {
            CancellationToken ct = this.GetCancellationTokenOnDestroy();
            while (!ct.IsCancellationRequested)
            {
                await UniTask.WaitUntil(() =>
                    mBaseObj.Input.MoveY < 0 &&
                    mBaseObj.Input.JustPressed(PlayerUnitInputType.Jump),
                    cancellationToken: ct
                );

                // Physics2D.IgnoreLayerCollision(LayerID.Player, LayerID.PlatformOneway, true);

                // await UniTask.Delay(TimeSpan.FromSeconds(0.2f), cancellationToken: ct);

                // Physics2D.IgnoreLayerCollision(LayerID.Player, LayerID.PlatformOneway, false);

                // try
                // {
                //     await UniTask.Delay(TimeSpan.FromSeconds(0.2f), cancellationToken: ct);
                // }
                // finally
                // {
                //     // 🔥 무조건 복구
                //     Physics2D.IgnoreLayerCollision(LayerID.Player, LayerID.PlatformOneway, false);
                // }

                await UniTask.Yield(PlayerLoopTiming.Update);
            }
        }
    }
}