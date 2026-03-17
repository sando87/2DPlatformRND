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
    [SerializeField] int ChainCount = 2;

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

        BaseObject target = UtilitiesPhy2D.CircleCast(startPos, 1.5f, dir, Spec.AttackRange, targetLayerMask, interactMask);
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

                this.ExDelayedCoroutine(ChainInterval, () => DoChainLightning(target.Body.Center, Spec.SplashRange, ChainCount, alreadyHitTargets));
            }
        }
        else
        {
            // 스킬 오브젝트 생성
            ProjectileBase proj = ProjectileBase.Create(ProjPrefab, startPos, mBaseObj.transform.right, mBaseObj.gameObject.layer);
            ApplySkillStatsToProjectile(proj);
        }
    }

    void DoChainLightning(Vector2 cenPos, float radius, int remainChainCount, List<BaseObject> alreadyHitTargets)
    {
        if (remainChainCount <= 0)
            return;

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

                this.ExDelayedCoroutine(ChainInterval, () => DoChainLightning(target.Body.Center, Spec.SplashRange, remainChainCount - 1, alreadyHitTargets));
            }
        }
    }

    BaseObject FindNextTarget(List<BaseObject> targets, Vector2 cenPos, List<BaseObject> alreadyHitTargets)
    {
        // 다음 체인 라이트닝 타겟 찾는 로직
        // 너무 가까이 있는 적은 스킵
        // 일정 거리 안에 있는 적들 중 랜덤하게 한명 선택
        // 일정 거리 안에 타겟이 없으면 그냥 제일 가까웠던 적을 타겟으로 한다
        int startIndex = MyUtils.RandomInt(0, targets.Count);
        BaseObject closeTarget = null;
        for (int i = 0; i < targets.Count; i++)
        {
            BaseObject target = targets[(startIndex + i) % targets.Count];
            if (alreadyHitTargets.Contains(target))
                continue;

            // if (!target.Health.IsFreezed)
            //     continue;

            float dist = (target.Body.Center - cenPos).magnitude;
            if (dist < 1)
            {
                closeTarget = target;
                continue;
            }

            return target;
        }

        return closeTarget;
    }


}
