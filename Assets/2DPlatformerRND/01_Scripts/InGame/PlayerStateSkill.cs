using DG.Tweening;
using UnityEngine;


namespace PahlBit
{
    public class PlayerStateSkill : PlayerStateBase
    {
        [SerializeField] GameObject MeleePrefab;
        [SerializeField] GameObject SkillPrefab;

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

            Base.Phy.Velocity = new Vector2(0f, 0f);
            PlayAnimWithFire(AnimStateNameHash.Skill, (idx) =>
            {
                InstantiateMelee(3);
            });

            ExitStateOnEnd();
        }

        public override void LeaveState()
        {
            base.LeaveState();
            Base.Phy.LockGravity = false;
            Time.timeScale = 1;
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
        void InstantiateMelee(int damage)
        {
            // 스킬 오브젝트 생성
            Vector3 startPos = transform.position + new Vector3(transform.right.x, 0, 0);
            GameObject melee = Instantiate(MeleePrefab, startPos, Quaternion.identity);
            Destroy(melee, 0.1f);
            melee.GetComponentInChildren<InteractableCollider>().OnInteractEnter.AddListener((col) =>
            {
                EnemyBase enemy = col.GetComponentInParent<EnemyBase>();
                if (enemy != null)
                {
                    enemy.GetDamaged(damage);
                    SlowEffect(0.1f, 0.1f);
                }
            });
        }
        void SlowEffect(float slowTimeScale, float duration)
        {
            Time.timeScale = slowTimeScale;
            this.ExDelayedCoroutine(duration, () => Time.timeScale = 1);
        }
    }
    
}
