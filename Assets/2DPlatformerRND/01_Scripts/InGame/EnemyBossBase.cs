using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using PahlBit;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyBossBase : EnemyBase
{
    [SerializeField] float _ManaRegen = 0.2f;

    [Header("AttackMelee")]
    [SerializeField] ProjectileBase MeleePrefab;
    [SerializeField] Transform FirePositionMelee = null;

    [Header("AttackA")]
    [SerializeField] float _ManaUse = 3;
    [SerializeField] int _ProjectileCount = 5;
    [SerializeField] float _PhyAttackA = 3;
    [SerializeField] ProjectileBase ProjectilePrefabA;
    [SerializeField] Transform FirePositionA = null;

    [Header("AttackB")]
    [SerializeField] int _Cooltime = 5;
    [SerializeField] float _PhyAttackB = 5;
    [SerializeField] ProjectileBase ProjectilePrefabB;
    [SerializeField] Transform FirePositionB = null;

    public SpecOption Option { get => mItem.Option; }
    public float ManaRegen { get => _ManaRegen; }
    public bool IsAwaked { get; set; } = false;

    Health mHealth;
    float mTime_AttackB = 0;
    ItemInfo mItem = null;

    public void DoAwakeBossWithItem(ItemInfo item)
    {
        IsAwaked = true;
        mItem = item;
        mBase.Spec.LinkOption(Option);

        float attackSpeed = mBase.EnemyObj.Spec.Option.AttackSpeedUp.Multiplier;
        mBase.AnimHelper.SetParamFloat(AnimatorParams.AttackSpeed, attackSpeed);

        float moveSpeed = mBase.EnemyObj.Spec.Option.MoveSpeedUp.Multiplier;
        mBase.AnimHelper.SetParamFloat(AnimatorParams.MoveSpeed, moveSpeed);

        mHealth.UpdateMaxStats(false);
    }

    protected override void Start()
    {
        base.Start();

        mHealth = mBase.Health;
    }

    public override void OnDeath()
    {
        mBase.Body.LockBody = true;

        mItem.IsEquipable = true;
        GameSystem.DoSave_UserSaveData();
        ItemObject.CreateNewItem(mBase.Body.Center, Quaternion.identity, mItem);
    }

    public bool IsAttackable_AttackA()
    {
        return mHealth.MaxMana >= _ManaUse;
    }
    public bool IsAttackable_AttackB()
    {
        return MyUtils.IsCooltimeOver(mTime_AttackB, _Cooltime);
    }

    public void DoFire_AttackA()
    {
        mHealth.UseMana(_ManaUse);

        Vector2 startPos = FirePositionA.position;
        int projCount = (_ProjectileCount + Option.ProjectileCountUp).ToInt();
        FireMultiShot(projCount, startPos, mBase.transform.rotation, 90);

        FireMelee(FirePositionMelee.position);
    }

    public void DoFire_AttackB()
    {
        mTime_AttackB = Time.time;

        Vector2 startPos = FirePositionB.position;

        ProjectileBase proj = ProjectileBase.Create(
            ProjectilePrefabB,
            startPos,
            mBase.transform.rotation,
            mBase.gameObject.layer
        );

        // proj.Stats.MoveSpeed *= Option.ProjectileSpeedUp;
        // proj.Stats.FireAngle = 0;
        // proj.Stats.AttackRange *= Option.AttackRangeUp;
        // proj.Stats.SplashRange *= Option.SplashRangeUp;
        // proj.Stats.Duration *= Option.DurationUp;

        RegistOnHitEvent(proj, _PhyAttackB, false);
    }

    void FireMelee(Vector2 startPos)
    {
        ProjectileBase proj = ProjectileBase.Create(
            MeleePrefab,
            startPos,
            mBase.transform.rotation,
            mBase.gameObject.layer
        );

        proj.OnHit.AddListener((col) =>
        {
            // 적과 충돌 시 처리할 내용
            Health health = col.ExGetCompInBase<Health>();
            if (health != null)
            {
                // 충돌 시 처리할 내용
                BaseObject target = col.ExGetBase();

                DamageInfo damageInfo = new DamageInfo();
                damageInfo.PhyDamage = _PhyAttackA + Option.BaseAttackAdd;
                health.GetDamaged(damageInfo);
            }
        });
    }

    void FireMultiShot(int arrowCount, Vector2 startPos, Quaternion baseRotation, float maxSpreadAngle)
    {
        if (arrowCount <= 0)
            return;

        float stepAngle = 10;
        float totalAngle = stepAngle * (arrowCount - 1);
        if (totalAngle > maxSpreadAngle)
        {
            stepAngle = maxSpreadAngle / (arrowCount - 1);
        }

        for (int i = 0; i < arrowCount; i++)
        {
            float offsetIndex = i - (arrowCount - 1) / 2f;
            float degree = offsetIndex * stepAngle;

            Quaternion rot = baseRotation * Quaternion.Euler(0f, 0f, degree);

            ProjectileBase proj = ProjectileBase.Create(
                ProjectilePrefabA,
                startPos,
                rot,
                mBase.gameObject.layer
            );

            proj.Stats.MoveSpeed *= Option.ProjectileSpeedUp;
            proj.Stats.FireAngle = 0;
            proj.Stats.AttackRange *= Option.AttackRangeUp;
            proj.Stats.SplashRange *= Option.SplashRangeUp;
            proj.Stats.Duration *= Option.DurationUp;

            RegistOnHitEvent(proj, _PhyAttackA, true);
        }
    }

    void RegistOnHitEvent(ProjectileBase proj, float damage, bool IsAttackA)
    {
        proj.OnHit.AddListener((col) =>
        {
            // 주변 지형과 충돌 시
            if (col.gameObject.layer == PahlBit.LayerID.Terrain && IsAttackA)
            {
                proj.DoEndProjectile();
                return;
            }

            // 적과 충돌 시 처리할 내용
            Health health = col.ExGetCompInBase<Health>();
            if (health != null)
            {
                // 충돌 시 처리할 내용
                BaseObject target = col.ExGetBase();

                DamageInfo damageInfo = new DamageInfo();
                damageInfo.PhyDamage = damage + Option.BaseAttackAdd;
                health.GetDamaged(damageInfo);

                if (IsAttackA)
                    proj.DoEndProjectile();
            }
        });
    }
}
