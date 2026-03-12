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

    List<BaseObject> mTargets = new();

    public override bool IsCastable()
    {
        return SkillMotion.IsChangable();
    }

    public override void UpdateSkill()
    {
        base.UpdateSkill();

        if (mInput.JustPressed(GetCurrentInputType()) && IsCastable())
        {
            mTargets.Clear();
            mBaseObj.StateMachine.TryChangeState(SkillMotion, (Action)DoFire);
        }
    }

    public override void DoFire()
    {
        base.DoFire();

        DoCastSkill();
    }

    public void DoCastSkill()
    {
        // 스킬 오브젝트 생성
        Vector2 startPos = mBaseObj.Body.Center + new Vector2(transform.right.x, 0);
        ProjectileBase proj = ProjectileBase.Create(ProjectilePrefab, startPos, mBaseObj.transform.rotation, mBaseObj.gameObject.layer);

        ApplySkillStatsToProjectile(proj);

        proj.OnHit.AddListener((col) =>
        {
            // 충돌 시 처리할 내용
            BaseObject target = col.ExGetBase();
            if (mTargets.Contains(target))
                return;

            mTargets.Add(target);

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
