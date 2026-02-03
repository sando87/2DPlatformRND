using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NaughtyAttributes;
using PahlBit;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.InputSystem;

public class SkillGun : SkillObject
{
    [SerializeField] PlayerStateGeneral SkillMotion;
    [SerializeField] ProjectileBase ProjectilePrefab;
    [SerializeField][Range(0, 1)] float RandomDestPos = 0.5f;

    public override bool IsCastable()
    {
        return SkillMotion.IsChangable();
    }

    public override void UpdateSkill()
    {
        base.UpdateSkill();

        if (mInput.IsPressing(GetCurrentInputType()) && IsCastable())
        {
            mBaseObj.StateMachine.ChangeState(SkillMotion, (Action)DoFire);
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

        Vector2 desPos = startPos + (BaseStats.ProjectileDistance * proj.transform.right.ExToVector2());
        Vector2 ranDesPos = MyUtils.Random(desPos, RandomDestPos);
        Vector2 diff = ranDesPos - startPos;
        proj.transform.right = (ranDesPos - startPos).normalized;
        proj.Stats.MaxDistance = diff.magnitude;

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
