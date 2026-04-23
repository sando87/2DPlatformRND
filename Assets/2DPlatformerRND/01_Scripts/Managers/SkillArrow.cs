using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NaughtyAttributes;
using PahlBit;
using UnityEngine;
using UnityEngine.InputSystem;

public class SkillArrow : SkillBase
{
    [SerializeField] PlayerStateGeneral SkillMotion;
    [SerializeField] ProjectileBase ProjectilePrefab;

    private float mMotionSpeed = 0;
    private int mProjectileCount = 0;
    private DamageInfo mDamageInfo = new DamageInfo();
    private PercentUp mCriticalRate = new PercentUp();
    private PercentUp mCriticalAttack = new PercentUp();

    public override bool IsCastable()
    {
        return base.IsCastable() && SkillMotion.IsChangable();
    }

    public override void OnPressingInput()
    {
        base.OnPressingInput();

        if (IsCastable())
        {
            UpdateSpec();
            mBaseObj.StateMachine.TryChangeState(SkillMotion, (Action)DoFire);
        }
    }

    void UpdateSpec()
    {
        mMotionSpeed = mBaseObj.PlayerObj.Spec.Option.AttackSpeedUp.Multiplier;
        mProjectileCount = (int)Spec.ProjectileCount;
        mDamageInfo = Spec.CalcCurrentDamages();
        mCriticalRate = mBaseObj.PlayerObj.Spec.Option.CriticalRate;
        mCriticalAttack = mBaseObj.PlayerObj.Spec.Option.CriticalAttack;
    }

    public override void DoFire()
    {
        base.DoFire();

        UseMana();
        StartCooltime();

        Vector2 startPos = mBaseObj.Body.Center + new Vector2(transform.right.x, 0);
        FireMultiShot(mProjectileCount, startPos, mBaseObj.transform.rotation, 90);
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
            float angle = offsetIndex * stepAngle;

            Quaternion rot = baseRotation * Quaternion.Euler(0f, 0f, angle);

            ProjectileBase proj = ProjectileBase.Create(
                ProjectilePrefab,
                startPos,
                rot,
                mBaseObj.gameObject.layer
            );

            ApplySkillStatsToProjectile(proj);

            RegistOnHitEvent(proj);
        }
    }

    void RegistOnHitEvent(ProjectileBase proj)
    {
        proj.OnHit.AddListener((col) =>
        {
            // 충돌 시 처리할 내용
            Health health = col.ExGetCompInBase<Health>();
            if (health != null)
            {
                health.GetDamaged(mDamageInfo);

                AttackResult result = new AttackResult()
                {
                    Target = col.ExGetBase(),
                    IsKilled = health.IsDead,
                };
                mBaseObj.GetComponentInChildren<BattleDispatcher>()?.DispatchAttackResult(result);
            }

            proj.DoEndProjectile();
        });
    }



}
