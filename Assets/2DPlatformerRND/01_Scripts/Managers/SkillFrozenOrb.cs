using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NaughtyAttributes;
using PahlBit;
using UnityEngine;
using UnityEngine.InputSystem;

public class SkillFrozenOrb : SkillBase
{
    [SerializeField] PlayerStateGeneral SkillMotion;
    [SerializeField] ProjectileBase ProjPrefab;

    IEnumerator Start()
    {
        while (true)
        {
            yield return new WaitUntil(() => mInput.IsPressing(GetCurrentInputType()) && IsCastable());
            mBaseObj.AnimHelper.SetParamBool(AnimatorParams.IsAttacking, true);

            ProjectileBase proj = null;
            Coroutine delayForFireProj = null;
            while (mInput.IsPressing(GetCurrentInputType()))
            {
                if (!IsCooltime && delayForFireProj == null)
                {
                    proj = CreateSkillProj();
                    proj.transform.SetParent(transform);
                    delayForFireProj = this.ExDelayedCoroutine(0.4f, () =>
                    {
                        UseMana();

                        proj.transform.SetParent(null);
                        proj.StartProjectile();
                        proj = null;
                        delayForFireProj = null;
                    });
                }
                yield return null;
            }

            StartCooltime();

            if (proj != null)
                proj.DestroyNow();

            if (delayForFireProj != null)
                StopCoroutine(delayForFireProj);

            mBaseObj.AnimHelper.SetParamBool(AnimatorParams.IsAttacking, false);
        }
    }

    public ProjectileBase CreateSkillProj()
    {
        // 스킬 오브젝트 생성
        Vector2 startPos = mBaseObj.Body.Foot + new Vector2(transform.right.x, 0);
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
