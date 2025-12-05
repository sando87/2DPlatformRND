using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NaughtyAttributes;
using PahlBit;
using UnityEngine;
using UnityEngine.InputSystem;

public class SkillBall : SkillObject
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
        ProjectileBase obj = ProjectileBase.Create(CirclePrefab, startPos, mBaseObj.transform.rotation, BaseStats, mBaseObj.gameObject.layer);
        obj.OnHit.AddListener((col) =>
        {
            // 충돌 시 처리할 내용
            EnemyBase enemy = col.GetComponentInParent<EnemyBase>();
            if (enemy != null)
            {
                LOG.trace(BaseStats.Attack);
                int damage = (int)BaseStats.Attack % 3;
                enemy.GetDamaged(damage + 1, mBaseObj.transform.right);
            }
        });
    }

}
