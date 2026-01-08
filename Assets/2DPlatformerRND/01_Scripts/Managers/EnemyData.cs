using System.ComponentModel;
using DG.Tweening;
using NaughtyAttributes;
using PahlBit;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class EnemyData
{
    public EnemyResourceData ResourceData { get; private set; } = null;

    [field: SerializeField]
    public EnemyStats Stats { get; private set; } = null;

    public void InitData(string enemyID)
    {
        ResourceData = EnemyResourceTable.Instance.GetInfo(enemyID);
        UpdateStats();
    }
    public void UpdateStats()
    {
        Stats = new EnemyStats();

        Stats.Health = ResourceData._Health.GetValue();
        Stats.Attack = ResourceData._Attack.GetValue();
        Stats.Defence = ResourceData._Defence.GetValue();
        Stats.MoveSpeed = ResourceData._MoveSpeed.GetValue();
        Stats.AttackSpeed = ResourceData._AttackSpeed.GetValue();
        Stats.Cooltime = ResourceData._Cooltime.GetValue();
        Stats.DetectRange = ResourceData._DetectRange.GetValue();
        Stats.AttackRange = ResourceData._AttackRange.GetValue();
        Stats.ItemDrop = ResourceData._ItemDrop.GetValue();
        Stats.GoldOnDeath = ResourceData._GoldOnDeath.GetValueInRange(MyUtils.RandomRate());
        Stats.ExpOnDeath = ResourceData._ExpOnDeath.GetValue();
    }
}
