using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NaughtyAttributes;
using PahlBit;
using UnityEngine;
using UnityEngine.InputSystem;

public class SkillMeleeCombo : SkillBase
{
    [SerializeField] PlayerStateCombo SkillMotion;
    [SerializeField] ProjectileBase MeleePrefab;
    [SerializeField] AudioClip[] _Clips = null;

    public override bool IsCastable()
    {
        return base.IsCastable() && SkillMotion.Priority >= mBaseObj.StateMachine.GetCurrentState().Priority;
    }

    public override void UpdateSkill()
    {
        base.UpdateSkill();

        if (mInput.JustPressed(GetCurrentInputType()) && IsCastable())
        {
            if (mBaseObj.StateMachine.GetCurrentState() == SkillMotion)
            {
                SkillMotion.DoNextCombo();
            }
            else
            {
                mBaseObj.StateMachine.TryChangeState(SkillMotion, (Action<int>)DoFireIndex);
            }
        }
    }

    public void DoFireIndex(int comboIndex)
    {
        base.DoFire();

        UseMana();

        SoundPlayManager.Instance.PlaySFXClip(_Clips[comboIndex % _Clips.Length]);

        DoCastSkill();
    }

    public void DoCastSkill()
    {
        // 스킬 오브젝트 생성
        Vector2 startPos = mBaseObj.Body.Center + new Vector2(transform.right.x, 0);
        ProjectileBase proj = ProjectileBase.Create(MeleePrefab, startPos, mBaseObj.transform.rotation, mBaseObj.gameObject.layer);

        // ApplySkillStatsToProjectile(proj);

        proj.OnHit.AddListener((col) =>
        {
            // 충돌 시 처리할 내용
            Health health = col.ExGetBase().GetComponentInChildren<Health>();
            if (health != null)
            {
                DamageInfo damage = Spec.CalcCurrentDamages();
                health.GetDamaged(damage);

                AttackResult result = new AttackResult()
                {
                    Target = col.ExGetBase(),
                    IsKilled = health.IsDead,
                };
                mBaseObj.GetComponentInChildren<BattleDispatcher>()?.DispatchAttackResult(result);
            }
        });
    }

}
