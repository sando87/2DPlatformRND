using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using PahlBit;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] float _DetectLossRange = 8f;
    [SerializeField] float _DetectRange = 5f;
    [SerializeField] float _AttackRange = 2f;
    [SerializeField] float _ThinkInterval = 0.1f;
    [SerializeField] float _MoveSpeed = 3f;

    BaseObject mBase = null;
    BaseObject mPlayerTarget = null;

    CancellationTokenSource mAI_CTS;
    CancellationTokenSource mStateCTS;

    private void Awake()
    {
        mBase = this.ExGetBase();
    }

    void Start()
    {
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
        MainAIFlow(mAI_CTS.Token).Forget();
    }
    public void StopAI()
    {
        mStateCTS?.Cancel();
        mStateCTS?.Dispose();

        // 모든 상태 종료
        mAI_CTS?.Cancel();
        mAI_CTS?.Dispose();
    }

    async UniTask MainAIFlow(CancellationToken ct)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: ct);
        Stop();
        await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: ct);

        // 최초 상태
        while (!ct.IsCancellationRequested)
        {
            // 주변 대상이 없으면 순찬모드 진입 후 탐색
            mPlayerTarget = null;
            CancelStateResetCTS(ct);
            EnterPatrolMode(mStateCTS.Token).Forget();
            BaseObject target = await DetectTarget(mStateCTS.Token, _DetectRange);
            if (target == null)
                continue;
            else
                mPlayerTarget = target;

            // 타겟 발견시 공격범위 밖이면 추격모드 진입
            bool isAttackable = IsTargetInRange(_AttackRange);
            if (!isAttackable)
            {
                CancelStateResetCTS(ct);
                EnterChaseMode(mStateCTS.Token).Forget();
                int returnIdx = await UniTask.WhenAny(IsAttackableTarget(mStateCTS.Token), IsLostTarget(mStateCTS.Token));
                if (mStateCTS.IsCancellationRequested)
                    break;

                isAttackable = returnIdx == 0;
            }

            if (isAttackable) // 공격 모드 진입
            {
                CancelStateResetCTS(ct);
                await EnterAttackMode(mStateCTS.Token);
                await EnterRecoverMode(mStateCTS.Token);
            }
        }
    }


    async UniTask<BaseObject> DetectTarget(CancellationToken ct, float range)
    {
        while (!ct.IsCancellationRequested)
        {
            BaseObject target = DetectPlayerAround(range);
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
            if (IsTargetInRange(_AttackRange))
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
            if (!IsTargetInRange(_DetectLossRange))
            {
                break;
            }
            else
            {
                await UniTask.Delay(TimeSpan.FromSeconds(_ThinkInterval), cancellationToken: ct);
            }
        }
    }

    void CancelStateResetCTS(CancellationToken parent)
    {
        mStateCTS?.Cancel();
        mStateCTS?.Dispose();
        mStateCTS = CancellationTokenSource.CreateLinkedTokenSource(parent);
    }

    async UniTask EnterPatrolMode(CancellationToken ct)
    {
        try
        {
            int curDir = mBase.Body.FrontDirInt;
            while (!ct.IsCancellationRequested)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(MyUtils.RandomFloat(0.5f, 1.5f)), cancellationToken: ct);
                curDir *= -1;
                Turn(curDir);
                Move(curDir * _MoveSpeed);
                await UniTask.Delay(TimeSpan.FromSeconds(MyUtils.RandomFloat(1.5f, 2.5f)), cancellationToken: ct);
                Stop();
            }
        }
        catch (OperationCanceledException)
        {
            // LOG.trace(ex.Message);
        }
    }

    async UniTask EnterChaseMode(CancellationToken ct)
    {
        try
        {
            while (!ct.IsCancellationRequested && mPlayerTarget != null)
            {
                int curDir = mBase.Body.Center.x < mPlayerTarget.Body.Center.x ? 1 : -1;
                Turn(curDir);
                Move(curDir * _MoveSpeed);
                await UniTask.Delay(TimeSpan.FromSeconds(MyUtils.RandomFloat(0.5f, 1.5f)), cancellationToken: ct);
            }
        }
        catch (OperationCanceledException)
        {
            // LOG.trace(ex.Message);
        }
    }

    async UniTask EnterAttackMode(CancellationToken ct)
    {
        Stop();
        mBase.AnimHelper.CrossFadeToState(AnimStateNameHash.Attack);
        await UniTask.WaitUntil(() => mBase.AnimHelper.GetCurrentStateNameHash(0) != (int)AnimStateNameHash.Attack, cancellationToken: ct);
    }

    async UniTask EnterRecoverMode(CancellationToken ct)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(1.5f), cancellationToken: ct);
    }



    BaseObject DetectPlayerAround(float range)
    {
        Collider2D col = Physics2D.OverlapCircle(mBase.Body.Center, range, LayerID.Player);
        if (col != null)
        {
            return col.ExGetBase();
        }
        return null;
    }

    protected void Stop()
    {
        mBase.AnimHelper.CrossFadeToState(AnimStateNameHash.Idle);
        mBase.Phy.Velocity = Vector2.zero;
    }
    protected void Turn(float worldDir)
    {
        if (worldDir == 0) return;

        Vector3 front = worldDir > 0 ? Vector3.forward : Vector3.back;
        transform.rotation = Quaternion.LookRotation(front, transform.up);
    }
    protected void Move(float moveHoriVelocity)
    {
        mBase.Phy.VelocityX = moveHoriVelocity;
        mBase.AnimHelper.CrossFadeToState(AnimStateNameHash.Run);
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
