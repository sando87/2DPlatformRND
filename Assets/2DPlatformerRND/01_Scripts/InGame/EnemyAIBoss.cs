using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using NaughtyAttributes;
using PahlBit;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

public class EnemyAIBoss : EnemyAI
{
    [SerializeField] List<Transform> _JumpPositions;

    Health mHealth;
    EnemyBossBase mBoss;
    bool mIsJumping = false;

    protected override void Start()
    {
        base.Start();

        mHealth = mBase.Health;
        mBoss = mBase.GetComponent<EnemyBossBase>();
    }

    protected async override UniTask<EnemyState> IdleMode(CancellationToken ctx)
    {
        Stop();
        PlayerTarget = null;

        try
        {
            mBase.AnimHelper.PlayAnim(AnimStateNameHash.Frozen);
            await UniTask.WaitUntil(() => mBoss.IsAwaked, cancellationToken: ctx);
            mBase.AnimHelper.PlayAnim(AnimStateNameHash.Idle);
            await UniTask.Delay(TimeSpan.FromSeconds(2), cancellationToken: ctx);
            PlayerTarget = await DetectTarget(ctx);
            if (PlayerTarget != null)
            {
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
            Stop();
        }
        return EnemyState.Idle;
    }

    protected async override UniTask<EnemyState> ChaseMode(CancellationToken ctx)
    {
        try
        {
            DoChaseMovePattern(ctx).Forget();

            int returnIdx = await UniTask.WhenAny(ThinkNextAttack(ctx), IsLostTarget(ctx));
            if (returnIdx == 0)
            {
                return EnemyState.Attack;
            }
            else if (returnIdx == 1)
            {
                Stop();
                PlayerTarget = null;
                mBase.AnimHelper.PlayAnim(AnimStateNameHash.Idle);
                await UniTask.Delay(TimeSpan.FromSeconds(2), cancellationToken: ctx);
                PlayerTarget = await DetectTarget(ctx);
            }
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
            if (MyUtils.RandomInt(0, 4) == 0)
            {
                break;
            }
            else
            {
                float thinkInterval = MyUtils.RandomFloat(_ThinkInterval - 0.5f, _ThinkInterval + 0.5f);
                thinkInterval.ExSetMinimum(0.1f);
                await UniTask.Delay(TimeSpan.FromSeconds(thinkInterval), cancellationToken: ct);
                await UniTask.WaitUntil(() => !mIsJumping, cancellationToken: ct);
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
                curDir = IsNoWayToMove() ? -curDir : ((MyUtils.RandomInt(0, 2) * 2) - 1);
                Turn(curDir);
                StartMoving(curDir * mSpec.MoveSpeed);
                await UniTask.Delay(TimeSpan.FromSeconds(MyUtils.RandomFloat(0.5f, 3.5f)), cancellationToken: ct);

                if (MyUtils.RandomInt(0, 2) == 0)
                {
                    mBase.AnimHelper.CrossFadeToState(AnimStateNameHash.Idle);
                    Stop();
                    await UniTask.Delay(TimeSpan.FromSeconds(1.5f), cancellationToken: ct);

                    Vector2 destPos = GetNextJumpDest();
                    await JumpTo(destPos, 1.5f, ct);
                }
            }
        }
        catch (OperationCanceledException)
        {
            // LOG.trace(ex.Message);
        }
    }
    Vector2 GetNextJumpDest()
    {
        int ranIdx = MyUtils.RandomInt(0, _JumpPositions.Count);
        for (int i = 0; i < _JumpPositions.Count; i++)
        {
            int idx = (ranIdx + i) % _JumpPositions.Count;
            Vector2 destPos = _JumpPositions[idx].position;
            float dist = Vector2.Distance(mBase.transform.position, destPos);
            if (5 < dist && dist < 25f)
            {
                return destPos;
            }
        }
        return _JumpPositions[ranIdx].position;
    }

    protected async UniTask JumpTo(Vector3 destPos, float duration, CancellationToken ct)
    {
        try
        {
            mIsJumping = true;
            float halfduration = duration * 0.5f;
            await UniTask.Yield(cancellationToken: ct);
            Stop();
            mBase.AnimHelper.CrossFadeToState(AnimStateNameHash.Jump);
            int dir = mBase.Body.Center.x < PlayerTarget.transform.position.x ? 1 : -1;
            Turn(dir);

            mBase.transform.DOMoveX(destPos.x, duration).SetEase(Ease.Linear);
            float maxY = Mathf.Max(mBase.Body.Foot.y, destPos.y) + 7;
            mBase.transform.DOMoveY(maxY, halfduration).SetEase(Ease.OutQuad);
            await UniTask.Delay(TimeSpan.FromSeconds(halfduration), cancellationToken: ct);
            mBase.transform.DOMoveY(destPos.y, halfduration).SetEase(Ease.InQuad);
            await UniTask.Delay(TimeSpan.FromSeconds(halfduration), cancellationToken: ct);
            mBase.AnimHelper.CrossFadeToState(AnimStateNameHash.Idle);
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: ct);
        }
        catch (OperationCanceledException)
        {
            // LOG.trace(ex.Message);
        }
        finally
        {
            mIsJumping = false;
            mBase.transform.DOKill();
        }
    }

}
