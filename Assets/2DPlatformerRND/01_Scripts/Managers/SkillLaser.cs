using System;
using PahlBit;
using UnityEngine;

public class SkillLaser : SkillBase
{
    [SerializeField] PlayerStateAttackLoop SkillMotion;
    [SerializeField] ProjectileBase LaserPrefab;

    private ProjectileBase mLaser = null;

    public override bool IsCastable()
    {
        return SkillMotion.IsChangable() && mLaser == null;
    }

    public override void UpdateSkill()
    {
        base.UpdateSkill();

        if (mInput.JustPressed(GetCurrentInputType()) && IsCastable())
        {
            mBaseObj.StateMachine.TryChangeState(SkillMotion, (Action)DoFire);
            DoCastSkill();
            // Create Muzzle Effect
            this.ExConditionCoroutine(() => !mInput.IsPressing(GetCurrentInputType()), OnReleaseSkill);
            this.ExConditionCoroutine(() => !SkillMotion.IsCurrentThisState(), OnLeaveStateMotion);
        }
    }

    // 스킬 채널링 종료된 순간(누르고 있는 버튼에서 손가락을 뗀 순간 호출됨 : 정상루틴)
    void OnReleaseSkill()
    {
        if (mLaser != null)
        {
            mLaser.DoEndProjectile();
            mLaser = null;
            SkillMotion.StopAttack();
        }
        else
        {
            SkillMotion.DoLeaveCurrentState();
        }

        StopAllCoroutines();
    }

    // 스킬 시전 중 다치거나 죽거나 하는 등의 다른 동작에 의해 강제로 스킬 채널링 종료되는 경우
    void OnLeaveStateMotion()
    {
        if (mLaser != null)
        {
            mLaser.DestroyNow();
            mLaser = null;
        }

        StopAllCoroutines();
    }

    // 스킬 채널링이 시작된 순간
    public override void DoFire()
    {
        base.DoFire();

        if (mLaser != null)
        {
            mLaser.StartProjectile();
        }
    }

    public void DoCastSkill()
    {
        // 스킬 오브젝트 생성
        Vector2 startPos = mBaseObj.Body.Center + new Vector2(transform.right.x, 0.2f);
        mLaser = ProjectileBase.Create(LaserPrefab, startPos, mBaseObj.transform.rotation, mBaseObj.gameObject.layer);

        ApplySkillStatsToProjectile(mLaser);

        mLaser.OnHit.AddListener((col) =>
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
    }

}
