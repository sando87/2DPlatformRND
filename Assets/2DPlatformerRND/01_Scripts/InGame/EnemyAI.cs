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

    BaseObject mBase = null;
    BaseObject mPlayerTarget = null;

    CancellationTokenSource mAI_CTS;

    private void Awake()
    {
        mBase = this.ExGetBase();
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
        // 모든 상태 종료
        mAI_CTS?.Cancel();
        mAI_CTS?.Dispose();
    }

    async UniTask MainAIFlow(CancellationToken ct)
    {
        // 최초 상태
        while (!ct.IsCancellationRequested)
        {
            BaseObject target = DetectPlayerAround(_DetectRange);
            if (target != null)
            {
                // 타겟 발견 => 공격가능하면 바로 공격 아니면 추적모드 진입
                mPlayerTarget = target;
                if (Vector2.Distance(mBase.Body.Center, mPlayerTarget.Body.Center) <= _AttackRange)
                {
                    // 공격 모드 진입
                    mPlayerTarget = target;
                    await EnterAttackMode(ct);
                    await EnterRecoverMode(ct);
                }
                else
                {
                    // 추적 모드 진입
                    mPlayerTarget = target;
                    EnterChaseMode(ct).Forget();
                    await UniTask.WhenAny(IsAttackableTarget(ct), IsLostTarget(ct));
                }
            }
            else
            {
                // 주변 대상이 없으면 순찬모드 진입 후 탐색
                mPlayerTarget = null;
                EnterPatrolMode(ct).Forget();
                await IsDetectedTarget(ct);
            }
        }
    }


    async UniTask IsDetectedTarget(CancellationToken ct)
    {
        try
        {
            while (!ct.IsCancellationRequested)
            {
                BaseObject target = DetectPlayerAround(_DetectRange);
                if (target != null)
                {
                    break;
                }
                else
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(_ThinkInterval), cancellationToken: ct);
                }
            }
        }
        catch (OperationCanceledException)
        {
            // LOG.trace(ex.Message);
        }
    }
    async UniTask IsAttackableTarget(CancellationToken ct)
    {
        try
        {
            while (!ct.IsCancellationRequested)
            {
                BaseObject target = DetectPlayerAround(_AttackRange);
                if (target != null)
                {
                    break;
                }
                else
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(_ThinkInterval), cancellationToken: ct);
                }
            }
        }
        catch (OperationCanceledException)
        {
            // LOG.trace(ex.Message);
        }
    }
    async UniTask IsLostTarget(CancellationToken ct)
    {
        try
        {
            while (!ct.IsCancellationRequested)
            {
                BaseObject target = DetectPlayerAround(_DetectLossRange);
                if (target == null)
                {
                    break;
                }
                else
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(_ThinkInterval), cancellationToken: ct);
                }
            }
        }
        catch (OperationCanceledException)
        {
            // LOG.trace(ex.Message);
        }
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

    async UniTask EnterChaseMode(CancellationToken ct)
    {
    }

    async UniTask EnterAttackMode(CancellationToken ct)
    {
    }

    async UniTask EnterRecoverMode(CancellationToken ct)
    {
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
        Turn(moveHoriVelocity);
        mBase.Phy.VelocityX = moveHoriVelocity;
        mBase.AnimHelper.CrossFadeToState(AnimStateNameHash.Run);
    }

}
