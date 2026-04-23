using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NaughtyAttributes;
using PahlBit;
using UnityEngine;
using UnityEngine.InputSystem;

public class SkillWaterBomb : SkillBase
{
    [SerializeField] PlayerStateGeneral SkillMotion;
    [SerializeField] ProjectileBase ProjPrefab;

    public override bool IsCastable()
    {
        return base.IsCastable() && SkillMotion.IsChangable();
    }

    public override void OnPressingInput()
    {
        base.OnPressingInput();

        if (IsCastable())
        {
            if (mBaseObj.StateMachine.TryChangeState(SkillMotion))
            {
                StartCoroutine(CoSkillCastingSeq());
            }
        }
    }

    IEnumerator CoSkillCastingSeq()
    {
        ProjectileBase proj = CreateSkillProj();
        yield return new WaitUntil(() => !SkillMotion.IsCurrentThisState() || SkillMotion.FireIndex >= 0);
        if (!SkillMotion.IsCurrentThisState())
        {
            proj.DestroyNow();
        }
        else
        {
            UseMana();
            StartCooltime();

            proj.StartProjectile();
        }
    }

    ProjectileBase CreateSkillProj()
    {
        // 스킬 오브젝트 생성
        Vector2 startPos = mBaseObj.Body.Center + new Vector2(transform.right.x, 0);
        ProjectileBase proj = ProjectileBase.Create(ProjPrefab, startPos, mBaseObj.transform.rotation, mBaseObj.gameObject.layer);

        ApplySkillStatsToProjectile(proj);

        proj.OnHit.AddListener((col) =>
        {
            // 충돌 시 처리할 내용
            Health health = col.ExGetBase().GetComponentInChildren<Health>();
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
        });
        return proj;
    }

}
