using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using PahlBit;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyAIFlying : EnemyAI
{
    bool mIsFlying = false;
    Vector2 mFirstPos;

    protected override void Start()
    {
        base.Start();

        mFirstPos = mBase.Body.Foot;
    }

    protected override async UniTask<EnemyState> PatrolMode(CancellationToken ctx)
    {
        // ===== ENTER =====
        Stop();
        mPlayerTarget = null;

        try
        {
            if (mIsFlying)
            {
                await FlyingToDestPosition(mFirstPos);
                mBase.AnimHelper.CrossFadeToState(AnimStateNameHash.Idle);
                mIsFlying = false;
            }

            mPlayerTarget = await DetectTarget(ctx);
            if (mPlayerTarget != null)
            {
                mIsFlying = true;
                mBase.AnimHelper.PlayAnim(AnimStateNameHash.Fly);
                mBase.Phy.AddForce(Vector2.up * 3);
                TurnTo(mPlayerTarget.Body.Center);
                await UniTask.WaitForSeconds(2, cancellationToken: ctx);
                mBase.Phy.Velocity = Vector2.zero;

                if (IsAttackable())
                {
                    return EnemyState.Attack;
                }
                else
                {
                    return EnemyState.Chase;
                }
            }
        }
        finally
        {
            // ===== EXIT =====
            mIsFlying = true;
        }

        return EnemyState.Patrol;
    }

    protected override async UniTask<EnemyState> ChaseMode(CancellationToken ctx)
    {
        try
        {
            while (!ctx.IsCancellationRequested && mPlayerTarget != null)
            {
                Vector2 targetDestPos = mPlayerTarget.Body.Center + new Vector2(0, 3);
                Vector2 vel = (targetDestPos - mBase.Body.Center).normalized * mSpec.MoveSpeed;
                float distanceFromFirstPos = (mFirstPos - mBase.Body.Foot).magnitude;

                if (distanceFromFirstPos > 20 || !IsTargetInRange(mSpec.DetectLossRange))
                {
                    return EnemyState.Patrol;
                }
                else if (IsTargetInRange(mSpec.AttackRange))
                {
                    return EnemyState.Attack;
                }
                else
                {
                    TurnTo(targetDestPos);
                    mBase.Phy.Velocity = vel;
                    await UniTask.Delay(TimeSpan.FromSeconds(0.1f), cancellationToken: ctx);
                }
            }
        }
        finally
        {
        }
        return EnemyState.Patrol;
    }

    // protected override async UniTask<EnemyState> AttackMode(CancellationToken ctx)
    // {
    //     try
    //     {
    //         Vector2 startPos = mBase.Body.Center;
    //         Vector2 attackPos = mPlayerTarget.Body.Center;
    //         TurnTo(attackPos);
    //         Vector2 vel = (attackPos - mBase.Body.Center).normalized * mSpec.MoveSpeed * 3;
    //         mBase.Phy.Velocity = vel;
    //         await UniTask.WaitUntil(() => mBase.Body.Foot.y <= attackPos.y, cancellationToken: ctx);
    //         DoFireAttack();
    //         mBase.Phy.Velocity = -vel;
    //         await UniTask.WaitUntil(() => mBase.Body.Center.y >= startPos.y, cancellationToken: ctx);
    //         mBase.Phy.Velocity = Vector2.zero;
    //         OnEndAttack();
    //         return EnemyState.Recover;
    //     }
    //     finally
    //     {
    //         // EXIT
    //         // 공격 후 정리 (히트박스 off 등)
    //     }
    // }

    async UniTask FlyingToDestPosition(Vector2 destPos)
    {
        CancelMoveCTS();
        mMoveCTS = new CancellationTokenSource();

        mBase.AnimHelper.CrossFadeToState(AnimStateNameHash.Fly);
        Vector2 startPos = mBase.Body.Foot;
        int startDir = startPos.x < destPos.x ? 1 : -1;
        Turn(startDir);

        Vector2 vel = (destPos - startPos).normalized * mSpec.MoveSpeed;
        while (!mMoveCTS.Token.IsCancellationRequested)
        {
            if (IsArrivedAtDest(mBase.Body.Foot, destPos, vel))
                break;

            mBase.Phy.Velocity = vel;
            await UniTask.Yield(mMoveCTS.Token);
        }

        mBase.Phy.Velocity = Vector2.zero;
        mBase.Phy.MoveFootPosition(destPos);
    }

    bool IsArrivedAtDest(Vector2 curPos, Vector2 destPos, Vector2 vel)
    {
        Vector2 diff = destPos - curPos;
        if (Vector2.Dot(diff, vel) <= 0)
            return true;

        return false;
    }


}
