using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using PahlBit;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyAIMushroom : EnemyAI
{
    [SerializeField] GameObject AttackArea = null;

    bool mIsAwaked = true;

    protected override async UniTask<EnemyState> PatrolMode(CancellationToken ctx)
    {
        // ===== ENTER =====
        Stop();
        mPlayerTarget = null;

        try
        {
            AnimEventState animEventState = null;
            if (mIsAwaked)
            {
                mIsAwaked = false;
                animEventState = mBase.AnimHelper.PlayAnim(AnimStateNameHash.Sleep);
                await UniTask.WaitUntil(() => animEventState.IsEnd, cancellationToken: ctx);
            }

            mPlayerTarget = await DetectTarget(ctx);
            if (mPlayerTarget != null)
            {
                animEventState = mBase.AnimHelper.PlayAnim(AnimStateNameHash.WakeUp);
                await UniTask.WaitUntil(() => animEventState.IsEnd, cancellationToken: ctx);
                mIsAwaked = true;

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
            mIsAwaked = true;
            Stop();
        }

        return EnemyState.Patrol;
    }

    protected override async UniTask<EnemyState> ChaseMode(CancellationToken ctx)
    {
        try
        {
            DoChaseMoving(ctx).Forget();
            int returnIdx = await UniTask.WhenAny(IsAttackableTarget(ctx), IsLostTarget(ctx));
            if (returnIdx == 0)
                return EnemyState.Attack;
            else if (returnIdx == 1)
                return EnemyState.Patrol;
        }
        finally
        {
        }
        return EnemyState.Patrol;
    }

    protected override async UniTask<EnemyState> AttackMode(CancellationToken ctx)
    {
        try
        {
            Stop();
            TurnTo(mPlayerTarget.Body.Center);
            mAttackTime = Time.time;

            AnimEventState animEventState = mBase.AnimHelper.PlayAnim(AnimStateNameHash.Attack);
            await UniTask.WaitUntil(() => animEventState.FireIndex == 0, cancellationToken: ctx);
            AttackArea.SetActive(true);
            while (animEventState.FireIndex == 0)
            {
                mBase.Phy.SetMoveSpeedOnly(mSpec.MoveSpeed * 3);
                await UniTask.Yield(cancellationToken: ctx);
            }
            mBase.Phy.SetMoveSpeedOnly(0);
            AttackArea.SetActive(false);
            await UniTask.WaitUntil(() => animEventState.IsEnd, cancellationToken: ctx);
            OnEndAttack();
            return EnemyState.Recover;
        }
        finally
        {
            // EXIT
            // 공격 후 정리 (히트박스 off 등)
            AttackArea.SetActive(false);
        }
    }

    // AttackArea Obecjt Event 로 등록되어서 호출됨
    public void OnAttack(Collider2D col)
    {
        // 충돌 시 처리할 내용
        Health health = col.ExGetBase().GetComponentInChildren<Health>();
        if (health != null)
        {
            float damage = mSpec.BaseAttack;
            health.GetDamaged(damage);
        }
    }

}
