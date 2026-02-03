using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NaughtyAttributes;
using PahlBit;
using UnityEngine;
using UnityEngine.InputSystem;

public class SkillArrow : SkillObject
{
    [SerializeField] PlayerStateGeneral SkillMotion;
    [SerializeField] ProjectileBase ProjectilePrefab;

    public override bool IsCastable()
    {
        return SkillMotion.IsChangable();
    }

    public override void UpdateSkill()
    {
        base.UpdateSkill();

        if (mInput.JustPressed(GetCurrentInputType()) && IsCastable())
        {
            mBaseObj.StateMachine.ChangeState(SkillMotion, (Action)DoFire);
        }
    }

    public override void DoFire()
    {
        base.DoFire();

        Vector2 startPos = mBaseObj.Body.Center + new Vector2(transform.right.x, 0);
        FireMultiShot((int)BaseStats.ProjectileCount, startPos, mBaseObj.transform.rotation, 90);
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
            Health health = col.ExGetBase().GetComponentInChildren<Health>();
            if (health != null)
            {
                float damage = BaseStats.Attack;
                health.GetDamaged(damage);

                AttackResult result = new AttackResult()
                {
                    Target = col.ExGetBase(),
                    IsKilled = health.IsDead,
                };
                mBaseObj.GetComponentInChildren<BattleDispatcher>()?.DispatchAttackResult(result);

                proj.DoEndProjectile();
            }
        });
    }



}
