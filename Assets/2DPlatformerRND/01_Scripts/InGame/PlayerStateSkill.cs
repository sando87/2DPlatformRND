using DG.Tweening;
using UnityEngine;


namespace PahlBit
{
    public class PlayerStateSkill : PlayerStateBase
    {
        [SerializeField] GameObject MeleePrefab;
        [SerializeField] GameObject SkillPrefab;
        [SerializeField] float StrongHitRange = 0.2f;

        BaseObject mTarget = null;
        float mTimeOfHit = 0;

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

            mTarget = FindFrontTarget();

            if (mTarget != null)
            {
                float dir = Base.Body.FrontDir.x;
                Vector3 destPos = mTarget.transform.position - new Vector3(3f * dir, 0, 0);
                Base.Phy.MoveFootPosition(destPos);
                Base.Phy.Velocity = new Vector2(3f * dir, 0f);
            }
            else
            {
                Base.Phy.Velocity = new Vector2(0f, 0f);
            }

            PlayAnimWithFire(AnimStateNameHash.Skill, (idx) =>
            {
                GameObject meleeSKill = InstantiateMelee();
                meleeSKill.GetComponentInChildren<InteractableCollider>().OnInteractEnter.AddListener((col) =>
                {
                    EnemyBase enemy = col.GetComponentInParent<EnemyBase>();
                    DoAttack(enemy);
                });
            });

            ExitStateOnEnd();
        }

        public override void UpdateState()
        {
            base.UpdateState();

            if (PlayerInput.JustPressed(PlayerUnitInputType.Skill) && mTimeOfHit == 0)
            {
                mTimeOfHit = Time.time;
            }
        }


        public override void LeaveState()
        {
            base.LeaveState();
            Base.Phy.LockGravity = false;
            IsStateCancelable = true;
            Time.timeScale = 1;
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
                    enemy.GetDamaged(20);
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
        void SlowEffect(float slowTimeScale, float duration)
        {
            Time.timeScale = slowTimeScale;
            this.ExDelayedCoroutine(duration, () => Time.timeScale = 1);
        }

        BaseObject FindFrontTarget()
        {
            RaycastHit2D hit = Physics2D.Raycast(Base.Body.Center, Base.Body.FrontDir, 7, 1 << LayerID.Enemy);
            return hit.collider?.ExGetBase();
        }

        void DoAttack(EnemyBase enemy)
        {
            if (enemy == null) return;

            if (IsStrongHit())
            {
                enemy.GetDamaged(3);
                SlowEffect(0.1f, 0.1f);
                IsStateCancelable = true;
            }
            else
            {
                enemy.GetDamaged(1);
            }
        }
    }

}
