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

    bool IsAwaked = false;

    protected override async UniTask<EnemyState> PatrolMode(CancellationToken ctx)
    {
        // ===== ENTER =====
        Stop();
        mPlayerTarget = null;

        try
        {
            if (IsAwaked)
            {
                // Play Sleep Anim
                // Wait
            }

            mPlayerTarget = await DetectTarget(ctx);
            if (mPlayerTarget != null)
            {
                if (IsTargetInRange(mSpec.AttackRange))
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

        return EnemyState.Patrol;
    }

    protected override UniTask<EnemyState> ChaseMode(CancellationToken ctx)
    {
        return base.ChaseMode(ctx);
    }

    protected override async UniTask<EnemyState> AttackMode(CancellationToken ctx)
    {
        try
        {
            Stop();
            TurnToPlayer();

            AnimEventState animEventState = mBase.AnimHelper.PlayAnim(AnimStateNameHash.Attack);
            await UniTask.WaitUntil(() => animEventState.IsFired, cancellationToken: ctx);
            mBase.Phy.SetMoveSpeedOnly(mSpec.MoveSpeed * 2);
            AttackArea.SetActive(true);
            await UniTask.WaitUntil(() => animEventState.IsFired, cancellationToken: ctx);
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
