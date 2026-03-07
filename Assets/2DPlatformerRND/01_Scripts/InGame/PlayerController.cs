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

        public bool IsGrounded { get => mBaseObj.Phy.IsGrounded; }

        public bool LockMove { get; set; } = false;
        public bool LockJump { get; set; } = false;
        public bool LockDash { get; set; } = false;
        public bool LockAll
        {
            get { return LockMove && LockJump && LockDash; }
            set { LockMove = value; LockJump = value; LockDash = value; }
        }

        BaseObject mBaseObj = null;
        SpecPlayer mSpec = null;
        PlayerUnitInput mPlayerInput = null;
        FiniteStateMachine mFSM = null;

        PlayerStateIdle mFsmIdle = null;
        PlayerStateWalk mFsmWalk = null;
        PlayerStateFloating mFsmFloat = null;

        private void Awake()
        {
            mBaseObj = GetComponentInParent<BaseObject>();
            mSpec = mBaseObj.GetComponentInChildren<SpecPlayer>();
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

        void DoMovement()
        {
            if (LockMove)
                return;

            if (IsGrounded)
            {
                float moveX = mPlayerInput.MoveX;
                if (!moveX.ExIsAlmostZero())
                {
                    mFSM.TryChangeState(mFsmWalk);
                }
                else
                {
                    mFSM.TryChangeState(mFsmIdle);
                }
            }
            else
            {
                mFSM.TryChangeState(mFsmFloat);
            }

        }
        void Jump()
        {
            if (LockJump)
                return;

            if (mPlayerInput.JustPressed(PlayerUnitInputType.Jump)
            && mPlayerInput.MoveY >= 0)
            {
                // SimulateJumpPoints();
                mBaseObj.Phy.DoJump(_JumpForce);
                mFSM.ForceChangeState(mFsmFloat);
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
                mFSM.TryChangeState<PlayerStateDash>();
            }
        }

        [Button("Simulate Jump Points")]
        public void SimulateJumpPoints()
        {
            JumpSimulationTable.DrawSimulationPoints(mBaseObj.transform.position, _JumpForce);
        }

    }
}