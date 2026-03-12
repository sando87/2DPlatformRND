using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NaughtyAttributes;
using PahlBit;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class SkillLightning : SkillBase
{
    [SerializeField] PlayerStateGeneral SkillMotion;
    [SerializeField] ProjectileBase ProjPrefab;
    [SerializeField] float ChainInterval = 0.15f;

    public UnityEvent<BaseObject> OnHit;

    public override bool IsCastable()
    {
        return SkillMotion.IsChangable();
    }

    public override void UpdateSkill()
    {
        base.UpdateSkill();

        if (mInput.JustPressed(GetCurrentInputType()) && IsCastable())
        {
            mBaseObj.StateMachine.TryChangeState(SkillMotion, (Action)DoFire);
        }
    }

    public override void DoFire()
    {
        base.DoFire();

        List<BaseObject> alreadyHitTargets = new List<BaseObject>();
        DoCastSkill(alreadyHitTargets);
    }

    void DoCastSkill(List<BaseObject> alreadyHitTargets)
    {
        Vector2 startPos = mBaseObj.Body.Center + new Vector2(transform.right.x, 0);
        Vector2 dir = mBaseObj.transform.right;
        int targetLayerMask = GameSystem.GetAttackableLayerMask(gameObject.layer);
        InteractMask interactMask = InteractMask.Unit;

        BaseObject target = UtilitiesPhy2D.CircleCast(startPos, 0.5f, dir, Spec.AttackRange, targetLayerMask, interactMask);
        if (target != null)
        {
            alreadyHitTargets.Add(target);

            Vector2 diff = target.Body.Center - startPos;
            ProjectileBase proj = ProjectileBase.Create(ProjPrefab, startPos, diff.normalized, mBaseObj.gameObject.layer);
            ApplySkillStatsToProjectile(proj);

            float scale = diff.magnitude / 4f; // 여기서 4는 라이트닝 기본 스케일의 이펙트 길이
            proj.transform.localScale = new Vector3(scale, 1, 1);

            // 충돌 시 처리할 내용
            Health health = target.GetComponentInChildren<Health>();
            if (health != null)
            {
                DamageInfo damageInfo = Spec.CalcCurrentDamages();
                health.GetDamaged(damageInfo);

                AttackResult result = new AttackResult()
                {
                    Target = target,
                    IsKilled = health.IsDead,
                };
                mBaseObj.GetComponentInChildren<BattleDispatcher>()?.DispatchAttackResult(result);

                OnHit?.Invoke(target);

                this.ExDelayedCoroutine(ChainInterval, () => DoChainLightning(target.Body.Center, Spec.SplashRange, alreadyHitTargets));
            }
        }
        else
        {
            // 스킬 오브젝트 생성
            ProjectileBase proj = ProjectileBase.Create(ProjPrefab, startPos, mBaseObj.transform.right, mBaseObj.gameObject.layer);
            ApplySkillStatsToProjectile(proj);
        }
    }

    void DoChainLightning(Vector2 cenPos, float radius, List<BaseObject> alreadyHitTargets)
    {
        Vector2 startPos = cenPos;
        Vector2 dir = mBaseObj.transform.right;
        int targetLayerMask = GameSystem.GetAttackableLayerMask(gameObject.layer);
        InteractMask interactMask = InteractMask.Unit;

        List<BaseObject> rets = TemporaryList<BaseObject>.StaticTempList;
        rets.Clear();
        int retCount = UtilitiesPhy2D.OverlapCircleAll(startPos, radius, targetLayerMask, interactMask, rets);
        if (retCount > 0)
        {
            BaseObject target = FindNextTarget(rets, startPos, alreadyHitTargets);
            if (target == null)
                return;

            alreadyHitTargets.Add(target);

            Vector2 diff = target.Body.Center - startPos;
            ProjectileBase proj = ProjectileBase.Create(ProjPrefab, startPos, diff.normalized, mBaseObj.gameObject.layer);
            ApplySkillStatsToProjectile(proj);

            float scale = diff.magnitude / 4f; // 여기서 4는 라이트닝 기본 스케일의 이펙트 길이
            proj.transform.localScale = new Vector3(scale, 1, 1);

            // 충돌 시 처리할 내용
            Health health = target.GetComponentInChildren<Health>();
            if (health != null)
            {
                DamageInfo damageInfo = Spec.CalcCurrentDamages();
                health.GetDamaged(damageInfo);

                AttackResult result = new AttackResult()
                {
                    Target = target,
                    IsKilled = health.IsDead,
                };
                mBaseObj.GetComponentInChildren<BattleDispatcher>()?.DispatchAttackResult(result);

                OnHit?.Invoke(target);

                this.ExDelayedCoroutine(ChainInterval, () => DoChainLightning(target.Body.Center, Spec.SplashRange, alreadyHitTargets));
            }
        }
    }

    BaseObject FindNextTarget(List<BaseObject> targets, Vector2 cenPos, List<BaseObject> alreadyHitTargets)
    {
        BaseObject mostCloseTarget = null;
        float minDist = float.PositiveInfinity;
        foreach (BaseObject target in targets)
        {
            if (alreadyHitTargets.Contains(target))
                continue;

            if (!target.Health.IsFreezed)
                continue;

            float dist = (target.Body.Center - cenPos).sqrMagnitude;
            if (dist < minDist)
            {
                minDist = dist;
                mostCloseTarget = target;
            }
        }
        return mostCloseTarget;
    }


}
