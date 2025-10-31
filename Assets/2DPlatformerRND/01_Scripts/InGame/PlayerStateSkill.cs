using DG.Tweening;
using UnityEngine;


namespace PahlBit
{
    public class PlayerStateSkill : PlayerStateBase
    {
        [SerializeField] float _FireDelay = 0.4f;
        [SerializeField] GameObject SkillPrefab;

        public override void HandleInput()
        {
            if (Base.PlayerInput.JustPressed(PlayerUnitInputType.Skill) && Base.PlayerCTRL.IsGrounded)
            {
                Base.StateMachine.ChangeState(this);
            }
        }

        public override void EnterState(object param)
        {
            base.EnterState(param);

            Base.AnimHelper.CrossFadeToState("PlayerSkill", 0);
            Base.PlayerCTRL.Velocity = new Vector2(0f, 0f);

            this.ExDelayedCoroutine(_FireDelay, () =>
            {
                InstantiateSkill();
            });

            this.ExDelayedCoroutine(0.8f, () =>
            {
                ChangeStateToIdle();
            });
        }

        void InstantiateSkill()
        {
            // 스킬 오브젝트 생성
            GameObject skill = Instantiate(SkillPrefab, transform.position, Quaternion.identity);
            Vector3 destPos = transform.position + new Vector3(transform.localScale.x * 10, 0, 0);
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
    }
}
