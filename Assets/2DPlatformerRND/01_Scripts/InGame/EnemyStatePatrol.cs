using System;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Events;


namespace PahlBit
{
    public class EnemyStatePatrol : EnemyStateBase
    {

        public override void EnterState(object param)
        {
            base.EnterState(param);
            this.ExDelayedTask(3, () => LOG.trace()).Forget();
        }

        public override void LeaveState()
        {
            base.LeaveState();
        }
    }
}
