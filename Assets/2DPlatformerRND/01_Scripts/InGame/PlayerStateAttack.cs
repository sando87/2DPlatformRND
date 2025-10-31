using UnityEngine;


namespace PahlBit
{
    public class PlayerStateAttack : PlayerStateBase
    {
        [SerializeField] float _FireDelay = 0.4f;
        [SerializeField] GameObject MeleePrefab;

        public override void HandleInput()
        {
            if (Base.PlayerInput.JustPressed(PlayerUnitInputType.Attack) && Base.PlayerCTRL.IsGrounded)
            {
                Base.StateMachine.ChangeState(this);
            }
        }

        public override void EnterState(object param)
        {
            base.EnterState(param);

            Base.AnimHelper.CrossFadeToState("PlayerMelee", 0);
            Base.PlayerCTRL.Velocity = new Vector2(0f, 0f);

            this.ExDelayedCoroutine(_FireDelay, () =>
            {
                InstantiateMelee();
            });

            // this.ExDelayedCoroutine(0.8f, () =>
            // {
            //     Base.StateMachine.ChangeStateToIdle();
            // });
        }

        void InstantiateMelee()
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
                    enemy.GetDamaged(10);
                }
            });
        }
    }
}
