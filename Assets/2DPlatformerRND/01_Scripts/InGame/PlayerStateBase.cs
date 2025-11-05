using System;
using System.Collections.Generic;
using NaughtyAttributes;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;


namespace PahlBit
{
    public class PlayerStateBase : FiniteStateBase
    {
        [SerializeField] bool _LockControl = false;

        public PlayerUnitInput PlayerInput { get => GetComponentInParent<PlayerUnitInput>(); }
        public PlayerMain PlayerMain { get => GetComponentInParent<PlayerMain>(); }

        public override void EnterState(object param)
        {
            base.EnterState(param);
            if (_LockControl)
                PlayerMain.LockControl = true;
        }

        public override void LeaveState()
        {
            base.LeaveState();
            if (_LockControl)
                PlayerMain.LockControl = false;
        }

        public void ExitStateOnEnd()
        {
            AddEventEnter(AnimStateNameHash.ExitDummy, () => ChangeControlableState());
        }

        public void ChangeControlableState()
        {
            if (!PlayerMain.IsGrounded)
            {
                Base.StateMachine.ChangeState<PlayerStateFloating>(null, true);
            }
            else
            {
                Vector2 moveInput = PlayerInput.GetInputValue<Vector2>(PlayerUnitInputType.Move);
                if (Mathf.Abs(moveInput.x) > 0.01f)
                {
                    Base.StateMachine.ChangeState<PlayerStateWalk>(null, true);
                }
                else
                {
                    Base.StateMachine.ChangeState<PlayerStateIdle>(null, true);
                }
            }
        }


    }
}
