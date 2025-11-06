using System.Collections;
using DG.Tweening;
using UnityEngine;


namespace PahlBit
{
    public class PlayerStateSkill : PlayerStateBase
    {
        [SerializeField] GameObject MeleePrefab;
        [SerializeField] GameObject SkillPrefab;
        [SerializeField] float StrongHitRange = 0.2f;
        [SerializeField] float FrontDetectRange = 8.0f;
        [SerializeField] float MoveDistance = 3.0f;

        BaseObject mTarget = null;
        float mTimeOfHit = 0;
        PlayerUnitInputType mNextActionInput = PlayerUnitInputType.None;

        public override void HandleInput()
        {
            if (PlayerInput.JustPressed(PlayerUnitInputType.Skill))
            {
                Base.StateMachine.ChangeState(this);
            }
        }

        public override void EnterState(object param)
        {
            base.EnterState(param);

            Base.Phy.LockGravity = true;
            mTimeOfHit = 0;
            IsStateCancelable = false;
            mNextActionInput = PlayerUnitInputType.None;

            // 현재 캐릭터와 주변 적 배치 상황에 따라 거기에 맞는 모션이 나간다.
            DecideAttackMotionByContext();

            // ExitStateOnEnd();
            AddEventEnter(AnimStateNameHash.ExitDummy, ChangeNextState);
        }

        void DecideAttackMotionByContext()
        {
            if (!PlayerMain.IsGrounded)
            {
                mTarget = FindAroundTarget();
                if (mTarget != null)
                {
                    PlayerMain.FlipToTarget(mTarget.transform);
                    Vector3 delta = mTarget.transform.position - Base.transform.position;
                    Vector2 velocity = Vector2.zero;
                    velocity.x = delta.x * 1.5f;
                    velocity.y = 20 + (delta.y * 0.5f);

                    Base.Phy.Velocity = velocity;
                }
                else
                {
                    Vector2 velocity = Vector2.zero;
                    velocity.x = 10 * Base.transform.right.x;
                    velocity.y = 20;
                    Base.Phy.Velocity = velocity;
                }

                Base.Phy.LockGravity = false;
                PlayAnimWithFire(AnimStateNameHash.Skill2, (idx) => OnFire(2));
                return;
            }

            mTarget = FindOverlappedTarget();
            if (mTarget != null)
            {
                float dir = Base.Body.FrontDir.x;
                Vector3 destPos = mTarget.transform.position;
                Base.Phy.MoveFootPosition(destPos);
                Base.Phy.Velocity = Vector2.zero;
                PlayAnimWithFire(AnimStateNameHash.Skill1, (idx) => OnFire(1));
                return;
            }

            mTarget = FindFrontTarget();
            if (mTarget != null)
            {
                PlayerMain.FlipToTarget(mTarget.transform);

                Vector3 delta = mTarget.transform.position - Base.transform.position;
                Vector2 force = Vector2.zero;
                force.x = delta.x * 1.2f;
                force.y = 0;

                Base.Phy.Velocity = Vector2.zero;
                Base.Phy.AddForce(force);
                PlayAnimWithFire(AnimStateNameHash.Skill, (idx) => OnFire(0));
                return;
            }

            Base.Phy.Velocity = Vector2.zero;
            PlayAnimWithFire(AnimStateNameHash.Skill, (idx) => OnFire(0));
        }

        public override void UpdateState()
        {
            base.UpdateState();

            if (PlayerInput.JustPressed(PlayerUnitInputType.Jump))
                mNextActionInput = PlayerUnitInputType.Jump;
            else if (PlayerInput.JustPressed(PlayerUnitInputType.Dash))
                mNextActionInput = PlayerUnitInputType.Dash;

            if (!PlayerInput.IsPressing(PlayerUnitInputType.Skill) && mTimeOfHit == 0)
            {
                mTimeOfHit = Time.time;
            }
        }

        void ChangeNextState()
        {
            if (mNextActionInput == PlayerUnitInputType.Jump)
                Base.StateMachine.ChangeState<PlayerStateJumpable>(null, true);
            else if (mNextActionInput == PlayerUnitInputType.Dash)
                Base.StateMachine.ChangeState<PlayerStateDash>(null, true);
            else
                ChangeControlableState();
        }


        public override void LeaveState()
        {
            base.LeaveState();
            Base.Phy.LockGravity = false;
            IsStateCancelable = true;
            Time.timeScale = 1;
            mNextActionInput = PlayerUnitInputType.None;
        }

        void OnFire(int attackType)
        {
            GameObject meleeSKill = InstantiateMelee();
            meleeSKill.GetComponentInChildren<InteractableCollider>().OnInteractEnter.AddListener((col) =>
            {
                EnemyBase enemy = col.GetComponentInParent<EnemyBase>();
                DoAttack(enemy, attackType);
            });

        }

        bool IsStrongHit()
        {
            return Time.time - mTimeOfHit < StrongHitRange;
        }

        void InstantiateSkill()
        {
            // 스킬 오브젝트 생성
            GameObject skill = Instantiate(SkillPrefab, transform.position, Quaternion.identity);
            Vector3 destPos = transform.position + new Vector3(transform.right.x * 10, 0, 0);
            skill.transform.DOMove(destPos, 0.5f).OnComplete(() => Destroy(skill));
            skill.GetComponentInChildren<InteractableCollider>().OnInteractEnter.AddListener((col) =>
            {
                EnemyBase enemy = col.GetComponentInParent<EnemyBase>();
                if (enemy != null)
                {
                    enemy.GetDamaged(20, Base.transform.right);
                    skill.transform.DOKill();
                    Destroy(skill);
                }
            });
        }
        GameObject InstantiateMelee()
        {
            // 스킬 오브젝트 생성
            Vector3 startPos = transform.position + new Vector3(transform.right.x, 0, 0);
            GameObject melee = Instantiate(MeleePrefab, startPos, Quaternion.identity);
            Destroy(melee, 0.1f);
            return melee;
        }

        BaseObject FindFrontTarget()
        {
            RaycastHit2D hit = Physics2D.Raycast(Base.Body.Center, Base.Body.FrontDir, FrontDetectRange, 1 << LayerID.Enemy);
            return hit.collider?.ExGetBase();
        }
        BaseObject FindOverlappedTarget()
        {
            Collider2D col = Physics2D.OverlapCircle(Base.Body.Center, 1, 1 << LayerID.Enemy);
            // if (col == null)
            // {
            //     RaycastHit2D hit = Physics2D.Raycast(Base.Body.Center, Base.Body.FrontDir, MoveDistance, 1 << LayerID.Enemy);
            //     return hit.collider?.ExGetBase();
            // }
            return col?.ExGetBase();
        }
        BaseObject FindAroundTarget()
        {
            Collider2D col = Physics2D.OverlapCircle(Base.Body.Center, FrontDetectRange, 1 << LayerID.Enemy);
            return col?.ExGetBase();
        }

        void DoAttack(EnemyBase enemy, int attackType)
        {
            if (enemy == null) return;

            if (IsStrongHit())
            {
                enemy.GetDamaged(3, Base.transform.right);
                PlayerMain.DoSlowEffect(0.1f, 0.04f, 0);
            }
            else
            {
                enemy.GetDamaged(1, Base.transform.right);
            }
        }
    }

}
