using System;
using System.Collections.Generic;
using NaughtyAttributes;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;


namespace PahlBit
{
    public class EnemyStateIdle : EnemyStateBase
    {
        public override void EnterState(object param)
        {
            base.EnterState(param);
            
            Stop();
        }

        public override void LeaveState()
        {
            base.LeaveState();
        }
    }
}
