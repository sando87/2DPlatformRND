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
        PlayerTarget = null;

        try
        {
            if (mIsFlying)
            {
                await FlyingToDestPosition(mFirstPos);
                mBase.AnimHelper.CrossFadeToState(AnimStateNameHash.Idle);
                mIsFlying = false;
            }

            PlayerTarget = await DetectTarget(ctx);
            if (PlayerTarget != null)
            {
                mIsFlying = true;
                mBase.AnimHelper.PlayAnim(AnimStateNameHash.Fly);
                mBase.Phy.AddForce(Vector2.up * 3);
                TurnTo(PlayerTarget.Body.Center);
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
            DoChaseFlying(ctx).Forget();
            int returnIdx = await UniTask.WhenAny(IsAttackableTarget(ctx), IsLostTarget(ctx), IsTooFarFromFirstPos(ctx));
            if (returnIdx == 0)
                return EnemyState.Attack;
            else if (returnIdx == 1)
                return EnemyState.Patrol;
            else if (returnIdx == 2)
                return EnemyState.Patrol;
        }
        finally
        {
        }
        return EnemyState.Patrol;
        // try
        // {
        //     while (!ctx.IsCancellationRequested && mPlayerTarget != null)
        //     {
        //         Vector2 targetDestPos = mPlayerTarget.Body.Center + new Vector2(0, 3);
        //         Vector2 vel = (targetDestPos - mBase.Body.Center).normalized * mSpec.MoveSpeed;
        //         float distanceFromFirstPos = (mFirstPos - mBase.Body.Foot).magnitude;

        //         if (distanceFromFirstPos > 20 || !IsTargetInRange(mSpec.DetectLossRange))
        //         {
        //             return EnemyState.Patrol;
        //         }
        //         else if (IsTargetInRange(mSpec.AttackRange))
        //         {
        //             return EnemyState.Attack;
        //         }
        //         else
        //         {
        //             TurnTo(targetDestPos);
        //             mBase.Phy.Velocity = vel;
        //             await UniTask.Delay(TimeSpan.FromSeconds(0.1f), cancellationToken: ctx);
        //         }
        //     }
        // }
        // finally
        // {
        // }
        // return EnemyState.Patrol;
    }

    protected async UniTask IsTooFarFromFirstPos(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            float distanceFromFirstPos = (mFirstPos - mBase.Body.Foot).sqrMagnitude;
            float refDistance = 20;
            if (distanceFromFirstPos > refDistance * refDistance)
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

    protected async UniTask DoChaseFlying(CancellationToken ct)
    {
        try
        {
            await UniTask.Yield(cancellationToken: ct);
            Stop();
            mBase.AnimHelper.PlayAnim(AnimStateNameHash.Fly);
            while (!ct.IsCancellationRequested && PlayerTarget != null)
            {
                Rect destArea = new Rect();
                destArea.size = new Vector2(8, 3);
                destArea.center = PlayerTarget.Body.Head + new Vector2(0, 5);
                Vector2 destPos = MyUtils.Random(destArea);
                Vector2 vel = (destPos - mBase.Body.Center).normalized * mSpec.MoveSpeed;
                TurnTo(destPos);
                while ((destPos - mBase.Body.Center).magnitude > 2)
                {
                    mBase.Phy.Velocity = vel;
                    await UniTask.Yield(cancellationToken: ct);
                }
                await UniTask.Delay(TimeSpan.FromSeconds(MyUtils.RandomFloat(1f, 5f)), cancellationToken: ct);
            }
        }
        catch (OperationCanceledException)
        {
            // LOG.trace(ex.Message);
        }
    }

    protected override bool IsAttackable()
    {
        if (PlayerTarget == null)
            return false;

        if (IsCooltime)
            return false;

        float range = mSpec.AttackRange;
        float distSqr = Vector2.SqrMagnitude(mBase.Body.Center - PlayerTarget.Body.Center);
        if (distSqr > range * range)
            return false;

        return true;
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
