using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NaughtyAttributes;
using PahlBit;
using UnityEngine;
using UnityEngine.InputSystem;

public class SkillFireBall : SkillBase
{
    [SerializeField] PlayerStateGeneral SkillMotion;
    [SerializeField] ProjectileBase CirclePrefab;

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

        DoCastSkill();
    }

    public void DoCastSkill()
    {
        // 스킬 오브젝트 생성
        Vector2 startPos = mBaseObj.Body.Center + new Vector2(transform.right.x, 0);
        ProjectileBase obj = ProjectileBase.Create(CirclePrefab, startPos, mBaseObj.transform.rotation, mBaseObj.gameObject.layer);
        DamageInfo damageInfo = Spec.CalcCurrentDamages();
        obj.OnHit.AddListener((col) =>
        {
            // 충돌 시 처리할 내용
            Health health = col.ExGetBase().GetComponentInChildren<Health>();
            if (health != null)
            {

                health.GetDamaged(damageInfo);

                AttackResult result = new AttackResult()
                {
                    Target = col.ExGetBase(),
                    IsKilled = health.IsDead,
                };
                mBaseObj.GetComponentInChildren<BattleDispatcher>()?.DispatchAttackResult(result);

                obj.DoEndProjectile();
            }

            BuffController buffCtrl = col.ExGetBase().Buffs;
            if (buffCtrl != null)
            {
                BuffInfo buffInfo = new BuffInfo();
                buffInfo.FireDamagePerSec = damageInfo.FireDamage * 0.3f;
                buffInfo.Duration = Spec.Duration;

                buffCtrl.ApplyBuff(buffInfo);
            }
        });
    }

}
