using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NaughtyAttributes;
using PahlBit;
using UnityEngine;
using UnityEngine.InputSystem;

public class SkillLaserBeam : SkillBase
{
    [SerializeField] ProjectileBase ProjPrefab;
    [SerializeField] GameObject BeamMuzzle;
    [SerializeField] float _FireInterval = 0.1f;

    IEnumerator Start()
    {
        while (true)
        {
            yield return new WaitUntil(() => mInput.IsPressing(GetCurrentInputType()) && !IsCooltime);
            mBaseObj.AnimHelper.SetParamBool(AnimatorParams.IsAttacking, true);
            BeamMuzzle.SetActive(true);

            Coroutine delayForFireProj = this.ExRepeatCoroutine(_FireInterval, () => CreateSkillProj());
            yield return new WaitUntil(() => !mInput.IsPressing(GetCurrentInputType()));

            if (delayForFireProj != null)
                StopCoroutine(delayForFireProj);

            StartCooltime();
            mBaseObj.AnimHelper.SetParamBool(AnimatorParams.IsAttacking, false);
            BeamMuzzle.SetActive(false);
        }
    }

    public ProjectileBase CreateSkillProj()
    {
        // 스킬 오브젝트 생성
        Vector2 startPos = BeamMuzzle.transform.position;
        startPos = MyUtils.Random(startPos, 0.2f);
        ProjectileBase proj = ProjectileBase.Create(ProjPrefab, startPos, mBaseObj.transform.rotation, mBaseObj.gameObject.layer);

        ApplySkillStatsToProjectile(proj);

        proj.OnHit.AddListener((col) =>
        {
            InteractableCollider ic = col.ExGetCompInBase<InteractableCollider>();
            if (ic != null && ic.MyProperty.HasFlag(InteractMask.Terrain))
            {
                proj.DoEndProjectile();
                return;
            }

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
