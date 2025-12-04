using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NaughtyAttributes;
using PahlBit;
using UnityEngine;
using UnityEngine.InputSystem;

public class SkillMelee : SkillObject
{
    [SerializeField] FiniteStateBase SkillMotion;
    [SerializeField] ProjectileBase MeleePrefab;

    public override bool IsCastable()
    {
        // 나중에 추가로 쿨타임이나 스턴같은 경우에 대한 조건 추가
        return SkillMotion.IsChangable();
    }

    public override void UpdateSkill()
    {
        base.UpdateSkill();

        if (mInput.JustPressed(GetCurrentInputType()) && IsCastable())
        {
            mBaseObj.StateMachine.ChangeState(SkillMotion);
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
        Vector3 startPos = transform.position + new Vector3(transform.right.x, 0, 0);
        ProjectileBase obj = ProjectileBase.Create(MeleePrefab, startPos, Quaternion.identity, this);
        obj.OnHit.AddListener((col) =>
        {
            // 충돌 시 처리할 내용
            EnemyBase enemy = col.GetComponentInParent<EnemyBase>();
            if (enemy != null)
            {
                enemy.GetDamaged((int)BaseStats.Attack, mBaseObj.transform.right);
            }
        });
    }

}
