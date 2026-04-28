using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using PahlBit;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

public class EnemyAIBoss : EnemyAI
{
    Health mHealth;
    EnemyBossBase mBoss;

    protected override void Start()
    {
        base.Start();

        mHealth = mBase.Health;
        mBoss = mBase.GetComponent<EnemyBossBase>();
    }

    // protected override UniTask<EnemyState> PatrolMode(CancellationToken ctx)
    // {
    //     return base.PatrolMode(ctx);
    // }

    protected async override UniTask<EnemyState> ChaseMode(CancellationToken ctx)
    {
        try
        {
            DoChaseMovePattern(ctx).Forget();

            int returnIdx = await UniTask.WhenAny(ThinkNextAttack(ctx), IsLostTarget(ctx));
            if (returnIdx == 0)
                return EnemyState.Attack;
            else if (returnIdx == 1)
                return EnemyState.Idle;
        }
        finally
        {
        }
        return EnemyState.Idle;
    }

    protected async override UniTask<EnemyState> AttackMode(CancellationToken ctx)
    {
        try
        {
            Stop();
            TurnToPlayer();

            if (MyUtils.RandomInt(0, 2) == 0)
            {
                AnimEventState animEventState = mBase.AnimHelper.PlayAnim(AnimStateNameHash.BossAttackA);
                await UniTask.WaitUntil(() => animEventState.IsFired, cancellationToken: ctx);
                mBoss.DoFire_AttackA();
                await UniTask.WaitUntil(() => animEventState.IsEnd, cancellationToken: ctx);
                OnEndAttack();
            }
            else
            {
                AnimEventState animEventState = mBase.AnimHelper.PlayAnim(AnimStateNameHash.BossAttackB);
                await UniTask.WaitUntil(() => animEventState.IsFired, cancellationToken: ctx);
                mBoss.DoFire_AttackB();
                await UniTask.WaitUntil(() => animEventState.IsEnd, cancellationToken: ctx);
                OnEndAttack();
            }

            return EnemyState.Chase;
        }
        finally
        {
            // EXIT
            // 공격 후 정리 (히트박스 off 등)
        }
    }

    protected async UniTask ThinkNextAttack(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            if (MyUtils.RandomInt(0, 2) == 0)
            {
                break;
            }
            else
            {
                float thinkInterval = MyUtils.RandomFloat(_ThinkInterval - 0.5f, _ThinkInterval + 0.5f);
                thinkInterval.ExSetMinimum(0.1f);
                await UniTask.Delay(TimeSpan.FromSeconds(thinkInterval), cancellationToken: ct);
            }
        }
    }

    protected async UniTask DoChaseMovePattern(CancellationToken ct)
    {
        try
        {
            await UniTask.Yield(cancellationToken: ct);
            Stop();
            while (!ct.IsCancellationRequested && PlayerTarget != null)
            {
                int curDir = mBase.Body.FrontDirInt;
                curDir = (MyUtils.RandomInt(0, 2) * 2) - 1;
                Turn(curDir);
                StartMoving(curDir * mSpec.MoveSpeed);
                await UniTask.Delay(TimeSpan.FromSeconds(MyUtils.RandomFloat(0.5f, 3.5f)), cancellationToken: ct);
            }
        }
        catch (OperationCanceledException)
        {
            // LOG.trace(ex.Message);
        }
    }

}
