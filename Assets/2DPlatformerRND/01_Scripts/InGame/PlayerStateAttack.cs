using UnityEngine;


namespace PahlBit
{
    public class PlayerStateAttack : PlayerStateBase
    {
        [SerializeField] GameObject MeleePrefab;

        public override void HandleInput()
        {
            if (PlayerInput.JustPressed(PlayerUnitInputType.Attack))
            {
                ChangeStateToThis();
            }
        }

        public override void EnterState(object param)
        {
            base.EnterState(param);

            PlayAnimWithFire(AnimStateNameHash.Melee, (idx) =>
            {
                InstantiateMelee();
            });
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
