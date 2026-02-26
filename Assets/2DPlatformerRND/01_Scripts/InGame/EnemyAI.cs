using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using PahlBit;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;

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
        Damaged,
        Death,
    }

    BaseObject mBase = null;
    SpecEnemy mSpec = null;
    BaseObject mPlayerTarget = null;

    CancellationTokenSource mAI_CTS;
    CancellationTokenSource mStateCTS;
    CancellationTokenSource mMoveCTS = null;

    EnemyState mState = EnemyState.Idle;

    [SerializeField] ProjectileBase MeleePrefab;
    [SerializeField] float _ThinkInterval = 0.5f;

    void Awake()
    {
        mBase = this.ExGetBase();
    }

    void Start()
    {
        mSpec = mBase.EnemyObj.Spec;

        mBase.Health.OnDamaged.AddListener(ChangeDamagedState);
        mBase.Health.OnDied.AddListener(ChangeDeathState);
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

        mState = EnemyState.Patrol;

        MainLoop(mAI_CTS.Token).Forget();
    }
    public void StopAI()
    {
        Stop();

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

                    case EnemyState.Damaged:
                        ChangeState(await DamagedMode(mStateCTS.Token));
                        break;

                    case EnemyState.Death:
                        ChangeState(await DeathMode(mStateCTS.Token));
                        break;

                    case EnemyState.None:
                    default:
                        return;
                }
            }
            catch (OperationCanceledException)
            {
            }
        }
    }
    public void ChangeState(EnemyState enemyState)
    {
        Stop();

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
            await mBase.AnimHelper.PlayAnim(AnimStateNameHash.Attack, ctx, OnFireAttack);
            OnEndAttack();
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
            else if (IsTargetInRange(mSpec.AttackRange))
                return EnemyState.Attack;
            else if (IsTargetInRange(mSpec.DetectLossRange))
                return EnemyState.Chase;
            else
                return EnemyState.Patrol;
        }
        finally
        {
        }
    }
    void ChangeDamagedState(DamagedResultInfo retInfo)
    {
        ChangeState(EnemyState.Damaged);
    }
    async UniTask<EnemyState> DamagedMode(CancellationToken ctx)
    {
        try
        {
            await mBase.AnimHelper.PlayAnim(AnimStateNameHash.Hit, ctx);
            return EnemyState.Recover;
        }
        finally
        {
        }
    }
    void ChangeDeathState()
    {
        ChangeState(EnemyState.Death);
    }
    async UniTask<EnemyState> DeathMode(CancellationToken ctx)
    {
        try
        {
            await mBase.AnimHelper.PlayAnim(AnimStateNameHash.Death, ctx);
            return EnemyState.None;
        }
        finally
        {
        }
    }


    async UniTask<BaseObject> DetectTarget(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            BaseObject target = DetectPlayerAround(mSpec.DetectRange);
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
            if (IsTargetInRange(mSpec.AttackRange))
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
            else if (!IsTargetInRange(mSpec.DetectLossRange))
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
                NodeNav node = GetCurrentNodeNav(mBase);
                if (node != null)
                {
                    NodeNavGroup nodeNavGroup = node.ParentGroup;
                    curDir *= -1;
                    Vector2 desPos = curDir > 0 ? nodeNavGroup.MostRightNode.CenterTopPos : nodeNavGroup.MostLeftNode.CenterTopPos;
                    MoveToDestPosition(mSpec.MoveSpeed, desPos).Forget();
                }
                await UniTask.Delay(TimeSpan.FromSeconds(MyUtils.RandomFloat(2f, 5f)), cancellationToken: ct);
            }
        }
        catch (OperationCanceledException)
        {
            // LOG.trace(ex.Message);
        }
    }

    PathInfo FindPath()
    {
        NodeNav node = GetCurrentNodeNav(mBase);
        if (node == null)
            return null;

        PathInfo path = PlatformerPathfinder.Instance.FindPath(node, mSpec.MoveSpeed);
        return path;
    }

    NodeNav GetCurrentNodeNav(BaseObject baseObject)
    {
        if (!baseObject.Phy.IsGrounded)
            return null;

        NodeNav node = PlatformerPathfinder.Instance.GetCurrentGroundNode(baseObject.Body.Rect);
        if (node != null)
            return node;

        return null;
    }

    bool IsNoWayToMove()
    {
        Vector2 pos = mBase.Body.FootFront + new Vector2(mBase.transform.right.x * 0.2f, 0);
        NodeNav frontNode = PlatformerPathfinder.Instance.GetNode(pos);
        if (frontNode != null && !frontNode.IsThin) // IsObstacled
            return true;

        pos.y -= 0.2f;
        NodeNav frontGroundNode = PlatformerPathfinder.Instance.GetNode(pos);
        if (frontGroundNode == null) // No ground ahead
            return true;

        return false;
    }

    async UniTask GotoPathDestPosition(PathInfo path, CancellationToken ct)
    {
        try
        {
            Stop();
            if (path.Transition.TransitionType == NodeTransitionType.JustJumpUp)
            {
                if (path.IsNoNeedToMove)
                {
                    await JustJumpUp(path.JumpForce);
                }
                else
                {
                    Vector2 worldWayPos = path.Transition.StartNode.CenterTopPos;
                    await MoveToDestPosition(mSpec.MoveSpeed, worldWayPos);
                    await UniTask.Delay(TimeSpan.FromSeconds(0.02f), cancellationToken: ct);
                    await JustJumpUp(path.JumpForce);
                }
            }
            else if (path.Transition.TransitionType == NodeTransitionType.DropDown)
            {
                if (path.IsNoNeedToMove)
                {
                    await DropDown();
                }
                else
                {
                    Vector2 worldWayPos = path.Transition.StartNode.CenterTopPos;
                    await MoveToDestPosition(mSpec.MoveSpeed, worldWayPos);
                    await UniTask.Delay(TimeSpan.FromSeconds(0.02f), cancellationToken: ct);
                    await DropDown();
                }
            }
            else if (path.Transition.TransitionType == NodeTransitionType.MovingJump)
            {
                Vector2 worldWayPos = path.Transition.StartNode.CenterTopPos;
                Vector2 worldDestPos = path.Transition.EndNode.CenterTopPos;
                await MoveToDestPosition(mSpec.MoveSpeed, worldWayPos);
                await UniTask.Delay(TimeSpan.FromSeconds(0.02f), cancellationToken: ct);
                await JumpMoving(path.JumpForce, mSpec.MoveSpeed, worldDestPos);
            }
            else if (path.Transition.TransitionType == NodeTransitionType.JumpAndMove)
            {
                Vector2 worldWayPos = path.Transition.StartNode.CenterTopPos;
                Vector2 worldDestPos = path.Transition.EndNode.CenterTopPos;
                await MoveToDestPosition(mSpec.MoveSpeed, worldWayPos);
                await UniTask.Delay(TimeSpan.FromSeconds(0.02f), cancellationToken: ct);
                await JumpAndMove(path.JumpForce, mSpec.MoveSpeed, worldDestPos);
            }
            else if (path.Transition.TransitionType == NodeTransitionType.WalkAndFall)
            {
                Vector2 worldWayPos = path.Transition.StartNode.CenterTopPos;
                Vector2 worldDestPos = path.Transition.EndNode.CenterTopPos;
                await MoveToDestPosition(mSpec.MoveSpeed, worldWayPos);
                await UniTask.Delay(TimeSpan.FromSeconds(0.02f), cancellationToken: ct);
                await MoveAndFall(mSpec.MoveSpeed, worldDestPos);
            }
            Stop();
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
                if (IsSameNodeGroupWithPlayer())
                {
                    int curDir = mBase.Body.Center.x < mPlayerTarget.Body.Center.x ? 1 : -1;
                    Turn(curDir);
                    StartMoving(curDir * mSpec.MoveSpeed);
                    await UniTask.Delay(TimeSpan.FromSeconds(MyUtils.RandomFloat(0.5f, 3.5f)), cancellationToken: ct);
                }
                else
                {
                    PathInfo path = FindPath();
                    if (path != null)
                    {
                        await GotoPathDestPosition(path, ct);
                    }

                    await UniTask.Delay(TimeSpan.FromSeconds(0.02f), cancellationToken: ct);
                }
            }
        }
        catch (OperationCanceledException)
        {
            // LOG.trace(ex.Message);
        }
    }

    bool IsSameNodeGroupWithPlayer()
    {
        if (mPlayerTarget == null)
            return false;

        NodeNav playerNode = GetCurrentNodeNav(mPlayerTarget);
        if (playerNode == null)
            return false;

        NodeNav baseNode = GetCurrentNodeNav(mBase);
        if (baseNode == null)
            return false;

        if (playerNode.ParentGroup == baseNode.ParentGroup)
            return true;

        return false;
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


    void CancelMoveCTS()
    {
        if (mMoveCTS != null)
        {
            mMoveCTS.Cancel();
            mMoveCTS.Dispose();
            mMoveCTS = null;
        }
    }
    void Stop()
    {
        CancelMoveCTS();

        if (mBase != null)
        {
            mBase.AnimHelper.CrossFadeToState(AnimStateNameHash.Idle);
            mBase.Phy.Velocity = Vector2.zero;
        }
    }
    void Turn(float worldDir)
    {
        if (worldDir == 0) return;

        Vector3 front = worldDir > 0 ? Vector3.forward : Vector3.back;
        transform.rotation = Quaternion.LookRotation(front, transform.up);
    }
    void StartMoving(float velocity)
    {
        CancelMoveCTS();
        mMoveCTS = new CancellationTokenSource();

        MoveToEnd(mMoveCTS.Token, velocity).Forget();
    }

    async UniTask MoveToEnd(CancellationToken ct, float v)
    {
        mBase.AnimHelper.CrossFadeToState(AnimStateNameHash.Run);

        while (!ct.IsCancellationRequested)
        {
            mBase.Phy.VelocityX = v;
            await UniTask.Yield(ct);
            if (IsNoWayToMove())
                break;
        }

        mBase.Phy.Velocity = Vector2.zero;
        mBase.AnimHelper.CrossFadeToState(AnimStateNameHash.Idle);
    }

    async UniTask MoveToDestPosition(float velocityX, Vector2 destPos)
    {
        CancelMoveCTS();
        mMoveCTS = new CancellationTokenSource();

        mBase.AnimHelper.CrossFadeToState(AnimStateNameHash.Run);
        Vector2 startPos = mBase.Body.Foot;
        int startDir = startPos.x < destPos.x ? 1 : -1;
        Turn(startDir);

        while (!mMoveCTS.Token.IsCancellationRequested)
        {
            if (IsArrivedDestPosition(destPos, startDir))
                break;

            mBase.Phy.VelocityX = velocityX * startDir;
            await UniTask.Yield(mMoveCTS.Token);
        }

        mBase.Phy.Velocity = Vector2.zero;
        mBase.AnimHelper.CrossFadeToState(AnimStateNameHash.Idle);
    }

    async UniTask MoveAndFall(float velocityX, Vector2 destPos)
    {
        CancelMoveCTS();
        mMoveCTS = new CancellationTokenSource();

        mBase.AnimHelper.CrossFadeToState(AnimStateNameHash.Run);
        Vector2 startPos = mBase.Body.Foot;
        int startDir = startPos.x < destPos.x ? 1 : -1;
        Turn(startDir);

        // 앞으로 그냥 걸어감(낙하될때까지)
        while (!mMoveCTS.Token.IsCancellationRequested)
        {
            mBase.Phy.VelocityX = velocityX * startDir;
            await UniTask.Yield(mMoveCTS.Token);

            if (IsArrivedDestPosition(destPos + new Vector2(0.5f * startDir, 0), startDir))
                break;
        }

        await UniTask.WaitUntil(() => mBase.Phy.IsGrounded, cancellationToken: mMoveCTS.Token);

        // 착지
        mBase.Phy.Velocity = Vector2.zero;
        mBase.AnimHelper.CrossFadeToState(AnimStateNameHash.Idle);

    }

    async UniTask JustJumpUp(float jumpForce)
    {
        CancelMoveCTS();
        mMoveCTS = new CancellationTokenSource();

        mBase.AnimHelper.CrossFadeToState(AnimStateNameHash.Jump);
        mBase.Phy.DoJump(jumpForce);
        await UniTask.WaitUntil(() => mBase.Phy.IsGrounded, cancellationToken: mMoveCTS.Token);

        mBase.Phy.Velocity = Vector2.zero;
        mBase.AnimHelper.CrossFadeToState(AnimStateNameHash.Idle);
    }
    async UniTask DropDown()
    {
        CancelMoveCTS();
        mMoveCTS = new CancellationTokenSource();

        mBase.AnimHelper.CrossFadeToState(AnimStateNameHash.Jump);

        mBase.Body.LockThinPlatformMomentarily();
        await UniTask.WaitUntil(() => !mBase.Body.LockThinPlatform && mBase.Phy.IsGrounded, cancellationToken: mMoveCTS.Token);

        mBase.Phy.Velocity = Vector2.zero;
        mBase.AnimHelper.CrossFadeToState(AnimStateNameHash.Idle);
    }
    async UniTask JumpMoving(float jumpForce, float velocityX, Vector2 destPos)
    {
        CancelMoveCTS();
        mMoveCTS = new CancellationTokenSource();

        mBase.AnimHelper.CrossFadeToState(AnimStateNameHash.Jump);
        Vector2 startPos = mBase.Body.Foot;
        int startDir = startPos.x < destPos.x ? 1 : -1;
        Turn(startDir);
        mBase.Phy.DoJump(jumpForce);

        while (!mMoveCTS.Token.IsCancellationRequested)
        {
            mBase.Phy.VelocityX = velocityX * startDir;
            await UniTask.Yield(mMoveCTS.Token);

            if (mBase.Phy.IsGrounded)
                break;
        }

        mBase.Phy.Velocity = Vector2.zero;
        mBase.AnimHelper.CrossFadeToState(AnimStateNameHash.Idle);
    }

    async UniTask JumpAndMove(float jumpForce, float velocityX, Vector2 destPos)
    {
        CancelMoveCTS();
        mMoveCTS = new CancellationTokenSource();

        mBase.AnimHelper.CrossFadeToState(AnimStateNameHash.Jump);
        Vector2 startPos = mBase.Body.Foot;
        int startDir = startPos.x < destPos.x ? 1 : -1;
        Turn(startDir);
        mBase.Phy.DoJump(jumpForce);

        while (!mMoveCTS.Token.IsCancellationRequested)
        {
            if (mBase.Phy.VelocityY < 0)
            {
                if (!IsArrivedDestPosition(destPos, startDir))
                    mBase.Phy.VelocityX = velocityX * startDir;
                else
                    mBase.Phy.VelocityX = 0;
            }

            await UniTask.Yield(mMoveCTS.Token);

            if (mBase.Phy.IsGrounded)
                break;
        }

        mBase.Phy.Velocity = Vector2.zero;
        mBase.AnimHelper.CrossFadeToState(AnimStateNameHash.Idle);
    }

    bool IsArrivedDestPosition(Vector2 destPos, int startDir)
    {
        Vector2 curPos = mBase.Body.Foot;
        if (Mathf.Abs(curPos.x - destPos.x) <= 0.2f)
            return true;

        int currentDir = curPos.x < destPos.x ? 1 : -1;
        return startDir != currentDir;
    }

    void OnFireAttack(int idx)
    {
        DoFireAttack();
    }
    void OnEndAttack()
    {
    }

    public void DoFireAttack()
    {
        // 스킬 오브젝트 생성
        Vector2 startPos = mBase.Body.Center + new Vector2(transform.right.x, 0);
        ProjectileBase obj = ProjectileBase.Create(MeleePrefab, startPos, mBase.transform.rotation, mBase.gameObject.layer);
        obj.OnHit.AddListener((col) =>
        {
            // 충돌 시 처리할 내용
            Health health = col.ExGetBase().GetComponentInChildren<Health>();
            if (health != null)
            {
                float damage = mSpec.BaseAttack;
                health.GetDamaged(damage);
            }
        });
    }

}
