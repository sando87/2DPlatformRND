using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
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

            PatrolWalk(this.GetCancellationTokenOnDestroy()).Forget();

            // this.ExDelayedTask(3, () => LOG.trace()).Forget();


        }

        async UniTask PatrolWalk(CancellationToken ct)
        {
            try
            {
                int curDir = Base.Body.FrontDirInt;
                while (!ct.IsCancellationRequested)
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(MyUtils.RandomFloat(0.5f, 1.5f)), cancellationToken: ct);
                    curDir *= -1;
                    Turn(curDir);
                    Move(curDir);
                    await UniTask.Delay(TimeSpan.FromSeconds(MyUtils.RandomFloat(1.5f, 2.5f)), cancellationToken: ct);
                    Stop();
                }
            }
            catch (OperationCanceledException)
            {
                // LOG.trace(ex.Message);
            }
        }

        public override void LeaveState()
        {
            base.LeaveState();
        }
    }
}
