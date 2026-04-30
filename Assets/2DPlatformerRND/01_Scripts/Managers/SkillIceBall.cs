using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NaughtyAttributes;
using PahlBit;
using UnityEngine;
using UnityEngine.InputSystem;

public class SkillIceBall : SkillBase
{
    [SerializeField] PlayerStateGeneral SkillMotion;
    [SerializeField] ProjectileBase ProjectilePrefab;

    float mAimingDegreeOffset = 0;

    public override bool IsCastable()
    {
        return base.IsCastable() && SkillMotion.IsChangable();
    }

    public override void OnPressingInput()
    {
        base.OnPressingInput();

        if (IsCastable())
        {
            mBaseObj.TurnToFloatX(mBaseObj.Input.MoveX);
            mBaseObj.StateMachine.TryChangeState(SkillMotion, (Action)DoFire);
        }
    }

    public override void DoFire()
    {
        base.DoFire();

        UseMana();
        StartCooltime();

        Vector2 inputXY = mBaseObj.Input.MoveXY;
        if (inputXY.magnitude > 0.1f)
            mAimingDegreeOffset = Mathf.Atan2(inputXY.y, Mathf.Abs(inputXY.x)) * Mathf.Rad2Deg * 0.5f;
        else
            mAimingDegreeOffset = 0;

        List<BaseObject> mTargets = new();
        Vector2 startPos = mBaseObj.Body.Center + new Vector2(transform.right.x, 0);
        FireMultiShot((int)Spec.ProjectileCount, startPos, mBaseObj.transform.rotation, 90, mTargets);
    }


    void FireMultiShot(int arrowCount, Vector2 startPos, Quaternion baseRotation, float maxSpreadAngle, List<BaseObject> targets)
    {
        if (arrowCount <= 0)
            return;

        float stepAngle = 20;
        float totalAngle = stepAngle * (arrowCount - 1);
        if (totalAngle > maxSpreadAngle)
        {
            stepAngle = maxSpreadAngle / (arrowCount - 1);
        }

        for (int i = 0; i < arrowCount; i++)
        {
            float offsetIndex = i - (arrowCount - 1) / 2f;
            float degree = mAimingDegreeOffset + (offsetIndex * stepAngle);

            Quaternion rot = baseRotation * Quaternion.Euler(0f, 0f, degree);

            ProjectileBase proj = ProjectileBase.Create(
                ProjectilePrefab,
                startPos,
                rot,
                mBaseObj.gameObject.layer
            );

            ApplySkillStatsToProjectile(proj);

            RegistOnHitEvent(proj, targets);
        }
    }

    void RegistOnHitEvent(ProjectileBase proj, List<BaseObject> targets)
    {
        proj.OnHit.AddListener((col) =>
        {
            // 주변 지형과 충돌 시
            if (col.gameObject.layer == PahlBit.LayerID.Terrain)
            {
                proj.DoEndProjectile();
                return;
            }

            // 충돌 시 처리할 내용
            Health health = col.ExGetCompInBase<Health>();
            if (health != null)
            {
                // 충돌 시 처리할 내용
                BaseObject target = col.ExGetBase();
                if (targets.Contains(target))
                    return;

                targets.Add(target);

                DamageInfo damageInfo = Spec.CalcCurrentDamages();
                health.GetDamaged(damageInfo);

                AttackResult result = new AttackResult()
                {
                    Target = col.ExGetBase(),
                    IsKilled = health.IsDead,
                };
                mBaseObj.GetComponentInChildren<BattleDispatcher>()?.DispatchAttackResult(result);
            }

            // proj.DoEndProjectile();
        });
    }


    public void DoCastSkill()
    {
        // 스킬 오브젝트 생성
        Vector2 startPos = mBaseObj.Body.Center + new Vector2(transform.right.x, 0);
        ProjectileBase proj = ProjectileBase.Create(ProjectilePrefab, startPos, mBaseObj.transform.rotation, mBaseObj.gameObject.layer);

        ApplySkillStatsToProjectile(proj);

        proj.OnHit.AddListener((col) =>
        {
            Health health = col.ExGetCompInBase<Health>();
            if (health != null)
            {
                DamageInfo damageInfo = Spec.CalcCurrentDamages();
                health.GetDamaged(damageInfo);

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
