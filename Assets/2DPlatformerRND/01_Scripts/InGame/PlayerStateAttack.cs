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

            Base.Phy.Velocity = Vector2.zero;
            Base.Phy.LockGravity = true;

            Base.AnimHelper.SetParamInt("ComboStep", 0);
            PlayAnim(AnimStateNameHash.MeleeA);

            AddEventMiddle(AnimStateNameHash.MeleeA, (idx) => InstantiateMelee(1));
            AddEventMiddle(AnimStateNameHash.MeleeB, (idx) => InstantiateMelee(1));
            AddEventMiddle(AnimStateNameHash.MeleeC, (idx) => InstantiateMelee(1));
            AddEventMiddle(AnimStateNameHash.MeleeD, (idx) => InstantiateMelee(2));

            ExitStateOnEnd();
        }

        public override void UpdateState()
        {
            base.UpdateState();

            if (PlayerInput.JustPressed(PlayerUnitInputType.Attack))
            {
                if (Base.AnimHelper.GetCurrentStateNameHash(Layer) == AnimStateNameHash.MeleeA)
                {
                    Base.AnimHelper.SetParamInt("ComboStep", 1);
                }
                else if (Base.AnimHelper.GetCurrentStateNameHash(Layer) == AnimStateNameHash.MeleeB)
                {
                    Base.AnimHelper.SetParamInt("ComboStep", 2);
                }
                else if (Base.AnimHelper.GetCurrentStateNameHash(Layer) == AnimStateNameHash.MeleeC)
                {
                    Base.AnimHelper.SetParamInt("ComboStep", 3);
                }
            }
        }

        public override void LeaveState()
        {
            base.LeaveState();
            Base.Phy.LockGravity = false;
            Base.AnimHelper.SetParamInt("ComboStep", 0);
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
                }
            });
        }


    }
}
