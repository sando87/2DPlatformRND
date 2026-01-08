using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using PahlBit;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public enum EnemyState
    {
        None,
        Idle,
        Patrol,
        Chase,
        Attack,
        Recover,
    }

    BaseObject mBase = null;
    EnemyStats mStats = null;
    BaseObject mPlayerTarget = null;

    CancellationTokenSource mAI_CTS;
    CancellationTokenSource mStateCTS;
    CancellationTokenSource mMoveCTS = null;

    EnemyState mState = EnemyState.Idle;

    [SerializeField] float _ThinkInterval = 0.5f;
    float DetectLossRange { get { return mStats.DetectRange * 1.5f; } }
    float DetectRange { get { return mStats.DetectRange; } }
    float AttackRange { get { return mStats.AttackRange; } }
    float MoveSpeed { get { return mStats.MoveSpeed; } }

    void Awake()
    {
        mBase = this.ExGetBase();
    }

    void Start()
    {
        mStats = mBase.GetComponentInChildren<EnemyDataMono>().Data.Stats;

        mBase.AnimHelper.AddEventMiddle(AnimStateNameHash.Attack, OnFireAttack);
        mBase.AnimHelper.AddEventLeave(AnimStateNameHash.Attack, OnEndAttack);
    }

    void OnEnable()
    {
        StartAI();
    }

    void OnDisable()
    {
        StopAI();
    }

    public void StartAI()
    {
        mAI_CTS?.Cancel();
        mAI_CTS?.Dispose();
        mAI_CTS = new CancellationTokenSource();

        mStateCTS?.Cancel();
        mStateCTS?.Dispose();
        mStateCTS = CancellationTokenSource.CreateLinkedTokenSource(mAI_CTS.Token);

        MainLoop(mAI_CTS.Token).Forget();
    }
    public void StopAI()
    {
        mMoveCTS?.Cancel();
        mMoveCTS?.Dispose();

        mStateCTS?.Cancel();
        mStateCTS?.Dispose();

        // 모든 상태 종료
        mAI_CTS?.Cancel();
        mAI_CTS?.Dispose();
    }

    async UniTask MainLoop(CancellationToken ct)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: ct);

        while (!ct.IsCancellationRequested)
        {
            try
            {
                switch (mState)
                {
                    case EnemyState.Idle:
                        ChangeState(await IdleMode(mStateCTS.Token));
                        break;

                    case EnemyState.Patrol:
                        ChangeState(await PatrolMode(mStateCTS.Token));
                        break;

                    case EnemyState.Chase:
                        ChangeState(await ChaseMode(mStateCTS.Token));
                        break;

                    case EnemyState.Attack:
                        ChangeState(await AttackMode(mStateCTS.Token));
                        break;

                    case EnemyState.Recover:
                        ChangeState(await RecoverMode(mStateCTS.Token));
                        break;
                    default:
                        ChangeState(EnemyState.Idle);
                        break;
                }
            }
            catch (OperationCanceledException)
            {
            }
        }
    }
    public void ChangeState(EnemyState enemyState)
    {
        mStateCTS?.Cancel();
        mStateCTS?.Dispose();
        mStateCTS = CancellationTokenSource.CreateLinkedTokenSource(mAI_CTS.Token);

        mState = enemyState;
    }
    async UniTask<EnemyState> IdleMode(CancellationToken ctx)
    {
        try
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: ctx);
            return EnemyState.Patrol;
        }
        finally
        {
        }
    }

    async UniTask<EnemyState> PatrolMode(CancellationToken ctx)
    {
        // ===== ENTER =====
        Stop();
        mPlayerTarget = null;

        try
        {
            DoPatrolMoving(ctx).Forget();
            mPlayerTarget = await DetectTarget(ctx);
            if (mPlayerTarget != null)
            {
                if (IsTargetInRange(AttackRange))
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
    async UniTask<EnemyState> ChaseMode(CancellationToken ctx)
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
    async UniTask<EnemyState> AttackMode(CancellationToken ctx)
    {
        try
        {
            Stop();
            mBase.AnimHelper.CrossFadeToState(AnimStateNameHash.Attack);
            await UniTask.WaitUntil(() => mBase.AnimHelper.GetCurrentStateNameHash(0) != (int)AnimStateNameHash.Attack, cancellationToken: ctx);
            return EnemyState.Recover;
        }
        finally
        {
            // EXIT
            // 공격 후 정리 (히트박스 off 등)
        }
    }
    async UniTask<EnemyState> RecoverMode(CancellationToken ctx)
    {
        try
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1.5f), cancellationToken: ctx);

            if (mPlayerTarget == null)
                return EnemyState.Patrol;
            else if (IsTargetInRange(AttackRange))
                return EnemyState.Attack;
            else if (IsTargetInRange(DetectLossRange))
                return EnemyState.Chase;
            else
                return EnemyState.Patrol;
        }
        finally
        {
        }
    }


    async UniTask<BaseObject> DetectTarget(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            BaseObject target = DetectPlayerAround(DetectRange);
            if (target != null)
            {
                return target;
            }
            else
            {
                await UniTask.Delay(TimeSpan.FromSeconds(_ThinkInterval), cancellationToken: ct);
            }
        }
        return null;
    }
    bool IsTargetInRange(float range)
    {
        if (mPlayerTarget == null)
            return false;

        float distSqr = Vector2.SqrMagnitude(mBase.Body.Center - mPlayerTarget.Body.Center);
        return distSqr <= range * range;
    }
    async UniTask IsAttackableTarget(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            if (IsTargetInRange(AttackRange))
            {
                break;
            }
            else
            {
                await UniTask.Delay(TimeSpan.FromSeconds(_ThinkInterval), cancellationToken: ct);
            }
        }
    }
    async UniTask IsLostTarget(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            if (mPlayerTarget == null)
            {
                break;
            }
            else if (!IsTargetInRange(DetectLossRange))
            {
                break;
            }
            else
            {
                await UniTask.Delay(TimeSpan.FromSeconds(_ThinkInterval), cancellationToken: ct);
            }
        }
    }


    async UniTask DoPatrolMoving(CancellationToken ct)
    {
        try
        {
            int curDir = mBase.Body.FrontDirInt;
            while (!ct.IsCancellationRequested)
            {
                Stop();
                await UniTask.Delay(TimeSpan.FromSeconds(MyUtils.RandomFloat(0.5f, 1.5f)), cancellationToken: ct);
                curDir *= -1;
                Turn(curDir);
                StartMoving(curDir * MoveSpeed);
                await UniTask.Delay(TimeSpan.FromSeconds(MyUtils.RandomFloat(1.5f, 2.5f)), cancellationToken: ct);
            }
        }
        catch (OperationCanceledException)
        {
            // LOG.trace(ex.Message);
        }
    }

    async UniTask DoChaseMoving(CancellationToken ct)
    {
        try
        {
            await UniTask.Yield(cancellationToken: ct);
            Stop();
            while (!ct.IsCancellationRequested && mPlayerTarget != null)
            {
                int curDir = mBase.Body.Center.x < mPlayerTarget.Body.Center.x ? 1 : -1;
                Turn(curDir);
                StartMoving(curDir * MoveSpeed);
                await UniTask.Delay(TimeSpan.FromSeconds(MyUtils.RandomFloat(0.5f, 1.5f)), cancellationToken: ct);
            }
        }
        catch (OperationCanceledException)
        {
            // LOG.trace(ex.Message);
        }
    }

    BaseObject DetectPlayerAround(float range)
    {
        Collider2D col = Physics2D.OverlapCircle(mBase.Body.Center, range, 1 << LayerID.Player);
        if (col != null)
        {
            return col.ExGetBase();
        }
        return null;
    }

    void Stop()
    {
        if (mBase != null)
        {
            mBase.AnimHelper.CrossFadeToState(AnimStateNameHash.Idle);
            mBase.Phy.Velocity = Vector2.zero;
        }

        mMoveCTS?.Cancel();
        mMoveCTS?.Dispose();
        mMoveCTS = null;
    }
    void Turn(float worldDir)
    {
        if (worldDir == 0) return;

        Vector3 front = worldDir > 0 ? Vector3.forward : Vector3.back;
        transform.rotation = Quaternion.LookRotation(front, transform.up);
    }
    void StartMoving(float velocity)
    {
        mMoveCTS?.Cancel();
        mMoveCTS?.Dispose();
        mMoveCTS = new CancellationTokenSource();

        MoveLoop(mMoveCTS.Token, velocity).Forget();
    }

    async UniTask MoveLoop(CancellationToken ct, float v)
    {
        mBase.AnimHelper.CrossFadeToState(AnimStateNameHash.Run);

        while (!ct.IsCancellationRequested)
        {
            mBase.Phy.VelocityX = v;
            await UniTask.Yield(ct);
        }

        mBase.Phy.Velocity = Vector2.zero;
    }

    void OnFireAttack(int idx)
    {
        LOG.trace("Enemy Attack Fired!");
        // 공격 판정
        if (mPlayerTarget != null)
        {
            // Vector2 toTarget = (mPlayerTarget.Body.Center - mBase.Body.Center).normalized;
            // mPlayerTarget.GetDamaged(1, toTarget);
        }
    }
    void OnEndAttack()
    {
    }

}
