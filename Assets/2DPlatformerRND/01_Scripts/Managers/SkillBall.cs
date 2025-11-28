using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NaughtyAttributes;
using PahlBit;
using UnityEngine;
using UnityEngine.InputSystem;

public class SkillBall : SkillObject
{
    [SerializeField] ProjectileBase CirclePrefab;

    public override bool IsCastable()
    {
        return base.IsCastable();
    }

    public override void DoCast()
    {
        base.DoCast();

        DoCastSkill();
    }

    public void DoCastSkill()
    {
        // 스킬 오브젝트 생성
        Vector3 startPos = transform.position + new Vector3(transform.right.x, 0, 0);
        ProjectileBase obj = Instantiate(CirclePrefab, startPos, Quaternion.identity);
        obj.DoCast(this, (col) =>
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
